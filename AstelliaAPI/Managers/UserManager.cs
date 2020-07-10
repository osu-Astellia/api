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
    }
}