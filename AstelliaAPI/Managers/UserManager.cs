using System;
using System.Collections.Generic;
using System.Linq;
using AstelliaAPI.Database;
using Newtonsoft.Json;

namespace AstelliaAPI.Managers
{
    public class UserManager
    {
        private static readonly AstelliaDbContextFactory factory = new AstelliaDbContextFactory();

        // packetName is how its called on peppy server packets file
        // packetArgs format: ("hewwo, hewwo|1000") splitted by "|"
        public static void SendPacket(string userId, string packetName, string packetArgs)
        {
            var dict = new Dictionary<string, string>
            {
                ["userID"] = userId, ["packetName"] = packetName, ["args"] = packetArgs
            };
            Global.Redis.PublishMessage("peppy:packet_sender", JsonConvert.SerializeObject(dict));
        }
    
        // packetName is how its called on peppy server packets file
        // packetArgs format: ("hewwo, hewwo|1000") splitted by "|"
        public static void SendPacketToEveryone(string packetName, string packetArgs)
        {
            var dict = new Dictionary<string, string>
            {
                ["packetName"] = packetName, ["args"] = packetArgs
            };
            Global.Redis.PublishMessage("peppy:packet_sender_to_all", JsonConvert.SerializeObject(dict));
        }

        public static void SendNotification(string userId, string message)
        {
            var dict = new Dictionary<string, string> { ["userID"] = userId, ["message"] = message };
            Global.Redis.PublishMessage("peppy:notification", JsonConvert.SerializeObject(dict));
        }

        public static bool Allowed(int userid)
        {
            var user = factory.Get().Users.FirstOrDefault(x => x.id == userid);
            if (user is null) return false;
            return (user.privileges & (RipplePrivileges.UserPublic | RipplePrivileges.UserNormal)) > 0;
        }

        public static bool Restricted(int userid)
        {
            var user = factory.Get().Users.FirstOrDefault(x => x.id == userid);
            if (user is null) return false;
            return (user.privileges & RipplePrivileges.UserNormal) > 0 &&
                   !((user.privileges & RipplePrivileges.UserPublic) > 0);
        }

        public static string GetMode(int mode)
        {
            return mode switch
            {
                1 => "taiko",
                2 => "ctb",
                3 => "mania",
                _ => "std"
            };
        }
        public static int GetPP(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.pp_std,
                1 => stats.pp_taiko,
                2 => stats.pp_ctb,
                3 => stats.pp_mania,
                _ => 0
            };
        }

        public static float GetAccuracy(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.avg_accuracy_std,
                1 => stats.avg_accuracy_taiko,
                2 => stats.avg_accuracy_ctb,
                3 => stats.avg_accuracy_mania,
                _ => 0
            };
        }

        public static int GetRank(IStats stats, int mode, bool isRelax)
        {
            if (isRelax) 
            {
                var rankObject = factory.Get()
                .RelaxStats
                .ToList()
                .OrderByDescending(x => GetPP(x, mode))
                .GroupBy(x => GetPP(x, mode))
                .Select((group, i) => new
                {
                    Rank = i + 1,
                    Players = group.OrderByDescending(x => GetPP(x, mode))
                });
                foreach (var single in rankObject)
                {
                    var player = single.Players.FirstOrDefault(x => x.username == stats.username);
                    if (!(player is null))
                        return single.Rank;
                }
            }
            else
            {
                var rankObject = factory.Get()
                .UsersStats
                .ToList()
                .OrderByDescending(x => GetPP(x, mode))
                .GroupBy(x => GetPP(x, mode))
                .Select((group, i) => new
                {
                    Rank = i + 1,
                    Players = group.OrderByDescending(x => GetPP(x, mode))
                });
                foreach (var single in rankObject)
                {
                    var player = single.Players.FirstOrDefault(x => x.username == stats.username);
                    if (!(player is null))
                        return single.Rank;
                }
            }
            return 0; 
        }

        public static int GetLevel(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.level_std,
                1 => stats.level_taiko,
                2 => stats.level_ctb,
                3 => stats.level_mania,
                _ => 0
            };
        }
        
        public static int GetPlaycount(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.playcount_std,
                1 => stats.playcount_taiko,
                2 => stats.playcount_ctb,
                3 => stats.playcount_mania,
                _ => 0
            };
        }
        public static int GetTotalHits(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.total_hits_std,
                1 => stats.total_hits_taiko,
                2 => stats.total_hits_ctb,
                3 => stats.total_hits_mania,
                _ => 0
            };
        }
        public static long GetTotalScore(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.total_score_std,
                1 => stats.total_score_taiko,
                2 => stats.total_score_ctb,
                3 => stats.total_score_mania,
                _ => 0
            };
        }
        public static long GetRankedScore(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.ranked_score_std,
                1 => stats.ranked_score_taiko,
                2 => stats.ranked_score_ctb,
                3 => stats.ranked_score_mania,
                _ => 0
            };
        }
        public static int GetReplaysWatched(IStats stats, int mode)
        {
            return mode switch
            {
                0 => stats.replays_watched_std,
                1 => stats.replays_watched_taiko,
                2 => stats.replays_watched_ctb,
                3 => stats.replays_watched_mania,
                _ => 0
            };
        }
    }
}