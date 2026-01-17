namespace DPNS.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this System.Security.Claims.ClaimsPrincipal user)
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }
            return userId;
        }
    }
}
