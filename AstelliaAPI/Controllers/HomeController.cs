using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AstelliaAPI.Database;
using AstelliaAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AstelliaAPI.Controllers
{
    public class TopScore
    {
        public float Accuracy;
        public string BeatmapSetId;
        public short Combo;
        public float Performance;
        public string Player;
        public string Rank;
        public string SongAuthor;
        public string SongName;
        public string Time;
        public int UserId;
    }

    public enum Mods
    {
        None = 0,
        NoFail = 1 << 0,
        Easy = 1 << 1,
        TouchDevice  = 1 << 2,
        Hidden = 1 << 3,
        HardRock = 1 << 4,
        SuddenDeath = 1 << 5,
        DoubleTime = 1 << 6,
        Relax = 1 << 7,
        HalfTime = 1 << 8,
        Nightcore = 1 << 9,
        Flashlight = 1 << 10,
        Autoplay = 1 << 11,
        SpunOut = 1 << 12,
        Relax2 = 1 << 13,
        Perfect = 1 << 14,
        Key4 = 1 << 15,
        Key5 = 1 << 16,
        Key6 = 1 << 17,
        Key7 = 1 << 18,
        Key8 = 1 << 19,
        FadeIn = 1 << 20,
        Random = 1 << 21,
        Cinema = 1 << 22,
        Target = 1 << 23,
        Key9 = 1 << 24,
        KeyCoop = 1 << 25,
        Key1 = 1 << 26,
        Key3 = 1 << 27,
        Key2 = 1 << 28,
        LastMod = 1 << 29,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,

        FreeModAllowed = Hidden | HardRock | DoubleTime | Flashlight | FadeIn | Easy | Relax | Relax2 | SpunOut |
                         NoFail | Easy | HalfTime | Autoplay | KeyMod,

        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn | Easy | Relax | Relax2 | SpunOut |
                            NoFail | Easy | HalfTime | Autoplay | SuddenDeath | Perfect | KeyMod | Target | Random |
                            Nightcore | LastMod
    }

    [ApiController]
    [Route("/astellia/api/")]
    [Produces("application/json")]
    public class HomeController : Controller
    {
        private readonly AstelliaDbContextFactory Factory = new AstelliaDbContextFactory();
        private IScore ScoreObject;

        public string GetRank(IScore score)
        {
            var tHits = score.c50 + score.c100 + score.c300 + score.cMiss;
            var ratio300 = (float) score.c300 / tHits;
            var ratio50 = (float) score.c50 / tHits;
            if (ratio300 == 1)
                return ((Mods) score.mods & Mods.Hidden) > 0 ||
                       ((Mods) score.mods & Mods.Flashlight) > 0
                    ? "SSHD"
                    : "SS";
            if (ratio300 > 0.9 && ratio50 <= 0.01 && score.cMiss == 0)
                return ((Mods) score.mods & Mods.Hidden) > 0 ||
                       ((Mods) score.mods & Mods.Flashlight) > 0
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

        [Produces("application/json")]
        [HttpGet("get_top_play")]
        public async Task<IActionResult> GetTopPlay([FromQuery(Name = "s")] int server)
        {
            switch (server)
            {
                case 0:
                    ScoreObject = Factory.Get().Scores.FromSqlRaw(
                            "SELECT score.id, `beatmap_md5`, `userid`, `score`, `max_combo`, `full_combo`,`mods`,`300_count`,`100_count`,`50_count`,`katus_count`,`gekis_count`,`misses_count`,`time`,`play_mode`,`completed`,`accuracy`,`pp` FROM `scores` AS score INNER JOIN `users` _user ON score.userid = _user.id WHERE _user.privileges & 1 AND _user.privileges & 2 AND score.completed = 3 AND play_mode = 0 ORDER BY score.pp DESC LIMIT 1")
                        .ToList()
                        .FirstOrDefault();
                    break;
                case 1:
                    ScoreObject = Factory.Get().ScoresRelax.FromSqlRaw(
                            "SELECT score.id, `beatmap_md5`, `userid`, `score`, `max_combo`, `full_combo`,`mods`,`300_count`,`100_count`,`50_count`,`katus_count`,`gekis_count`,`misses_count`,`time`,`play_mode`,`completed`,`accuracy`,`pp` FROM `scores_relax` AS score INNER JOIN `users` _user ON score.userid = _user.id WHERE _user.privileges & 1 AND _user.privileges & 2 AND score.completed = 3 AND play_mode = 0 ORDER BY score.pp DESC LIMIT 1")
                        .ToList()
                        .FirstOrDefault();
                    break;
                default:
                    Ok("Sorry, it's empty page :)");
                    break;
            }
            var url =
                $"https://osu.ppy.sh/api/get_beatmaps?k={Config.Get().APIKey}&h={ScoreObject.beatmap_md5}&a=1&m=0";

            Console.WriteLine($"URL: {url}");

            Console.WriteLine("Started fetching info...");

            using var webClient = new WebClient();

            var osuBeatmapInfo = (dynamic) JsonConvert.DeserializeObject(webClient.DownloadString(url));
            var songAuthor = osuBeatmapInfo[0].creator;

            Console.WriteLine($"Done... SongAuthor: {songAuthor}");

            Console.WriteLine("Building TopScore object");

            var topScore = new TopScore
            {
                Player = Factory.Get().Users.Where(x => x.id == ScoreObject.userid).Select(x => x.username)
                    .FirstOrDefault(),
                UserId = ScoreObject.userid,
                Performance = ScoreObject.pp,
                Accuracy = (float) Math.Round(ScoreObject.accuracy, 2),
                Combo = (short) ScoreObject.max_combo,
                Rank = GetRank(ScoreObject),
                SongName = Factory.Get().Beatmaps.Where(x => ScoreObject.beatmap_md5 == x.beatmap_md5)
                    .Select(x => x.song_name).FirstOrDefault(),
                SongAuthor = (string) songAuthor,
                Time = TimeHelper.UnixTimestampToDateTime(Convert.ToDouble(ScoreObject.time)).ToRfc3339String(),
                BeatmapSetId = Factory.Get().Beatmaps.Where(x => ScoreObject.beatmap_md5 == x.beatmap_md5)
                    .Select(x => x.beatmapset_id).FirstOrDefault().ToString()
            };
            Console.WriteLine("Done.");
            Console.WriteLine("Rank: " + topScore.Rank);

            Response.ContentType = "application/json";
            Response.StatusCode = 200;

            var json = JsonConvert.SerializeObject(topScore);
            return Content(json);
        }

        [HttpGet("get_news")]
        public async Task<IActionResult> GetNews([FromQuery(Name = "l")] int limit)
        {
            return Ok();
        }

        [HttpGet("get_latest_beatmap")]
        public async Task<IActionResult> GetLatestBeatmap()
        {
            return Ok();
        }
    }
}