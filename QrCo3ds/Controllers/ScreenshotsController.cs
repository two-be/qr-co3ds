using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using QrCo3ds.Extensions;
using QrCo3ds.Models;
using QrCo3ds.Utilities;
using Softveloper.IO;

namespace QrCo3ds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScreenshotsController : ControllerBase
    {
        private readonly QrCo3dsContext _context;

        public ScreenshotsController(QrCo3dsContext context) => _context = context;

        [HttpGet("{id}/File")]
        public async Task<ActionResult> GetForFile(int id)
        {
            try
            {
                var screenshot = await _context.Screenshots.FirstOrDefaultAsync(x => x.Id == id);
                if (screenshot == null)
                {
                    return BadRequest(new ExceptionInfo("That screenshot doesn't exist.", $"ScreenshotId: {id}"));
                }

                var fileName = Path.GetFileName(screenshot.LocalPath);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var stream = System.IO.File.OpenRead(screenshot.LocalPath);
                Response.Headers.Add(HeaderNames.ContentDisposition, cd.ToString());
                return File(stream, mimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPost("Game/{gameId}")]
        public async Task<ActionResult<List<ScreenshotInfo>>> Post(int gameId)
        {
            try
            {
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == gameId);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("Please enter a valid game."));
                }

                var directory = Path.Combine(Path.GetDirectoryName(game.CiaLocalPath), "Screenshot");
                Filerectory.CreateDirectory(directory);

                var screenshots = Request.Form.Files.Select(x =>
                {
                    var path = Path.Combine(directory, x.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        x.CopyTo(stream);
                    }
                    var screenshot = new ScreenshotInfo
                    {
                        ContentType = x.ContentType,
                        FileName = x.FileName,
                        GameId = gameId,
                        LocalPath = path,
                    };
                    return screenshot;
                });
                await _context.Screenshots.AddRangeAsync(screenshots);
                await _context.SaveChangesAsync();
                return screenshots.ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var screenshot = await _context.Screenshots.FirstOrDefaultAsync(x => x.Id == id);
                if (screenshot == null)
                {
                    return Ok();
                }
                _context.Screenshots.Remove(screenshot);
                await _context.SaveChangesAsync();
                Filerectory.DeleteFile(screenshot.LocalPath);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
