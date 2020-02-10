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
using Newtonsoft.Json;
using QrCo3ds.Extensions;
using QrCo3ds.Models;
using QrCo3ds.Utilities;
using Softveloper.IO;

namespace QrCo3ds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DlcsController : ControllerBase
    {
        private readonly QrCo3dsContext _context;

        public DlcsController(QrCo3dsContext context) => _context = context;

        [HttpGet("{id}/File")]
        public async Task<ActionResult> GetForFile(int id)
        {
            try
            {
                var dlc = await _context.Dlcs.FirstOrDefaultAsync(x => x.Id == id);
                if (dlc == null)
                {
                    return BadRequest(new ExceptionInfo("That dlc doesn't exist.", $"DlcId: {id}"));
                }

                var fileName = Path.GetFileName(dlc.LocalPath);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var stream = System.IO.File.OpenRead(dlc.LocalPath);
                Response.Headers.Add(HeaderNames.ContentDisposition, cd.ToString());
                return File(stream, mimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DlcInfo>> Post([FromForm]DlcRequest value)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<DlcInfo>(value.Json);
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == data.GameId);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("Please enter a valid game."));
                }

                var folder = game.Name;
                Path.GetInvalidFileNameChars().ToList().ForEach(x =>
                {
                    folder = folder.Replace(x, '-');
                });

                var cia = value.CiaFile;
                var directory = Path.Combine(Paths.Attachment, folder, "Dlc");

                Filerectory.CreateDirectory(directory);

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    data.LocalPath = path;
                }

                var dlc = new DlcInfo
                {
                    ContentType = cia.ContentType,
                    FileName = cia.FileName,
                    GameId = data.GameId,
                    LocalPath = data.LocalPath,
                    Name = data.Name,
                };

                await _context.Dlcs.AddAsync(dlc);
                await _context.SaveChangesAsync();

                return dlc;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DlcInfo>> Put(int id, [FromForm]DlcRequest value)
        {
            try
            {
                var dlc = await _context.Dlcs.FirstOrDefaultAsync(x => x.Id == id);
                if (dlc == null)
                {
                    return BadRequest(new ExceptionInfo("That dlc doesn't exist.", $"DlcId: {id}"));
                }

                var data = JsonConvert.DeserializeObject<DlcInfo>(value.Json);
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == data.GameId);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("Please enter a valid game."));
                }

                var folder = game.Name;
                Path.GetInvalidFileNameChars().ToList().ForEach(x =>
                {
                    folder = folder.Replace(x, '-');
                });

                var cia = value.CiaFile;
                var directory = Path.Combine(Paths.Attachment, folder, "Dlc");

                Filerectory.CreateDirectory(directory);

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    Filerectory.DeleteFile(dlc.LocalPath);
                    dlc.ContentType = cia.ContentType;
                    dlc.FileName = cia.FileName;
                    dlc.LocalPath = path;
                }

                dlc.Name = data.Name;

                await _context.SaveChangesAsync();

                return dlc;
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
                var dlc = await _context.Dlcs.FirstOrDefaultAsync(x => x.Id == id);
                if (dlc == null)
                {
                    return Ok();
                }
                _context.Dlcs.Remove(dlc);
                await _context.SaveChangesAsync();
                Filerectory.DeleteFile(dlc.LocalPath);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
