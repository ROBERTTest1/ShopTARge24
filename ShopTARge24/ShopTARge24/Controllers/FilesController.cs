using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopTARge24.Data;

namespace ShopTARge24.Controllers
{
    [Route("files")]
    public class FilesController : Controller
    {
        private readonly ShopTARge24Context _context;

        public FilesController(ShopTARge24Context context)
        {
            _context = context;
        }

        [HttpGet("api/{id:guid}")]
        public async Task<IActionResult> GetFromDb(Guid id)
        {
            var file = await _context.FileToApis.FirstOrDefaultAsync(x => x.Id == id);
            if (file == null || file.ImageData == null)
            {
                return NotFound();
            }

            var contentType = GetContentType(file.ImageTitle);
            return File(file.ImageData, contentType);
        }

        private static string GetContentType(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "image/jpeg";
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                _ => "image/jpeg"
            };
        }
    }
}


