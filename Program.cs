using EventParkingReservation.BackgroundServices;
using EventParkingReservation.Data;
using EventParkingReservation.Middleware;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Implementations;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;



var builder =
    WebApplication.CreateBuilder(args);

// =====================================================
// CONTROLLERS
// =====================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// =====================================================
// DATABASE
// =====================================================

string connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is missing.");

builder.Services
    .AddDbContext<ApplicationDbContext>(
        options =>
        {
            options.UseSqlServer(
                connectionString);
        });

// =====================================================
// CORS - ANGULAR
// =====================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularFrontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// =====================================================
// REPOSITORIES
// =====================================================

builder.Services.AddScoped<
    IAuthRepository,
    AuthRepository>();

builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

builder.Services.AddScoped<
    IVenueRepository,
    VenueRepository>();

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository>();

builder.Services.AddScoped<
    IEventRepository,
    EventRepository>();

builder.Services.AddScoped<
    ISeatRepository,
    SeatRepository>();

builder.Services.AddScoped<
    IParkingRepository,
    ParkingRepository>();

builder.Services.AddScoped<
    IBookingRepository,
    BookingRepository>();

builder.Services.AddScoped<
    IPaymentRepository,
    PaymentRepository>();

builder.Services.AddScoped<
    INotificationRepository,
    NotificationRepository>();

builder.Services.AddScoped<
    IDashboardRepository,
    DashboardRepository>();

// =====================================================
// SERVICES
// =====================================================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

builder.Services.AddScoped<
    IVenueService,
    VenueService>();

builder.Services.AddScoped<
    ICategoryService,
    CategoryService>();

builder.Services.AddScoped<
    IEventService,
    EventService>();

builder.Services.AddScoped<
    ISeatService,
    SeatService>();

builder.Services.AddScoped<
    IParkingService,
    ParkingService>();

builder.Services.AddScoped<
    IBookingService,
    BookingService>();

builder.Services.AddScoped<
    IPaymentService,
    PaymentService>();

builder.Services.AddScoped<
    INotificationService,
    NotificationService>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();

// =====================================================
// BACKGROUND WORKER
// =====================================================

builder.Services.AddHostedService<
    BookingExpiryService>();

// =====================================================
// JWT SETTINGS
// =====================================================

string jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is missing.");

string jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is missing.");

string jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience is missing.");

// =====================================================
// AUTHENTICATION
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults
                .AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults
                .AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults
                .AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata =
            false;

        options.SaveToken =
            true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer =
                    true,

                ValidateAudience =
                    true,

                ValidateLifetime =
                    true,

                ValidateIssuerSigningKey =
                    true,

                ValidIssuer =
                    jwtIssuer,

                ValidAudience =
                    jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtKey)),

                RoleClaimType =
                    System.Security.Claims
                        .ClaimTypes.Role,

                NameClaimType =
                    System.Security.Claims
                        .ClaimTypes.Name,

                ClockSkew =
                    TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// =====================================================
// SWAGGER + JWT AUTHORIZE BUTTON
// =====================================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title =
                "Event Parking Reservation API",

            Version =
                "v1",

            Description =
                "Event and Parking Reservation System API"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name =
                "Authorization",

            Type =
                SecuritySchemeType.Http,

            Scheme =
                "bearer",

            BearerFormat =
                "JWT",

            In =
                ParameterLocation.Header,

            Description =
                "Enter the JWT token generated by /api/Auth/login."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id =
                                "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});

// =====================================================
// BUILD
// =====================================================

var app =
    builder.Build();

// =====================================================
// GLOBAL EXCEPTION HANDLER
// =====================================================

app.UseMiddleware<
    ExceptionMiddleware>();

// =====================================================
// SWAGGER
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Event Parking Reservation API v1");

        options.DocumentTitle =
            "Event Parking Reservation API";
    });
}

// =====================================================
// HTTP PIPELINE
// =====================================================

app.UseHttpsRedirection();

app.UseCors(
    "AngularFrontend");

// IMPORTANT:
// Authentication before Authorization.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await AdminSeeder.SeedAsync(dbContext);
}

app.Run();