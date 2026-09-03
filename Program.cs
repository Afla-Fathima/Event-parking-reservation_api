using System.Text;
using EventParkingReservation.Data;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Implementations;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder =
    WebApplication.CreateBuilder(args);

// ==========================================
// CONTROLLERS + SWAGGER
// ==========================================

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer();

builder.Services
    .AddSwaggerGen();

// ==========================================
// DATABASE
// ==========================================

builder.Services
    .AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.UseSqlServer(
                builder.Configuration
                    .GetConnectionString(
                        "DefaultConnection"));
        });

// ==========================================
// REPOSITORIES
// ==========================================

builder.Services
    .AddScoped<IAuthRepository,
        AuthRepository>();

builder.Services
    .AddScoped<IBookingRepository,
        BookingRepository>();

builder.Services
    .AddScoped<ICategoryRepository,
        CategoryRepository>();

builder.Services
    .AddScoped<ICustomerRepository,
        CustomerRepository>();

builder.Services
    .AddScoped<IEventRepository,
        EventRepository>();

builder.Services
    .AddScoped<INotificationRepository,
        NotificationRepository>();

builder.Services
    .AddScoped<IParkingSlotRepository,
        ParkingSlotRepository>();

builder.Services
    .AddScoped<IPaymentRepository,
        PaymentRepository>();

builder.Services
    .AddScoped<ISeatRepository,
        SeatRepository>();

builder.Services
    .AddScoped<IVenueRepository,
        VenueRepository>();

// ==========================================
// SERVICES
// ==========================================

builder.Services
    .AddScoped<IAuthService,
        AuthService>();

builder.Services
    .AddScoped<IBookingService,
        BookingService>();

builder.Services
    .AddScoped<ICategoryService,
        CategoryService>();

builder.Services
    .AddScoped<ICustomerService,
        CustomerService>();

builder.Services
    .AddScoped<IEventService,
        EventService>();

builder.Services
    .AddScoped<INotificationService,
        NotificationService>();

builder.Services
    .AddScoped<IParkingSlotService,
        ParkingSlotService>();

builder.Services
    .AddScoped<IPaymentService,
        PaymentService>();

builder.Services
    .AddScoped<ISeatService,
        SeatService>();

builder.Services
    .AddScoped<IVenueService,
        VenueService>();

// ==========================================
// JWT AUTHENTICATION
// ==========================================

string jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt Key is missing.");

string? issuer =
    builder.Configuration["Jwt:Issuer"];

string? audience =
    builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(
        JwtBearerDefaults
            .AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = issuer,

                ValidAudience = audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8
                            .GetBytes(jwtKey)),

                ClockSkew =
                    TimeSpan.Zero
            };
    });

builder.Services
    .AddAuthorization();

// ==========================================
// BUILD
// ==========================================

var app = builder.Build();

// ==========================================
// HTTP PIPELINE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();