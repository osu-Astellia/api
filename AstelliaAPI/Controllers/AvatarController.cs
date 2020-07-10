using System.IO;
using System.Threading.Tasks;
using AstelliaAPI.Database;
using AstelliaAPI.Managers;
using Jdenticon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AstelliaAPI.Controllers
{
    [ApiController]
    [Route("/frontend/api/v1/avatar/{id:int}")]
    public class AvatarController : Controller
    {
        private readonly AstelliaDbContextFactory factory = new AstelliaDbContextFactory();

        public async Task<IActionResult> Index(int id)
        {
            if (!Directory.Exists(Config.Get().AvatarPath))
                Directory.CreateDirectory(Config.Get().AvatarPath);

            if (System.IO.File.Exists(Config.Get().AvatarPath + "/" + id + ".png"))
            {
                var file = await System.IO.File.ReadAllBytesAsync(Config.Get().AvatarPath + "/" + id + ".png");
                return File(file, "image/png");
            }

            var user = await factory.Get().Users.FirstOrDefaultAsync(x => x.id == id);
            
            if (user is null)
                return StatusCode(404);
            
            var subname = user.username.Substring(0, 2);

            await using var avatar = Identicon.FromValue(subname, 64).SaveAsPng();
            await using var ms = new MemoryStream();

            await avatar.CopyToAsync(ms);
            await System.IO.File.WriteAllBytesAsync(Config.Get().AvatarPath + "/" + id + ".png", ms.ToArray());

            UserManager.SendPacketToEveryone("avatarRefresh", id.ToString());

            return File(ms.ToArray(), "image/png");
        }
    }
}