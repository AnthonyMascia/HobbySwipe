using System.Security.Claims;

namespace HobbySwipe.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.FindFirstValue(ClaimTypes.Email);
        }

        public static bool IsInRole(this ClaimsPrincipal principal, string roleName)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.IsInRole(roleName) ?? false;
        }

        public static bool HasClaim(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.HasClaim(c => c.Type == claimType) ?? false;
        }

        public static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.FindFirstValue(claimType);
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal?.FindAll(ClaimTypes.Role).Select(claim => claim.Value) ?? Enumerable.Empty<string>();
        }

    }
}
