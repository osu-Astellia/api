using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstelliaAPI.Database
{
    [Table("updater_info")]
    public class UpdaterInfo
    {
        [Required] public string file_hash;

        [Required] public int file_version;

        [Required] public string filename;

        [Required] public long filesize;

        [Required] public int patch_from;

        [Required] public int patch_id;

        [Required] public string timestamp;

        [Required] public string url_full;

        [Required] public string url_patch;
    }
}