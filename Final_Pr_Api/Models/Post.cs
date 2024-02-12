using System.ComponentModel.DataAnnotations;

namespace Final_Pr_Api.Models
{
    public class Post
    {
        [Key]
        public int idPost { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int authorId { get; set; }
        public DateTime createTime { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
