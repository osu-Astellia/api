using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("clans")]
    public class Clan
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string name { get; set; }

        [Required] public string description { get; set; }

        [Required] public string icon { get; set; }

        [Required] public string tag { get; set; }

        [Required] public string background { get; set; }
    }

    [Table("user_clans")]
    public class UserClan
    {
        [Required] public int user { get; set; }

        [Required] public int clan { get; set; }

        [Required] public int perms { get; set; }
    }
}