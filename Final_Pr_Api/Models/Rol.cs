using System.ComponentModel.DataAnnotations;

namespace Final_Pr_Api.Models
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }
        public string name { get; set; }

    }
}
