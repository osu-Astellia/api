using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace AstelliaAPI.Database
{
    [Table("users")]
    public class User
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] public string username { get; set; }

        [Required] public string username_safe { get; set; }

        [Required] public string password_md5 { get; set; }

        [Required] public string email { get; set; }

        [Required] public int register_datetime { get; set; }

        [Required] public int password_version { get; set; }

        [Required] public byte[]? ssalt { get; set; }

        [Required] public RipplePrivileges privileges { get; set; }

        [Required] public bool nc_instead_dt { get; set; }
    }

    [Table("users_stats")]
    public class UserStats : IStats
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required] public string? userpage_content { get; set; }

        [Required] public string? country { get; set; }

        [Required] public int play_style { get; set; } 
        [Required] public int favourite_mode { get; set; }

        [Required] public string custom_badge_icon { get; set; }

        [Required] public string custom_badge_name { get; set; }

        [Required] public bool show_custom_badge { get; set; }

        [Required] public string username { get; set; }
        
        [Required] public int level_std { get; set; }
        [Required] public int level_taiko { get; set; }
        [Required] public int level_ctb { get; set; }
        [Required] public int level_mania { get; set; }
        [Required] public long ranked_score_std { get; set; }
        [Required] public long ranked_score_taiko { get; set; }
        [Required] public long ranked_score_ctb { get; set; }
        [Required] public long ranked_score_mania { get; set; }
        [Required] public long total_score_std { get; set; }
        [Required] public long total_score_taiko { get; set; }
        [Required] public long total_score_ctb { get; set; }
        [Required] public long total_score_mania { get; set; }

        [Required] public int total_hits_std { get; set; }
        [Required] public int total_hits_taiko { get; set; }
        [Required] public int total_hits_ctb { get; set; }
        [Required] public int total_hits_mania { get; set; }


        [Required] public int replays_watched_std { get; set; }
        [Required] public int replays_watched_taiko { get; set; }
        [Required] public int replays_watched_ctb { get; set; }
        [Required] public int replays_watched_mania { get; set; }

        [Required] public int playcount_std { get; set; }
        [Required] public int playcount_taiko { get; set; }
        [Required] public int playcount_ctb { get; set; }
        [Required] public int playcount_mania { get; set; }

        [Required] public float avg_accuracy_std { get; set; }

        [Required] public float avg_accuracy_taiko { get; set; }

        [Required] public float avg_accuracy_ctb { get; set; }

        [Required] public float avg_accuracy_mania { get; set; }

        [Required] public int pp_std { get; set; }

        [Required] public int pp_taiko { get; set; }

        [Required] public int pp_ctb { get; set; }

        [Required] public int pp_mania { get; set; }
    }

    [Table("rx_stats")]
    public class RelaxStats : IStats
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required] public int favourite_mode { get; set; }
        [Required] public string username { get; set; }
        [Required] public string? country { get; set; }
        [Required] public int level_std { get; set; }
        [Required] public int level_taiko { get; set; }
        [Required] public int level_ctb { get; set; }
        [Required] public int level_mania { get; set; }
        [Required] public long ranked_score_std { get; set; }
        [Required] public long ranked_score_taiko { get; set; }
        [Required] public long ranked_score_ctb { get; set; }
        [Required] public long ranked_score_mania { get; set; }
        [Required] public long total_score_std { get; set; }
        [Required] public long total_score_taiko { get; set; }
        [Required] public long total_score_ctb { get; set; }
        [Required] public long total_score_mania { get; set; }

        [Required] public int total_hits_std { get; set; }
        [Required] public int total_hits_taiko { get; set; }
        [Required] public int total_hits_ctb { get; set; }
        [Required] public int total_hits_mania { get; set; }


        [Required] public int replays_watched_std { get; set; }
        [Required] public int replays_watched_taiko { get; set; }
        [Required] public int replays_watched_ctb { get; set; }
        [Required] public int replays_watched_mania { get; set; }
        [Required] public int playcount_std { get; set; }
        [Required] public int playcount_taiko { get; set; }
        [Required] public int playcount_ctb { get; set; }
        [Required] public int playcount_mania { get; set; }
        
        [Required] public float avg_accuracy_std { get; set; }
        [Required] public float avg_accuracy_taiko { get; set; }
        [Required] public float avg_accuracy_ctb { get; set; }
        [Required] public float avg_accuracy_mania { get; set; }
        [Required] public int pp_std { get; set; }
        [Required] public int pp_taiko { get; set; }
        [Required] public int pp_ctb { get; set; }
        [Required] public int pp_mania { get; set; }
    }

    [Flags]
    public enum RipplePrivileges
    {
        UserBanned = 0,
        UserPublic = 1,
        UserNormal = 2 << 0,
        UserDonor = 2 << 1,
        AdminAccessRAP = 2 << 2,
        AdminManageUsers = 2 << 3,
        AdminBanUsers = 2 << 4,
        AdminSilenceUsers = 2 << 5,
        AdminWipeUsers = 2 << 6,
        AdminManageBeatmaps = 2 << 7,
        AdminManageServers = 2 << 8,
        AdminManageSettings = 2 << 9,
        AdminManageBetaKeys = 2 << 10,
        AdminManageReports = 2 << 11,
        AdminManageDocs = 2 << 12,
        AdminManageBadges = 2 << 13,
        AdminViewRAPLogs = 2 << 14,
        AdminManagePrivileges = 2 << 15,
        AdminSendAlerts = 2 << 16,
        AdminChatMod = 2 << 17,
        AdminKickUsers = 2 << 18,
        UserPendingVerification = 2 << 19,
        UserTournamentStaff = 2 << 20,
        AdminCaker = 2 << 21,
        AdminViewTopScores = 2 << 22
    }

    public interface IStats
    {
        [Required] public string username { get; set; }

        [Required] public int pp_std { get; set; }

        [Required] public int pp_taiko { get; set; }

        [Required] public int pp_ctb { get; set; }

        [Required] public int pp_mania { get; set; }
        
        [Required] public int level_std { get; set; }
        [Required] public int level_taiko { get; set; }
        [Required] public int level_ctb { get; set; }
        [Required] public int level_mania { get; set; }


        [Required] public long ranked_score_std { get; set; }
        [Required] public long ranked_score_taiko { get; set; }
        [Required] public long ranked_score_ctb { get; set; }
        [Required] public long ranked_score_mania { get; set; }
        [Required] public long total_score_std { get; set; }
        [Required] public long total_score_taiko { get; set; }
        [Required] public long total_score_ctb { get; set; }
        [Required] public long total_score_mania { get; set; }

        [Required] public int total_hits_std { get; set; }
        [Required] public int total_hits_taiko { get; set; }
        [Required] public int total_hits_ctb { get; set; }
        [Required] public int total_hits_mania { get; set; }


        [Required] public int replays_watched_std { get; set; }
        [Required] public int replays_watched_taiko { get; set; }
        [Required] public int replays_watched_ctb { get; set; }
        [Required] public int replays_watched_mania { get; set; }

        [Required] public int playcount_std { get; set; }
        [Required] public int playcount_taiko { get; set; }
        [Required] public int playcount_ctb { get; set; }
        [Required] public int playcount_mania { get; set; }

        [Required] public float avg_accuracy_std { get; set; }

        [Required] public float avg_accuracy_taiko { get; set; }

        [Required] public float avg_accuracy_ctb { get; set; }

        [Required] public float avg_accuracy_mania { get; set; }
    }
}