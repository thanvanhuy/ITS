using System.Security.Claims;

namespace VVA.ITS.WebApp.Services
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserID(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
