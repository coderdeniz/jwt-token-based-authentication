namespace UdemyCore.Models
{
    public class UserRefreshToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
