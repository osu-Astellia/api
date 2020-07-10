using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("tokens")]
    public class Token
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string user { get; set; }

        [Required] public string token { get; set; }

        [Required] public int privileges { get; set; }
    }
}