using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database 
{
	[Table("users_relationships")]
    public class Friend
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int user1 { get; set; }

        [Required]
        public int user2 { get; set; }
    }
}