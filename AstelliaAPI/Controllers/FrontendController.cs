﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AstelliaAPI.Database;
using AstelliaAPI.Helpers;
using AstelliaAPI.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AstelliaAPI.Controllers
{
    public class UserUpdate
    {
        public int playstyle { get; set; }
        public CustomBadge custom_badge { get; set; }
        public int favourite_mode { get; set; }
        public bool nc_instead_dt { get; set; }
    }

    public class UserUpdatePassword
    {
        public string email { get; set; }
        public string newpassword { get; set; }
        public string currentpassword { get; set; }
    }

    public class UserUpdateUserpage
    {
        public string content { get; set; }
    }

    public class UserRegister
    {
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string captcha_key { get; set; }
    }

    public class UserClanCreate
    {
        public string name { get; set; }
        public string clan_tag { get; set; }
        public string about { get; set; }
        public string logo { get; set; }
    }

    public class UserLogin
    {
        public string login { get; set; }
        public string password { get; set; }
        public string ip { get; set; }
        public string captcha_key { get; set; }
        public bool is_bancho { get; set; }
    }

    public class UserMe
    {
        public int id { get; set; }
        public string username { get; set; }
        public int privileges { get; set; }
        public bool restricted { get; set; }
        public bool banned { get; set; }
        public bool nc_instead_dt { get; set; }
    }

    public class UserSettings
    {
        public int playstyle { get; set; }
        public CustomBadge custom_badge { get; set; }
        public int favourite_mode { get; set; }
        public bool nc_instead_dt { get; set; }
    }

    public class CustomBadge
    {
        public bool enabled { get; set; }
        public string icon { get; set; }
        public string title { get; set; }
    }

    public class PPGraph
    {
        public int user { get; set; }
        public string time { get; set; }
        public int pp { get; set; }
        public int is_relax { get; set; }
        public int gamemode { get; set; }
    }

    public class CaptchaResponse
    {
        public bool success { get; set; }
        public string timestamp { get; set; }
        public string host { get; set; }
    }

    public class LeaderboardResponse
    {
        public int id { get; set; }
        public string username { get; set; }
        public float level { get; set; }
        public int playcount { get; set; }
        public int pp { get; set; }
        public string country { get; set; }
        public int ss_ranks { get; set; }
        public int s_ranks { get; set; }
        public int a_ranks { get; set; }
        public float accuracy { get; set; }
    }

    public class PasswordMergeRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string type { get; set; }
    }

    public class PasswordMergeResponse
    {
        public string password_bcrypt { get; set; }
        public int user_id { get; set; }
    }


    [ApiController]
    [Route("/frontend/api/v1/")]
    [RequestFormLimits(ValueLengthLimit = 1024 * 1024 * 8, ValueCountLimit = 1024 * 1024 * 8)]
    public class FrontendController : Controller
    {
        private readonly AstelliaDbContextFactory factory = new AstelliaDbContextFactory();

        [HttpPatch("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdate userNewStats)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            using var factoryWrite = factory.GetForWrite();
            var stats = await factoryWrite.Context.UsersStats.Where(x => x.id.ToString() == dbToken.user)
                .FirstOrDefaultAsync();
            var user = await factoryWrite.Context.Users.FirstOrDefaultAsync(x => x.id.ToString() == dbToken.user);

            stats.play_style = userNewStats.playstyle;
            stats.custom_badge_icon = userNewStats.custom_badge.icon;
            stats.custom_badge_name = userNewStats.custom_badge.title;
            stats.show_custom_badge = userNewStats.custom_badge.enabled;
            stats.favourite_mode = userNewStats.favourite_mode;
            user.nc_instead_dt = userNewStats.nc_instead_dt;

            return Ok();
        }

        [HttpGet("sendPacket")]
        public async Task<IActionResult> SendPacket([FromQuery(Name = "u")] string userId,
            [FromQuery(Name = "p")] string packetName, [FromQuery(Name = "a")] string packetArgs)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.FirstOrDefaultAsync(x => x.token == token);
            var user = await factory.Get().Users.FirstOrDefaultAsync(x => x.id.ToString() == dbToken.user);

            if ((user.privileges & RipplePrivileges.AdminManageUsers) <= 0)
                return ContentHelper.GenerateOk("No, sowwy, that's my pwace ~ c OwO c ~");
            
            UserManager.SendPacket(userId, packetName, packetArgs);
            return ContentHelper.GenerateOk("Sent!");

        }

        [HttpPost("updateUser/avatar")]
        public async Task<IActionResult> UpdateUserAvatar()
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            var avatar = Request.Form.Files.GetFile("File");
            if (avatar is null) ContentHelper.GenerateError("File is invalid.");
            if (!Regex.IsMatch(avatar.FileName, @"([^\s]+(\.(?i)(jpg|png|jpeg))$)"))
                return ContentHelper.GenerateError("File is not an image.");
            await using (var m = new MemoryStream())
            {
                await avatar.CopyToAsync(m);
                m.Position = 0;
                await using var avatarFile =
                    System.IO.File.Create(Config.Get().AvatarPath + "/" + $"{dbToken.user}.png");
                m.WriteTo(avatarFile);
                m.Close();
                avatarFile.Close();
            }

            UserManager.SendPacketToEveryone("avatarRefresh", dbToken.user);

            return Ok();
        }

        [HttpPatch("updateUser/password")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserUpdatePassword passwordNewInfo)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            using var factoryWrite = factory.GetForWrite();
            var user = await factoryWrite.Context.Users.Where(x => x.id.ToString() == dbToken.user)
                .FirstOrDefaultAsync();

            user.password_md5 = passwordNewInfo.newpassword;
            user.email = passwordNewInfo.email;

            return Ok();
        }

        [HttpPatch("updateUser/userpage")]
        public async Task<IActionResult> UpdateUserpage([FromBody] UserUpdateUserpage userpageUpdateInfo)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            using var factoryWrite = factory.GetForWrite();
            var user = await factoryWrite.Context.UsersStats.Where(x => x.id.ToString() == dbToken.user)
                .FirstOrDefaultAsync();

            user.userpage_content = userpageUpdateInfo.content;
            return Ok();
        }

        [HttpGet("user/pp_graph")]
        public async Task<IActionResult> GetPPGraph([FromQuery(Name = "id")] int id, [FromQuery(Name = "r")] bool relax)
        {
            var ppGraphs = factory.Get().PPGraphs.Where(x => x.user == id).ToList();
            return ContentHelper.GenerateOk(ppGraphs);
        }

        [HttpPost("clans/create")]
        public async Task<IActionResult> ClanCreate([FromBody] UserClanCreate userClanInfo)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            using var factoryWrite = factory.GetForWrite();
            var user = await factory.Get().Users.Where(x => x.id.ToString() == dbToken.user).FirstOrDefaultAsync();

            var clan = await factoryWrite.Context.Clans.AddAsync(new Clan
            {
                name = userClanInfo.name,
                description = userClanInfo.about,
                background = "",
                icon = userClanInfo.logo,
                tag = userClanInfo.clan_tag
            });

            await factoryWrite.Context.UserClans.AddAsync(new UserClan
            {
                clan = clan.Entity.id,
                perms = 8, // owner permission
                user = user.id
            });

            return ContentHelper.GenerateOk("Clan created!");
        }

        [HttpPost("auth/login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLogin userLoginInfo)
        {
            var user = await factory.Get().Users
                .FirstOrDefaultAsync(x => userLoginInfo.login.ToLower().Replace(" ", "_") == x.username_safe);
            if (user is null) return ContentHelper.GenerateError("User not found");
            if (user.password_version < 3)
                return ContentHelper.GenerateError("Please merge password to new algorithm.");
            if (!PasswordHelper.IsValidPassword(user.password_md5, userLoginInfo.password, user.ssalt,
                userLoginInfo.is_bancho)) return ContentHelper.GenerateError("Invalid Password");
            if (!userLoginInfo.is_bancho)
                if (!await VerifyCaptcha(userLoginInfo.captcha_key, userLoginInfo.ip))
                    return ContentHelper.GenerateError("Captcha is not verified.");

            var dbToken = await factory.Get().Tokens.FirstOrDefaultAsync(x => x.user == user.id.ToString());
            if (dbToken is null)
            {
                var tokenObject = new Token
                {
                    token = Guid.NewGuid().ToString(),
                    user = user.id.ToString(),
                    privileges = 0
                };
                var factoryWrite = factory.GetForWrite();
                await factoryWrite.Context.Tokens.AddAsync(tokenObject);
                await factoryWrite.Context.SaveChangesAsync();
            }

            dbToken = await factory.Get().Tokens.FirstOrDefaultAsync(x => x.user == user.id.ToString());
            var dict = new Dictionary<string, string> { ["token"] = dbToken.token };


            return Content(JsonConvert.SerializeObject(dict));
        }

        [HttpPost("auth/register")]
        public async Task<IActionResult> UserRegister([FromBody] UserRegister userRegisterInfo)
        {
            var user = await factory.Get().Users
                .FirstOrDefaultAsync(x => userRegisterInfo.login.ToLower().Replace(" ", "_") == x.username_safe);
            
            if (!(user is null)) return ContentHelper.GenerateError("User with this login already exist.");
            
            if (userRegisterInfo.password.Length < 4 || userRegisterInfo.password.Length > 25)
                return ContentHelper.GenerateError("Password too long or too short.");
            
            if (userRegisterInfo.login.Length < 3 || userRegisterInfo.login.Length > 16)
                return ContentHelper.GenerateError("Username too long or too short.");
            
            var emailCheck = await factory.Get().Users.FirstOrDefaultAsync(x => x.email == userRegisterInfo.email);
            if (!(emailCheck is null)) return ContentHelper.GenerateError("User with that email already exist.");
            
            if (!await VerifyCaptcha(userRegisterInfo.captcha_key, userRegisterInfo.ip))
                return ContentHelper.GenerateError("Captcha is not verified.");
            
            string password;
            byte[] salt = null;

            (password, salt) = PasswordHelper.GeneratePassword(userRegisterInfo.password);
            using var factoryWrite = factory.GetForWrite();
            var userObject = new User
            {
                username = userRegisterInfo.login,
                username_safe = userRegisterInfo.login.ToLower().Replace(" ", "_"),
                email = userRegisterInfo.email,
                password_md5 = password,
                privileges = RipplePrivileges.UserPendingVerification,
                register_datetime = TimeHelper.CurrentUnixTimestamp(),
                ssalt = salt,
                password_version = 3
            };
            await factoryWrite.Context.Users.AddAsync(userObject);
            await factoryWrite.Context.UsersStats.AddAsync(new UserStats
            {
                username = userRegisterInfo.login,
                custom_badge_icon = "",
                show_custom_badge = false,
                custom_badge_name = "",
                favourite_mode = 0,
                userpage_content = "",
                play_style = 0,
                country = "XX"
            });
            await factoryWrite.Context.RelaxStats.AddAsync(new RelaxStats
            {
                username = userRegisterInfo.login,
                favourite_mode = 0,
                country = "XX"
            });
            await factoryWrite.Context.SaveChangesAsync();
            var tokenObject = new Token
            {
                token = Guid.NewGuid().ToString(),
                user = userObject.id.ToString(),
                privileges = 0
            };
            await factoryWrite.Context.Tokens.AddAsync(tokenObject);
            await factoryWrite.Context.SaveChangesAsync();
            return ContentHelper.GenerateOk(tokenObject.token);
        }

        [HttpGet("getIP")]
        public async Task<IActionResult> GetIP()
        {
            var dict = new Dictionary<string, string>();
            string ip = Request.Headers["X-Forwarded-For"];
            dict["ip"] = ip.Split(',')[0];
            return Content(JsonConvert.SerializeObject(dict));
        }

        [HttpGet("user/@me")]
        public async Task<IActionResult> Me()
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.GenerateError("Token not found");
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            var user = await factory.Get().Users.Where(x => x.id.ToString() == dbToken.user).FirstOrDefaultAsync();
            var stats = await factory.Get().UsersStats.Where(x => x.id == user.id).FirstOrDefaultAsync();

            var meResponse = new UserMe
            {
                id = user.id,
                username = user.username,
                privileges = (int) user.privileges,
                banned = UserManager.Allowed(user.id),
                restricted = UserManager.Restricted(user.id)
            };
            var userStats = new UserSettings
            {
                playstyle = stats.play_style,
                custom_badge = new CustomBadge
                {
                    icon = stats.custom_badge_icon,
                    title = stats.custom_badge_name,
                    enabled = stats.show_custom_badge
                },
                favourite_mode = stats.favourite_mode,
                nc_instead_dt = user.nc_instead_dt
            };

            var meAndStats = JsonConvert.SerializeObject(new object[] {meResponse, userStats});

            return Content(meAndStats);
        }

        [HttpPost("merge")]
        public async Task<IActionResult>
            PasswordMerge([FromBody] PasswordMergeRequest passwordMergeRequest) // temporary
        {
            using var factoryWrite = factory.GetForWrite();
            var user = await factoryWrite.Context.Users
                .Where(x => x.username_safe == passwordMergeRequest.username.ToLower().Replace(" ", "_"))
                .FirstOrDefaultAsync();
            if (user is null) return ContentHelper.GenerateError("User not found");

            if (user.password_version > 2)
                return ContentHelper.GenerateError("Account is already on newest password algorithm");
            
            byte[] salt = null;
            var validPassword =
                BCrypt.Net.BCrypt.Verify(MD5Helper.GetMd5(passwordMergeRequest.password), user.password_md5);

            if (!validPassword) return ContentHelper.GenerateError("Invalid password.");
            
            string password;
            (password, salt) = PasswordHelper.GeneratePassword(passwordMergeRequest.password);

            user.password_md5 = password;
            user.password_version = 3;
            user.ssalt = salt;

            return ContentHelper.GenerateOk("Merged! You can continue playing on our server <3");
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery(Name = "mode")] int mode,
            [FromQuery(Name = "l")] int limit, [FromQuery(Name = "p")] int page,
            [FromQuery(Name = "relax")] bool isRelax, [FromQuery(Name = "country")] string country = "")
        {
            IEnumerable<LeaderboardResponse> leaderboard = null;
            if (isRelax)
                leaderboard = factory.Get()
                    .RelaxStats
                    .ToList()
                    .Where(x => GetPP(x, mode) > 0)
                    .OrderByDescending(x => GetPP(x, mode))
                    .Skip(page == 1 ? 0 : page * limit)
                    .Take(limit)
                    .Select(x => new LeaderboardResponse
                    {
                        id = x.id,
                        username = x.username,
                        accuracy = GetAccuracy(x, mode),
                        country = x.country,
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        pp = GetPP(x, mode),
                        playcount = GetPlaycount(x, mode),
                        level = GetLevel(x, mode)
                    });
            else
                leaderboard = factory.Get()
                    .UsersStats
                    .ToList()
                    .Where(x => GetPP(x, mode) > 0)
                    .OrderByDescending(x => GetPP(x, mode))
                    .Skip(page == 1 ? 0 : page * limit)
                    .Take(limit)
                    .Select(x => new LeaderboardResponse
                    {
                        id = x.id,
                        username = x.username,
                        accuracy = GetAccuracy(x, mode),
                        country = x.country,
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        pp = GetPP(x, mode),
                        playcount = GetPlaycount(x, mode),
                        level = GetLevel(x, mode)
                    });
            Response.ContentType = "application/json";
            return Content(JsonConvert.SerializeObject(leaderboard));
        }

        public async Task<bool> CheckToken()
        {
            string token = Request.Headers["Authorization"];
            return token != null && await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync() != null;
        }

        public async Task<bool> VerifyCaptcha(string key, string userIp)
        {
            if (key == Config.Get().RecaptchaPrivate)
                return true;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", Config.Get().RecaptchaPrivate),
                new KeyValuePair<string, string>("response", key),
                new KeyValuePair<string, string>("remoteip", userIp)
            });
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

            var responseObject =
                JsonConvert.DeserializeObject<CaptchaResponse>(await response.Content.ReadAsStringAsync());
            return responseObject.success;
        }

        public int GetPP(IStats stats, int mode)
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

        public float GetAccuracy(IStats stats, int mode)
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
        
        public int GetLevel(IStats stats, int mode)
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
        
        public int GetPlaycount(IStats stats, int mode)
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
    }
}