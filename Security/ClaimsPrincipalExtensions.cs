using System.Security.Claims;

namespace EventParkingReservation.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetCustomerId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value) ||
                !int.TryParse(value, out var customerId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid authenticated user.");
            }

            return customerId;
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role)
                   ?? string.Empty;
        }
    }
}