using System.ComponentModel.DataAnnotations;

namespace Final_Pr_Api.Models
{
    public class Comment
    {
        [Key]
        public int idComment { get; set; }
        public string text { get; set; }
        public int authorId { get; set; }
        public int postId { get; set; }
        public DateTime? createTime { get; set; }

    }
}
