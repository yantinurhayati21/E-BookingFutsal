using System.Security.Claims;

namespace E_BookingFutsal.Helper
{
    public static class GetIdentity
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                return user.Claims.FirstOrDefault(x => x.Type == "Username")?.Value ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
