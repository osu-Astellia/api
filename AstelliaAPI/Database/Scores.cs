using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("scores")]
    public class Score : IScore
    {
        [Required] public byte completed { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string beatmap_md5 { get; set; }

        [Required] public int userid { get; set; }

        [Required] public int score { get; set; }

        [Required] public int max_combo { get; set; }

        [Required] public int mods { get; set; }

        [Required] [Column("300_count")] public int c300 { get; set; }

        [Required] [Column("100_count")] public int c100 { get; set; }

        [Required] [Column("50_count")] public int c50 { get; set; }

        [Required] [Column("gekis_count")] public int cGeki { get; set; }

        [Required] [Column("katus_count")] public int cKatu { get; set; }

        [Required] [Column("misses_count")] public int cMiss { get; set; }

        [Required] public string time { get; set; }

        [Required] public sbyte play_mode { get; set; }

        [Required] public double accuracy { get; set; }

        [Required] public float pp { get; set; }
    }

    [Table("scores_relax")]
    public class ScoreRelax : IScore
    {
        [Required] public byte completed { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string beatmap_md5 { get; set; }

        [Required] public int userid { get; set; }

        [Required] public int score { get; set; }

        [Required] public int max_combo { get; set; }

        [Required] public int mods { get; set; }

        [Required] [Column("300_count")] public int c300 { get; set; }

        [Required] [Column("100_count")] public int c100 { get; set; }

        [Required] [Column("50_count")] public int c50 { get; set; }

        [Required] [Column("gekis_count")] public int cGeki { get; set; }

        [Required] [Column("katus_count")] public int cKatu { get; set; }

        [Required] [Column("misses_count")] public int cMiss { get; set; }

        [Required] public string time { get; set; }

        [Required] public sbyte play_mode { get; set; }

        [Required] public double accuracy { get; set; }

        [Required] public float pp { get; set; }
    }
}