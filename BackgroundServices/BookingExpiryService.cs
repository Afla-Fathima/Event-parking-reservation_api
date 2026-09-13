using EventParkingReservation.Data;
using EventParkingReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.BackgroundServices
{
    public class BookingExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<BookingExpiryService> _logger;


        public BookingExpiryService(
            IServiceScopeFactory scopeFactory,
            ILogger<BookingExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }



        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Booking expiry service started.");


            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExpireBookingsAsync(
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing expired bookings.");
                }


                try
                {
                    await Task.Delay(
                        TimeSpan.FromMinutes(1),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }


            _logger.LogInformation(
                "Booking expiry service stopped.");
        }




        private async Task ExpireBookingsAsync(
            CancellationToken cancellationToken)
        {
            using IServiceScope scope =
                _scopeFactory.CreateScope();


            ApplicationDbContext db =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();


            DateTime now =
                DateTime.UtcNow;



            var strategy =
                db.Database.CreateExecutionStrategy();



            await strategy.ExecuteAsync(async () =>
            {

                await using var transaction =
                    await db.Database
                        .BeginTransactionAsync(
                            cancellationToken);


                try
                {

                    List<Booking> expiredBookings =
                        await db.Bookings

                        .Include(b =>
                            b.BookingSeats)

                        .ThenInclude(bs =>
                            bs.Seat)

                        .Include(b =>
                            b.ParkingReservation)

                        .ThenInclude(pr =>
                            pr!.ParkingSlot)

                        .Where(b =>
                            b.Status == "Pending" &&
                            b.HoldExpiresAt != null &&
                            b.HoldExpiresAt <= now)

                        .ToListAsync(
                            cancellationToken);



                    if (expiredBookings.Count == 0)
                    {
                        await transaction.CommitAsync(
                            cancellationToken);

                        return;
                    }




                    foreach (Booking booking in expiredBookings)
                    {

                        booking.Status =
                            "Expired";


                        booking.PaymentStatus =
                            "Expired";



                        // RELEASE SEATS

                        foreach (
                            BookingSeat bookingSeat
                            in booking.BookingSeats)
                        {

                            if (bookingSeat.Status == "Active")
                            {
                                bookingSeat.Status =
                                    "Released";
                            }


                            if (bookingSeat.Seat != null)
                            {
                                bookingSeat.Seat.Status =
                                    "Available";
                            }

                        }




                        // RELEASE PARKING


                        ParkingReservation? parking =
                            booking.ParkingReservation;



                        if (parking != null &&
                            parking.Status == "Active")
                        {

                            parking.Status =
                                "Released";


                            if (parking.ParkingSlot != null)
                            {
                                parking.ParkingSlot.Status =
                                    "Available";
                            }

                        }




                        // CREATE NOTIFICATION


                        db.Notifications.Add(
                            new Notification
                            {

                                CustomerId =
                                    booking.CustomerId,


                                Title =
                                    "Booking Expired",


                                Message =
                                    $"Booking {booking.BookingNumber} expired because payment was not completed within 15 minutes. Reserved seats and parking have been released.",



                                Type =
                                    "Booking",



                                IsRead =
                                    false,



                                CreatedAt =
                                    DateTime.UtcNow

                            });

                    }




                    await db.SaveChangesAsync(
                        cancellationToken);



                    await transaction.CommitAsync(
                        cancellationToken);



                    _logger.LogInformation(
                        "Expired {Count} booking(s) and released their resources.",
                        expiredBookings.Count);

                }
                catch
                {

                    await transaction.RollbackAsync(
                        cancellationToken);

                    throw;

                }


            });

        }

    }
}