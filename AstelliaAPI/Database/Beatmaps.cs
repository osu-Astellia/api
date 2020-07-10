using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("beatmaps")]
    public class Beatmap
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string beatmap_md5 { get; set; }

        [Required] public int beatmap_id { get; set; }

        [Required] public int beatmapset_id { get; set; }


        [Required] public string song_name { get; set; }

        [Required] public RankedStatus ranked { get; set; }
    }

    public enum RankedStatus : sbyte
    {
        Unknown = -2,
        NotSubmited = -1,
        LatestPending = 0,
        NeedUpdate = 1,
        Ranked = 2,
        Approved = 3,
        Qualified = 4,
        Loved = 5
    }
}