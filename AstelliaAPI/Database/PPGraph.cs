using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("pp_graph")]
    public class PPGraph
    {
        [Required] public int user { get; set; }

        [Required] public DateTime time { get; set; }

        [Required] public int pp { get; set; }

        [Required] public bool is_relax { get; set; }

        [Required] public int gamemode { get; set; }
    }
}