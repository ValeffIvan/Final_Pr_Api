using System.ComponentModel.DataAnnotations;

namespace Final_Pr_Api.Models
{
    public class User
    {
        [Key]
        public int idUsers { get; set; }
        public string password { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }
}
