namespace Final_Pr_Api.Models
{
    public class UserDetails
    {
        public int idUsers { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public DateTime createTime { get; set; }
        public string Role { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserDetails User { get; set; }
        public string Message { get; set; }
    }

}
