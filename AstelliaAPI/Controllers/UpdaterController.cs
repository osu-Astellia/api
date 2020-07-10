using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstelliaAPI.Database;
using AstelliaAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AstelliaAPI.Controllers
{
    [ApiController]
    [Route("/frontend/api/v1/check-updates")]
    [RequestFormLimits(ValueLengthLimit = 1024 * 1024 * 8, ValueCountLimit = 1024 * 1024 * 8)]
    public class UpdaterController : Controller
    {
        private readonly AstelliaDbContextFactory factory = new AstelliaDbContextFactory();

        public async Task<IActionResult> Index([FromQuery(Name = "action")] string action)
        {
            switch (action) // no sense for me to add any other actions
            {
                case "check":
                    if (Request.Method == "GET")
                    {
                        var latestFiles = new List<UpdaterInfo>();
                        var updatableFiles = factory.Get().UpdaterInfo.GroupBy(x => x.filename);
                        foreach (var updatableFile in updatableFiles)
                        {
                            var latestFile = factory.Get().UpdaterInfo.Where(x => x.filename == updatableFile.Key)
                                .OrderByDescending(x => x.file_version).FirstOrDefault();
                            latestFiles.Append(latestFile);
                        }

                        return ContentHelper.GenerateOkCustom(latestFiles);
                    }

                    return StatusCode(405);
                case "put":
                {
                    if (Request.Method != "POST") return StatusCode(405);
                    
                    string ip = Request.Headers["X-Forwarded-For"];
                    if (ip != "185.255.134.174")
                        return ContentHelper.GenerateError("Authentication error.");
                    var files = Request.Form.Files;
                    if (files == null) return ContentHelper.GenerateError("File is null.");
                    
                    var file = files[0];
                    await using var m = new MemoryStream();
                    await file.CopyToAsync(m);
                    m.Position = 0;
                    await using var updatingFile =
                        System.IO.File.Create(Config.Get().UpdaterPath + "/" + file.FileName);

                    m.WriteTo(updatingFile);
                    m.Close();
                    var oldFileVersion = factory.Get().UpdaterInfo
                        .Where(x => x.filename == file.FileName)
                        .OrderByDescending(x => x.file_version)
                        .Select(x => x.file_version)
                        .FirstOrDefault();
                    if (oldFileVersion == default) oldFileVersion = 0;
                    factory.GetForWrite().Context.UpdaterInfo.Add(new UpdaterInfo
                    {
                        filename = file.FileName,
                        file_hash = MD5Helper.GetMd5(Config.Get().UpdaterPath + "/" + file.FileName),
                        file_version = oldFileVersion++,
                        filesize = file.Length,
                        url_full = "https://updater.astellia.club/" + file.FileName
                    });

                    return ContentHelper.GenerateError("File is null.");
                }
            }

            return StatusCode(405);
        }
    }
}