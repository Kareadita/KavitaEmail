using System.Security.Authentication;
using System.Security.Claims;

namespace Skeleton.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var userClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null) throw new AuthenticationException("User is not authenticated");
            return userClaim.Value;
        }
    }
}