using System;
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
using System.Web;

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

    public class UserUpdateUsernameBody
    {
        public string password { get; set; }

        public string username { get; set; }
    }
    public class UserRegister
    {
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string captcha_key { get; set; }
    }

    public class Difficulty
    {
        public int id { get; set; }
        public string difficulty_name { get; set; }
        public int ranked { get; set; }
    }


    public class PaymentStatusResponse
    {
        public string status { get; set; }

        public string username { get; set; }
    }

    public class SupporterResponse
    {
        public long expires_at { get; set; }
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


    public class Badge
    {

        public int id { get; set; }


        public string name { get; set; }

        public string url { get; set; }
    }
    public class UserMe
    {
        public int id { get; set; }
        public string username { get; set; }
        public int privileges { get; set; }
        public bool restricted { get; set; }
        public bool banned { get; set; }
        public bool nc_instead_dt { get; set; }

        public string email { get; set; }

        public bool supporter { get; set; }

        public int supporter_expires_at { get; set; }
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

    public class BeatmapResponse
    {
        public int beatmapID { get; set; }
        public int beatmapSetID { get; set; }
        public string mapper { get; set; }
        public string name { get; set; }
        public int AR { get; set; }
        public int OD { get; set; }
        public int HP { get; set; }
        public int CS { get; set; }
        public int time { get; set; }
        public int bpm { get; set; }
        public float stars { get; set; }
        public List<Difficulty> diffs { get; set; }
        public int status { get; set; }
    }

    public class ProfileResponse
    {
        public int id { get; set; }
        public string username { get; set; }
        public int place { get; set; }
        public int level { get; set; }
        public int playcount { get; set; }
        public int pp { get; set; }
        public string country { get; set; }
        public int ss_ranks { get; set; }
        public int s_ranks { get; set; }
        public int a_ranks { get; set; }
        public int four_key_rank { get; set; }
        public int seven_key_rank { get; set; }
        public string userpage_content { get; set; }
        public int account_created_at { get; set; }
        public int replays_watched { get; set; }
        public long ranked_score { get; set; }
        public long total_score { get; set; }
        public int total_hits { get; set; }
        public float accuracy { get; set; }
        public bool is_friend { get; set; }

        public int verification_type { get; set; }

        public int is_supporter { get; set; }

        public List<String> also_known_as { get; set; }
    }

    public class ProfileScore
    {
        public int score_id { get; set; }
        public int beatmap_id { get; set; }
        public int mods { get; set; }
        public float accuracy { get; set; }
        public string beatmap_title { get; set; }
        public string difficulty { get; set; }
        public string timestamp { get; set; }
        public int pp { get; set; }
        public string rank { get; set; }
    }

    public class BeatmapRankRequest
    {
        public int set_id { get; set; }
        public int beatmap_id { get; set; }
        public bool is_set { get; set; }
        public int[] diffs { get; set; }
        public string status { get; set; }
    }

    public class ProfileBeatmap
    {
        public int BeatmapId;
        public int BeatmapSetId;
        public string SongTitle;
        public string Difficulty;
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
            if (!await CheckToken()) return ContentHelper.NoTokenError();
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
            if (!await CheckToken()) return ContentHelper.NoTokenError();
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
            if (!await CheckToken()) return ContentHelper.NoTokenError();
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
            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };

            var token = context.Tokens.FirstOrDefault(x => x.token == Request.Headers["Authorization"]);
            if ((token is null)) return ContentHelper.NoTokenError();

            var user = context.Users.FirstOrDefault(x => x.id == int.Parse(token.user));

            if (!PasswordHelper.IsValidPassword(user.password_md5, passwordNewInfo.currentpassword, user.ssalt, false)) 
                return ContentHelper.Message("Invalid old password", 400);

            var (newpasswd, salt) = PasswordHelper.GeneratePassword(passwordNewInfo.newpassword);

            user.password_md5 = newpasswd;
            user.ssalt = salt;

            if (passwordNewInfo.email != user.email) user.email = passwordNewInfo.email;

            var newtoken = Guid.NewGuid().ToString();
            token.token = newtoken;
            await context.SaveChangesAsync();


            UserManager.KickBanchoUser(int.Parse(token.user), "Password changed, please relog");

            return Ok();
        }

        [HttpPost("update/username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UserUpdateUsernameBody body)
        {
            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };

            var token = context.Tokens.FirstOrDefault(x => x.token == Request.Headers["Authorization"].ToString());

            var user = context.Users.FirstOrDefault(x => x.id == int.Parse(token.user));

            if (!UserManager.IsSupporter(user.id))
                return ContentHelper.Message(":nihuya_unmiy:", 400);

            var testUser = context.Users.FirstOrDefault(x => x.username_safe == UserManager.GetSafeUsername(body.username));

            if (!(testUser is null)) 
                return ContentHelper.Message("This nickname is already used, try another", 400);

            if (!Regex.IsMatch(body.username, @"^[\w ]+$")) return ContentHelper.Message("Not well-formed username", 400);

            if (body.username.Length < 2 || body.username.Length > 16) 
                return ContentHelper.Message("Username is too long or too short!");

            await context.UsernameHistories.AddAsync(new UsernameHistory
            {
                userid = user.id,
                username = user.username,
                changed_datetime = TimeHelper.CurrentUnixTimestamp()
            });

            var usersStats = context.UsersStats.FirstOrDefault(x => x.id == user.id);
            var relaxStats = context.RelaxStats.FirstOrDefault(x => x.id == user.id);

            usersStats.username = user.username;
            relaxStats.username = user.username;

            user.username = body.username;
            user.username_safe = UserManager.GetSafeUsername(body.username);

            UserManager.KickBanchoUser(user.id, $"Nickname changed, new nickname: {body.username}");
            await context.SaveChangesAsync();
            return ContentHelper.Message("Ok");
        }

        [HttpPatch("updateUser/userpage")]
        public async Task<IActionResult> UpdateUserpage([FromBody] UserUpdateUserpage userpageUpdateInfo)
        {
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();

            using var factoryWrite = factory.GetForWrite();
            var user = await factoryWrite.Context.UsersStats.Where(x => x.id.ToString() == dbToken.user)
                .FirstOrDefaultAsync();

            var not_forbidden = true;
            foreach (var line in userpageUpdateInfo.content.Split("\n"))
            {
                if (!Regex.IsMatch(line, @"/\[(\w+)[^w]*?](.*?)\[\/\1]/g")) not_forbidden = false;
            }

            if (!not_forbidden) 
                return ContentHelper.Message("Forbidden userpage content", 400);
            user.userpage_content = userpageUpdateInfo.content;

            return Ok();
        }

        [HttpGet("user/pp_graph")]
        public async Task<IActionResult> GetPPGraph([FromQuery(Name = "id")] int id, [FromQuery(Name = "r")] bool relax)
        {
            var ppGraphs = await factory.Get().PPGraphs.Where(x => x.user == id).ToListAsync();
            return ContentHelper.GenerateOk(ppGraphs);
        }

        [HttpPost("clans/create")]
        public async Task<IActionResult> ClanCreate([FromBody] UserClanCreate userClanInfo)
        {
            string token = Request.Headers["Authorization"];

            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

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
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };

            var user = await factory.Get().Users
                .FirstOrDefaultAsync(x => userLoginInfo.login.ToLower().Replace(" ", "_") == x.username_safe);

            if (user is null)
                return ContentHelper.GenerateError("User not found");

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
                await context.Tokens.AddAsync(tokenObject);
                await context.SaveChangesAsync();
            }

            dbToken = await factory.Get().Tokens.FirstOrDefaultAsync(x => x.user == user.id.ToString());

            var dict = new Dictionary<string, string> { ["token"] = dbToken.token };

            return Content(JsonConvert.SerializeObject(dict));
        }

        [HttpGet("users/whatid")]
        public async Task<IActionResult> WhatId([FromQuery(Name = "u")] string name)
        {
            var user = await factory.Get().Users
                .FirstOrDefaultAsync(x => name.ToLower().Replace(" ", "_") == x.username_safe);


            if (!(user is null) && !UserManager.Restricted(user.id))
                return ContentHelper.GenerateOk(user.id);
            else
                return ContentHelper.GenerateError("User not found.");
        }


        [HttpPost("auth/register")]
        public async Task<IActionResult> UserRegister([FromBody] UserRegister userRegisterInfo)
        {
            var user = await factory.Get().Users
                .FirstOrDefaultAsync(x => userRegisterInfo.login.ToLower().Replace(" ", "_") == x.username_safe);

            if (!(user is null)) return ContentHelper.GenerateError("User with this login already exist.");

            if (!Regex.IsMatch(userRegisterInfo.login, @"^[\w ]+$"))
                return ContentHelper.GenerateError("Login doesn't match format.");

            if (userRegisterInfo.password.Length < 4 || userRegisterInfo.password.Length > 25)
                return ContentHelper.GenerateError("Password too long or too short.");

            if (userRegisterInfo.login.Length < 3 || userRegisterInfo.login.Length > 16)
                return ContentHelper.GenerateError("Username too long or too short.");

            var emailCheck = await factory.Get().Users.FirstOrDefaultAsync(x => x.email == userRegisterInfo.email);
            if (!(emailCheck is null)) return ContentHelper.GenerateError("User with that email already exist.");

            var forbiddenNames = "WhiteCat_mrekk_Vaxei_Micca_RyuK_Alumetri_Freddie Benson_Mathi_FlyingTuna_aetrna_FGSky_Rafis_Karthy_Vaxei 2_obtio_badeu_Lifeline_ASecretBox_a_Blue_Spare_Andros_Varvalian_Dereban_Akolibed_Vamhi_Ralidea_azr8_Paraqeet_mcy4_InBefore_uyghti_Umbre_firebat92_fieryrage_Bartek22830_jpeg_SrChispa_MouseEasy_Maxim Bogdan_Parkes_Gasha_okinamo_Vinno_chocomint_Dustice_DanyL_Weabole_CXu_MAREK MARUCHA_filsdelama";

            if (forbiddenNames.Split("_").Contains(userRegisterInfo.login))
                return ContentHelper.GenerateError("Forbidden name");

            if (!await VerifyCaptcha(userRegisterInfo.captcha_key, userRegisterInfo.ip))
                return ContentHelper.GenerateError("Captcha is not verified.");

            string token = await RegisterUser(userRegisterInfo);

            return ContentHelper.GenerateOk(token);
        }

        [HttpGet("users/password_reset")]
        public async Task<IActionResult> PasswordReset([FromQuery(Name = "u")] int user, [FromQuery(Name = "p")] string password)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };

            string token = Request.Headers["Authorization"];

            if (!await CheckToken())
                return ContentHelper.NoTokenError();

            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();

            if (((RipplePrivileges)dbToken.privileges & RipplePrivileges.AdminManageUsers) > 0)
            {
                string hashPassword;
                byte[] salt = null;
                (hashPassword, salt) = PasswordHelper.GeneratePassword(password);

                var player = await context.Users
                    .FirstOrDefaultAsync(x => x.id == user);

                if (player is null)
                    return ContentHelper.GenerateError("");

                player.password_md5 = hashPassword;
                player.ssalt = salt;

                if (player.password_version != 3)
                    player.password_version = 3;

                await context.SaveChangesAsync();

                return Ok();
            }
            return ContentHelper.GenerateOk("You're not allowed to do that.");
        }

        [HttpGet("beatmaps/info")]
        public async Task<IActionResult> BeatmapInfo([FromQuery(Name = "beatmapid")] int beatmapId)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            var client = new HttpClient();
            var resp = (dynamic)JsonConvert.DeserializeObject(await (await client.GetAsync("https://hentai.ninja/api/b/" + beatmapId)).Content.ReadAsStringAsync());
            var respSet = (dynamic)JsonConvert.DeserializeObject(await (await client.GetAsync("https://hentai.ninja/api/s/" + resp.ParentSetID)).Content.ReadAsStringAsync());

            var diffs = new List<Difficulty>();
            int setId = resp.ParentSetID;
            var beatmaps = context.Beatmaps.Where(x => x.beatmapset_id == setId);
            foreach (var child in respSet.ChildrenBeatmaps)
            {
                int bId = child.BeatmapID;
                var _beatmap = beatmaps.Where(x => x.beatmap_id == bId).FirstOrDefault();
                int ranked = 0;
                if (_beatmap != null)
                {
                    ranked = (int)_beatmap.ranked;
                }
                else
                {
                    ranked = respSet.RankedStatus;
                }
                diffs.Add(new Difficulty
                {
                    id = child.BeatmapID,
                    difficulty_name = child.DiffName,
                    ranked = ranked
                });
            }

            var beatmap = new BeatmapResponse
            {
                beatmapID = resp.BeatmapID,
                beatmapSetID = resp.ParentSetID,
                mapper = respSet.Creator,
                AR = resp.AR,
                OD = resp.OD,
                CS = resp.CS,
                HP = resp.HP,
                bpm = resp.BPM,
                stars = resp.DifficultyRating,
                name = respSet.Artist + " - " + respSet.Title,
                diffs = diffs,
                time = resp.TotalLength
            };

            return ContentHelper.GenerateOkCustom(beatmap);
        }

        [HttpPost("beatmaps/rank")]
        public async Task<IActionResult> BeatmapRank([FromBody] BeatmapRankRequest beatmapRankInfo)
        {
            string token = Request.Headers["Authorization"];

            if (!await CheckToken())
                return ContentHelper.NoTokenError();

            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();

            if (((RipplePrivileges)dbToken.privileges & RipplePrivileges.AdminManageBeatmaps) > 0)
            {
                RankedStatus status = RankedStatus.Unknown;

                switch (beatmapRankInfo.status)
                {
                    case "ranked":
                        status = RankedStatus.Ranked;
                        break;
                    case "loved":
                        status = RankedStatus.Loved;
                        break;
                    case "unranked":
                        status = RankedStatus.LatestPending;
                        break;
                }

                var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
                if (beatmapRankInfo.is_set)
                {
                    var beatmaps = context.Beatmaps.Where(x => x.beatmapset_id == beatmapRankInfo.set_id);
                    foreach (var beatmap in beatmaps)
                    {
                        if (status == RankedStatus.Loved)
                        {
                            var scores = context.Scores.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            var scoresRelax = context.ScoresRelax.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);

                            foreach (var vnScore in scores)
                            {
                                vnScore.pp = 0;
                            }
                            foreach (var rxScore in scores)
                            {
                                rxScore.pp = 0;
                            }
                        }
                        else if (status == RankedStatus.LatestPending)
                        {
                            var scores = context.Scores.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            var scoresRelax = context.ScoresRelax.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            context.Scores.RemoveRange(scores);
                            context.ScoresRelax.RemoveRange(scoresRelax);
                        }
                        beatmap.ranked = status;
                    }
                }
                else
                {
                    foreach (var diff in beatmapRankInfo.diffs)
                    {
                        var beatmap = context.Beatmaps.Where(x => x.beatmap_id == diff).FirstOrDefault();
                        if (status == RankedStatus.Loved)
                        {
                            var scores = context.Scores.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            var scoresRelax = context.ScoresRelax.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);

                            foreach (var vnScore in scores)
                            {
                                vnScore.pp = 0;
                            }
                            foreach (var rxScore in scores)
                            {
                                rxScore.pp = 0;
                            }
                        }
                        else if (status == RankedStatus.LatestPending)
                        {
                            var scores = context.Scores.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            var scoresRelax = context.ScoresRelax.Where(x => x.beatmap_md5 == beatmap.beatmap_md5);
                            context.Scores.RemoveRange(scores);
                            context.ScoresRelax.RemoveRange(scoresRelax);
                        }
                        beatmap.ranked = status;
                    }
                }
                await context.SaveChangesAsync();
            }
            return Ok();
        }

        public async Task<string> RegisterUser(UserRegister userRegisterInfo)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            string password;
            byte[] salt = null;

            (password, salt) = PasswordHelper.GeneratePassword(userRegisterInfo.password);
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
            await context.Users.AddAsync(userObject);
            await context.UsersStats.AddAsync(new UserStats
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
            await context.RelaxStats.AddAsync(new RelaxStats
            {
                username = userRegisterInfo.login,
                favourite_mode = 0,
                country = "XX"
            });
            await context.SaveChangesAsync();
            var tokenString = Guid.NewGuid().ToString();
            var tokenObject = new Token
            {
                token = tokenString,
                user = userObject.id.ToString(),
                privileges = 0
            };
            await context.Tokens.AddAsync(tokenObject);
            await context.SaveChangesAsync();

            return tokenString;
        }

        [HttpGet("getIP")]
        public IActionResult GetIP()
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

            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            var user = await factory.Get().Users.Where(x => x.id.ToString() == dbToken.user).FirstOrDefaultAsync();
            var stats = await factory.Get().UsersStats.Where(x => x.id == user.id).FirstOrDefaultAsync();

            var meResponse = new UserMe
            {
                id = user.id,
                username = user.username,
                privileges = (int)user.privileges,
                banned = UserManager.Allowed(user.id),
                restricted = UserManager.Restricted(user.id),
                email = user.email,
                supporter = UserManager.IsSupporter(user.id),
                supporter_expires_at = user.supporter_expires_at
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

            var meAndStats = JsonConvert.SerializeObject(new object[] { meResponse, userStats });

            return Content(meAndStats);
        }



        [HttpPost("merge")]
        public async Task<IActionResult> PasswordMerge([FromBody] PasswordMergeRequest passwordMergeRequest) // temporary
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            var user = await context.Users
                .Where(x => x.username_safe == passwordMergeRequest.username.ToLower().Replace(" ", "_"))
                .FirstOrDefaultAsync();

            if (user is null)
                return ContentHelper.GenerateError("User not found");

            if (user.password_version > 2)
                return ContentHelper.GenerateError("Account is already on newest password algorithm");

            byte[] salt = null;

            var validPassword =
                BCrypt.Net.BCrypt.Verify(MD5Helper.GetMd5(passwordMergeRequest.password), user.password_md5);

            if (!validPassword) 
                return ContentHelper.GenerateError("Invalid password.");

            string password;
            (password, salt) = PasswordHelper.GeneratePassword(passwordMergeRequest.password);

            user.password_md5 = password;
            user.password_version = 3;
            user.ssalt = salt;

            await context.SaveChangesAsync();

            return ContentHelper.GenerateOk("Merged! You can continue playing on our server <3");
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery(Name = "mode")] int mode,
            [FromQuery(Name = "l")] int limit, [FromQuery(Name = "p")] int page,
            [FromQuery(Name = "relax")] bool isRelax, [FromQuery(Name = "country")] string country = "")
        {
            IEnumerable<LeaderboardResponse> leaderboard = null;

            var p = (page - 1) * limit;

            if (isRelax)
            {
                leaderboard = (await factory.Get()
                    .RelaxStats
                    .ToListAsync())
                    .Where(x => UserManager.GetPP(x, mode) > 0 && !UserManager.Restricted(x.id))
                    .OrderByDescending(x => UserManager.GetPP(x, mode))
                    .Skip(page <= 1 ? 0 : (page - 1) * limit)
                    .Take(limit)
                    .Select(x => new LeaderboardResponse
                    {
                        id = x.id,
                        username = x.username,
                        accuracy = UserManager.GetAccuracy(x, mode),
                        country = x.country,
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        pp = UserManager.GetPP(x, mode),
                        playcount = UserManager.GetPlaycount(x, mode),
                        level = UserManager.GetLevel(x, mode)
                    });
            }
            else
            {
                leaderboard = (await factory.Get()
                    .UsersStats
                    .ToListAsync())
                    .Where(x => UserManager.GetPP(x, mode) > 0 && !UserManager.Restricted(x.id))
                    .OrderByDescending(x => UserManager.GetPP(x, mode))
                    .Skip(page <= 1 ? 0 : (page - 1) * limit)
                    .Take(limit)
                    .Select(x => new LeaderboardResponse
                    {
                        id = x.id,
                        username = x.username,
                        accuracy = UserManager.GetAccuracy(x, mode),
                        country = x.country,
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        pp = UserManager.GetPP(x, mode),
                        playcount = UserManager.GetPlaycount(x, mode),
                        level = UserManager.GetLevel(x, mode)
                    });
            }
            Response.ContentType = "application/json";
            return Content(JsonConvert.SerializeObject(leaderboard));
        }

        [HttpGet("donations/support")]
        public async Task<IActionResult> GetSupporterUrlQiwi([FromQuery(Name = "phone")] string phone, [FromQuery(Name = "token")] string auth)
        {
            var strToken = auth;

            if (!await CheckToken(strToken))
                return ContentHelper.NoTokenError();

            var token = factory.Get()
                .Tokens
                .Where(x => x.token == strToken).FirstOrDefault();

            var user = factory.Get()
                .Users
                .FirstOrDefault(x => x.id == int.Parse(token.user));

            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            var lifetime = TimeHelper.CurrentUnixTimestamp() + 3600;

            var guid = Guid.NewGuid().ToString();
            await context.Bills.AddAsync(new Bills
            {
                phone = phone,
                email = user.email,
                userid = user.id,
                lifetime = lifetime,
                guid = guid,
                state = "WAITING"

            });

            await context.SaveChangesAsync();
            var reddrURL = HttpUtility.UrlEncode($"https://astellia.club/payments/check?id={guid}");
            var encodedLifetime = HttpUtility.UrlEncode(TimeHelper.UnixTimestampToDateTime(lifetime).ToString("YYYY-MM-DD"));
            var comment = HttpUtility.UrlDecode($"Supporter status for {user.username}");
            var publicKey = HttpUtility.UrlDecode(Config.Get().QiwiPublicKey);
            return Redirect(
                $"https://oplata.qiwi.com/create?publicKey={publicKey}&amount={Config.Get().DonorCost}&billId={guid}&successUrl={reddrURL}&email={user.email}&lifetime={encodedLifetime}&comment={comment}&account={user.id}");
        }


        [HttpGet("donations/check")]
        public async Task<IActionResult> CheckPayment([FromQuery(Name = "id")] string paymentID)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };

            var payment = UserManager.GetBill(paymentID);

            if ((payment is null) || payment.state == "DONE")
                return ContentHelper.Message("BILL_NOT_FOUND", 400);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Config.Get().QiwiPrivateKey}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var req = await httpClient.GetAsync($"https://api.qiwi.com/partner/bill/v1/bills/{paymentID}");
            var response = (dynamic)JsonConvert.DeserializeObject(await req.Content.ReadAsStringAsync());
            var user = context.Users
                .FirstOrDefault(x => x.id == payment.userid);

            if (response.status.value == "PAID")
            {
                user.supporter_expires_at = TimeHelper.CurrentUnixTimestamp() + 2592000;

                var bill = context.Bills
                .FirstOrDefault(x => x.guid == paymentID);
                bill.state = "DONE";

                await context.SaveChangesAsync();
            }

            return ContentHelper.GenerateOk(new PaymentStatusResponse
            {
                status = response.status.value,
                username = user.username
            });
        }


        [HttpGet("donations/status")]
        public async Task<IActionResult> GetSupporterStatus()
        {
            string token = Request.Headers["Authorization"];

            if (!await CheckToken()) 
                return ContentHelper.NoTokenError();

            var userid = factory.Get()
                .Tokens
                .Where(x => x.token == token).FirstOrDefault();

            var user = factory.Get()
                .Users
                .FirstOrDefault(x => x.id == Int32.Parse(userid.user));

            if (!(user.supporter_expires_at == 0))
            {
                return ContentHelper.GenerateOkCustom(new SupporterResponse
                {
                    expires_at = user.supporter_expires_at
                });
            }
            else
            {
                return ContentHelper.GenerateOk("NOT_SUPPORTER");
            }
        }

        [HttpPost("users/{id:int}/friend")]
        public async Task<IActionResult> AddFriend(int id)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            string token = Request.Headers["Authorization"];
            if (!await CheckToken()) return ContentHelper.NoTokenError();
            var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
            var user = await factory.Get().Users.Where(x => x.id.ToString() == dbToken.user).FirstOrDefaultAsync();

            await context.Friends.AddAsync(new Friend
            {
                user1 = user.id,
                user2 = id
            });
            await context.SaveChangesAsync();
            return ContentHelper.GenerateOk("ok");
        }

        [HttpGet("profile_info")]
        public async Task<IActionResult> ProfileInfo([FromQuery(Name = "u")] int user, [FromQuery(Name = "m")] int mode, [FromQuery(Name = "r")] bool isRelax)
        {
            string token = Request.Headers["Authorization"];
            User _user = null;

            if (!(token is null))
            {
                if (!await CheckToken())
                    return ContentHelper.NoTokenError();

                var dbToken = await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync();
                _user = await factory.Get().Users.Where(x => x.id.ToString() == dbToken.user).FirstOrDefaultAsync();
            }

            IEnumerable<ProfileResponse> profile = null;
            if (isRelax)
            {
                profile = factory.Get()
                    .RelaxStats
                    .Where(x => x.id == user)
                    .Select(x => new ProfileResponse
                    {
                        id = x.id,
                        username = x.username,
                        place = UserManager.GetRank(x, mode, isRelax),
                        accuracy = UserManager.GetAccuracy(x, mode),
                        country = UserManager.GetCountry(x.id),
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        userpage_content = factory.Get().UsersStats.Where(x => x.id == user).FirstOrDefault().userpage_content,
                        account_created_at = factory.Get().Users.Where(x => x.id == user).FirstOrDefault().register_datetime,
                        pp = UserManager.GetPP(x, mode),
                        playcount = UserManager.GetPlaycount(x, mode),
                        level = UserManager.GetLevel(x, mode),
                        replays_watched = UserManager.GetReplaysWatched(x, mode),
                        ranked_score = UserManager.GetRankedScore(x, mode),
                        total_score = UserManager.GetTotalScore(x, mode),
                        total_hits = UserManager.GetTotalHits(x, mode),
                        is_supporter = UserManager.GetSupporterExpiresAt(user),
                        also_known_as = UserManager.GetUsernameKnownAs(x.id),

                        is_friend = _user == default ? false : factory.Get().Friends.Where(x => x.user1 == _user.id && x.user2 == x.id).FirstOrDefault() == default ? false : true
                    });
            }

            else
            {
                profile = factory.Get()
                    .UsersStats
                    .Where(x => x.id == user)
                    .Select(x => new ProfileResponse
                    {
                        verification_type = x.verification_type,
                        id = x.id,
                        username = x.username,
                        place = UserManager.GetRank(x, mode, isRelax),
                        accuracy = UserManager.GetAccuracy(x, mode),
                        country = x.country,
                        ss_ranks = 0,
                        s_ranks = 0,
                        a_ranks = 0,
                        four_key_rank = UserManager.GetKeyRank(x, 4),
                        seven_key_rank = UserManager.GetKeyRank(x, 7),
                        userpage_content = factory.Get().UsersStats.Where(x => x.id == user).FirstOrDefault().userpage_content,
                        account_created_at = factory.Get().Users.Where(x => x.id == user).FirstOrDefault().register_datetime,
                        pp = UserManager.GetPP(x, mode),
                        playcount = UserManager.GetPlaycount(x, mode),
                        level = UserManager.GetLevel(x, mode),
                        replays_watched = UserManager.GetReplaysWatched(x, mode),
                        ranked_score = UserManager.GetRankedScore(x, mode),
                        total_score = UserManager.GetTotalScore(x, mode),
                        total_hits = UserManager.GetTotalHits(x, mode),
                        is_supporter = UserManager.GetSupporterExpiresAt(user),
                        also_known_as = UserManager.GetUsernameKnownAs(x.id),

                        is_friend = _user == default ? false : factory.Get().Friends.Where(x => x.user1 == _user.id && x.user2 == x.id).FirstOrDefault() == default ? false : true
                    });
            }

            return Content(JsonConvert.SerializeObject(profile));
        }

        [HttpPost("score_submit_processor")]
        public async Task<IActionResult> ScoreSubmitProcessor([FromBody] string body)
        {
            var context = new AstelliaDbContext { Database = { AutoTransactionsEnabled = false } };
            Console.WriteLine(body);
            var json = (dynamic)JsonConvert.DeserializeObject(body);
            int id = json.userID;
            var user = context.UsersStats.Where(x => x.id == id).FirstOrDefault();
            if (((Mods)json.score.MODS & Mods.Key4) > 0)
            {
                var k4pp = UserManager.GetK4PP(json.userID);
                user.FourKeyPP = k4pp;
            }
            else if (((Mods)json.score.MODS & Mods.Key7) > 0)
            {
                var k7pp = UserManager.GetK7PP(json.userID);
                user.SevenKeyPP = k7pp;
            }
            await context.SaveChangesAsync();

            return Ok();

        }
        [HttpGet("user/best")]
        public async Task<IActionResult> BestUserScores([FromQuery(Name = "u")] int user, [FromQuery(Name = "m")] int mode, [FromQuery(Name = "r")] bool isRelax)
        {
            var bestScores = new HashSet<ProfileScore>();
            IOrderedEnumerable<IScore> selfScores;
            if (isRelax)
            {
                selfScores = (await factory.Get().ScoresRelax.Where(x => x.userid == user && x.completed == 3 && x.play_mode == mode)
                                    .ToArrayAsync())
                                    .OrderByDescending(x => x.pp);
            }
            else
            {
                selfScores = (await factory.Get().Scores.Where(x => x.userid == user && x.completed == 3 && x.play_mode == mode)
                                    .ToArrayAsync())
                                    .OrderByDescending(x => x.pp);
            }

            foreach (var selfScore in selfScores)
            {
                var profileBeatmap = await GetBeatmapByMd5(selfScore.beatmap_md5);
                bestScores.Add(new ProfileScore
                {
                    score_id = selfScore.id,
                    beatmap_id = profileBeatmap.BeatmapId,
                    mods = selfScore.mods,
                    accuracy = (float)selfScore.accuracy,
                    beatmap_title = profileBeatmap.SongTitle,
                    difficulty = profileBeatmap.Difficulty,
                    timestamp = TimeHelper.UnixTimestampToDateTime(Convert.ToDouble(selfScore.time)).ToRfc3339String(),
                    pp = (int)selfScore.pp,
                    rank = GetRank(selfScore)
                });
            }
            Response.ContentType = "application/json";
            return Content(JsonConvert.SerializeObject(bestScores));
        }

        public async Task<ProfileBeatmap> GetBeatmapByMd5(string md5)
        {
            var beatmap = await factory.Get().Beatmaps.Where(x => x.beatmap_md5 == md5).FirstOrDefaultAsync();
            var profileBeatmap = new ProfileBeatmap
            {
                BeatmapId = beatmap?.beatmap_id ?? 0,
                BeatmapSetId = beatmap?.beatmapset_id ?? 0,
                SongTitle = beatmap?.song_name.Split('[')[0] ?? "Unknown",
                Difficulty = "[" + beatmap?.song_name.Split('[')[1] ?? "[unknown]"
            };
            return profileBeatmap;
        }

        public async Task<bool> CheckToken()
        {
            string token = Request.Headers["Authorization"];
            return token != null && await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync() != null;
        }

        public async Task<bool> CheckToken(string token) =>
            token != null && await factory.Get().Tokens.Where(x => x.token == token).FirstOrDefaultAsync() != null;

        public string GetRank(IScore score)
        {
            var tHits = score.c50 + score.c100 + score.c300 + score.cMiss;
            var ratio300 = (float)score.c300 / tHits;
            var ratio50 = (float)score.c50 / tHits;
            if (ratio300 == 1)
                return ((Mods)score.mods & Mods.Hidden) > 0 ||
                       ((Mods)score.mods & Mods.Flashlight) > 0
                    ? "SSHD"
                    : "SS";
            if (ratio300 > 0.9 && ratio50 <= 0.01 && score.cMiss == 0)
                return ((Mods)score.mods & Mods.Hidden) > 0 ||
                       ((Mods)score.mods & Mods.Flashlight) > 0
                    ? "SHD"
                    : "S";
            if (ratio300 > 0.8 && score.cMiss == 0 || ratio300 > 0.9)
                return "A";
            if (ratio300 > 0.7 && score.cMiss == 0 || ratio300 > 0.8)
                return "B";
            if (ratio300 > 0.6)
                return "C";
            return "D";
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
    }
}