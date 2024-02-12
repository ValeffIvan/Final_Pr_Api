using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_Pr_Api.Models
{
    public class Comment
    {
        [Key]
        public int idComment { get; set; }
        public string text { get; set; }
        public int authorId { get; set; }
        public int postId { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }
    }
}
