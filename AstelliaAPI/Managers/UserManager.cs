using System;
using System.Collections.Generic;
using System.Linq;
using AstelliaAPI.Controllers;
using AstelliaAPI.Database;
using Newtonsoft.Json;
using ServiceStack.Redis;
using ServiceStack.Text;
using AstelliaAPI.Helpers;

namespace AstelliaAPI.Managers
{
    public class UserManager
    {
        private static readonly AstelliaDbContextFactory factory = new AstelliaDbContextFactory();

        private static readonly PooledRedisClientManager clientsManager = new PooledRedisClientManager();
        private static readonly IRedisPubSubServer redisPubSub = new RedisPubSubServer(clientsManager, "aluri:score")
        {
            OnMessage = (channel, msg) =>
            {
                var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
                var json = (dynamic)JsonConvert.DeserializeObject(msg);
                int id = json.userID;
                var user = context.UsersStats.Where(x => x.id == id).FirstOrDefault();
                if (((Mods)json.score.MODS & Mods.Key4) > 0)
                {
                    var k4pp = GetK4PP(json.userID);
                    user.FourKeyPP = k4pp;
                }
                else if (((Mods)json.score.MODS & Mods.Key7) > 0)
                {
                    var k7pp = GetK7PP(json.userID);
                    user.SevenKeyPP = k7pp;
                }
                context.SaveChanges();
            }
        }.Start();


        public static string GetSafeUsername(string username) => username.Replace(' ', '_').ToLower();

        // packetName is how its called on peppy server packets file
        // packetArgs format: ("hewwo, hewwo|1000") splitted by "|"
        public static void SendPacket(string userId, string packetName, string packetArgs)
        {
            var dict = new Dictionary<string, string>
            {
                ["userID"] = userId,
                ["packetName"] = packetName,
                ["args"] = packetArgs
            };
            Global.Redis.PublishMessage("peppy:packet_sender", JsonConvert.SerializeObject(dict));
        }

        // packetName is how its called on peppy server packets file
        // packetArgs format: ("hewwo, hewwo|1000") splitted by "|"
        public static void SendPacketToEveryone(string packetName, string packetArgs)
        {
            var dict = new Dictionary<string, string>
            {
                ["packetName"] = packetName,
                ["args"] = packetArgs
            };
            Global.Redis.PublishMessage("peppy:packet_sender_to_all", JsonConvert.SerializeObject(dict));
        }

        public static void KickBanchoUser(int userid, string reason)
        {
            var dict = new Dictionary<string, string>
            {
                ["userID"] = userid.ToString(),
                ["reason"] = reason
            };

            Global.Redis.PublishMessage("peppy:disconnect", JsonConvert.SerializeObject(dict));
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
            return ((user.privileges & RipplePrivileges.UserPublic) > 0) && (user.privileges & (RipplePrivileges.UserNormal)) > 0;
        }

        public static bool Restricted(int userid)
        {
            var user = factory.Get().Users.FirstOrDefault(x => x.id == userid);
            if (user is null) return false;
            return (user.privileges & RipplePrivileges.UserNormal) > 0 &&
                   !((user.privileges & RipplePrivileges.UserPublic) > 0) || user.privileges == 0;
        }



        public static bool IsAdmin(int userid)
        {
            var user = factory.Get()
            .Users
            .FirstOrDefault(x => x.id == userid);
            return (user.privileges & RipplePrivileges.AdminManageUsers) > 0;
        }


        public static int GetUserIdByToken(string token)
        {

            var user = factory.Get()
            .Tokens
            .FirstOrDefault(x => x.token == token);


            return user == null ? 0 : user.id;
        }

        public static User GetUserByToken(string token)
        {
            var id = factory.Get()
                .Tokens
                .FirstOrDefault(x => x.token == token).id;

            return factory.Get()
                .Users
                .FirstOrDefault(x => x.id == id); ;
        }



        public static Bills GetBill(string id)
        {
            return factory.Get()
                .Bills
                .FirstOrDefault(x => x.guid == id);
        }

        public static int GetSupporterExpiresAt(int userid)
        {

            var user = factory.Get()
            .Users
            .FirstOrDefault(x => x.id == userid);

            if ((user is null))
                return 1;

            return user.supporter_expires_at;
        }

        public static List<string> GetUsernameKnownAs(int userid)
        {
            //var nicknames = factory.Get().UsernameHistories
            //.OrderByDescending(x => x.changed_datetime)
            //.Where(x => x.userid == userid);


            var s = new List<string>();

            //foreach (var nickname in nicknames)
            //{
            //    s.Add(nickname.username);
            //}

            return s;
        }

        public static bool IsSupporter(int userid)
        {
            var t = GetSupporterExpiresAt(userid);
            if (t == 1)
                return false;

            return t > TimeHelper.CurrentUnixTimestamp();
        }

        public static void ProcessScoreSubmission()
        {
            var clientsManager = new PooledRedisClientManager();
            var redisPubSub = new RedisPubSubServer(clientsManager, "aluri:score")
            {
                OnMessage = async (channel, msg) =>
                {
                    var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
                    var json = (dynamic)JsonConvert.DeserializeObject(msg);
                    int id = json.userID;
                    var user = context.UsersStats.Where(x => x.id == id).FirstOrDefault();
                    if (((Mods)json.score.MODS & Mods.Key4) > 0)
                    {
                        var k4pp = GetK4PP(id);
                        user.FourKeyPP = (int)k4pp;
                    }
                    else if (((Mods)json.score.MODS & Mods.Key7) > 0)
                    {
                        var k7pp = GetK7PP(id);
                        user.SevenKeyPP = (int)k7pp;
                    }
                    await context.SaveChangesAsync();
                }
            }.Start();
        }

        public static double GetK4PP(int userId)
        {
            var scores = factory.Get().Scores.Where(x => x.completed == 3 && x.play_mode == 3 && ((Mods)x.mods & Mods.Key4) > 0 && x.userid == userId).OrderByDescending(x => x.pp);
            double totalPP = 0;

            for (var i = 0; i < 500; i++)
            {
                var score = scores.ToList()[i];
                totalPP += Math.Round(Math.Round(score.pp) * Math.Pow(0.95, i));
            }

            return totalPP;
        }

        public static double GetK7PP(int userId)
        {
            var scores = factory.Get().Scores.Where(x => x.completed == 3 && x.play_mode == 3 && ((Mods)x.mods & Mods.Key7) > 0 && x.userid == userId).OrderByDescending(x => x.pp);
            double totalPP = 0;

            for (var i = 0; i < 500; i++)
            {
                var score = scores.ToList()[i];
                totalPP += Math.Round(Math.Round(score.pp) * Math.Pow(0.95, i));
            }

            return totalPP;
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
        public static int GetKeyRank(IStats user, int k)
        {
            var rankObject = factory.Get()
               .UsersStats
               .ToList()
               .Where(x => !Restricted(x.id))
               .OrderByDescending(x => GetKeyPP(user, k))
               .GroupBy(x => GetKeyPP(user, k))
               .Select((group, i) => new
               {
                   Rank = i + 1,
                   Players = group.OrderByDescending(x => GetKeyPP(user, k))
               });
            foreach (var single in rankObject)
            {
                var player = single.Players.FirstOrDefault(x => x.username == user.username);
                if (!(player is null))
                    return single.Rank;
            }
            return 0;
        }
        public static int GetRank(IStats stats, int mode, bool isRelax)
        {
            if (isRelax)
            {
                var rankObject = factory.Get()
                    .RelaxStats
                    .ToList()
                    .Where(x => !Restricted(x.id))
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
                    .Where(x => !Restricted(x.id))
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

        public static int GetPP(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.pp_std;
            if (mode == 1)
                return stats.pp_taiko;
            if (mode == 2)
                return stats.pp_ctb;
            if (mode == 3)
                return stats.pp_mania;
            return 0;
        }

        public static int GetKeyPP(IStats stats, int k)
        {
            if (k == 4)
                return stats.FourKeyPP;
            else if (k == 7)
                return stats.SevenKeyPP;

            return 0;
        }

        public static float GetAccuracy(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.avg_accuracy_std;
            if (mode == 1)
                return stats.avg_accuracy_taiko;
            if (mode == 2)
                return stats.avg_accuracy_ctb;
            if (mode == 3)
                return stats.avg_accuracy_mania;
            return 0;
        }

        public static int GetLevel(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.level_std;
            if (mode == 1)
                return stats.level_taiko;
            if (mode == 2)
                return stats.level_ctb;
            if (mode == 3)
                return stats.level_mania;
            return 0;
        }

        public static int GetPlaycount(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.playcount_std;
            if (mode == 1)
                return stats.playcount_taiko;
            if (mode == 2)
                return stats.playcount_ctb;
            if (mode == 3)
                return stats.playcount_mania;
            return 0;
        }
        public static int GetTotalHits(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.total_hits_std;
            if (mode == 1)
                return stats.total_hits_taiko;
            if (mode == 2)
                return stats.total_hits_ctb;
            if (mode == 3)
                return stats.total_hits_mania;
            return 0;
        }
        public static long GetTotalScore(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.total_score_std;
            if (mode == 1)
                return stats.total_score_taiko;
            if (mode == 2)
                return stats.total_score_ctb;
            if (mode == 3)
                return stats.total_score_mania;
            return 0;
        }
        public static long GetRankedScore(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.ranked_score_std;
            if (mode == 1)
                return stats.ranked_score_taiko;
            if (mode == 2)
                return stats.ranked_score_ctb;
            if (mode == 3)
                return stats.ranked_score_mania;
            return 0;
        }
        public static int GetReplaysWatched(IStats stats, int mode)
        {
            if (mode == 0)
                return stats.replays_watched_std;
            if (mode == 1)
                return stats.replays_watched_taiko;
            if (mode == 2)
                return stats.replays_watched_ctb;
            if (mode == 3)
                return stats.replays_watched_mania;
            return 0;
        }

        public static string GetCountry(int userid)
        {
            var user = factory.Get()
                .UsersStats
                .FirstOrDefault(x => x.id == userid);
            return user.country;
        }
    }
}