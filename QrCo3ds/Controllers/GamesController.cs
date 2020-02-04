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
    public class GamesController : ControllerBase
    {
        private QrCo3dsContext _context;

        public GamesController(QrCo3dsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<GameInfo>>> Get()
        {
            try
            {
                return await _context.Games.OrderBy(x => x.ReleaseDate).ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameInfo>> Get(int id)
        {
            try
            {
                return await _context.Games.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpGet("{id}/BoxArtFile")]
        public async Task<ActionResult<GameInfo>> GetForBoxArtFile(int id)
        {
            try
            {
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("That game doesn't exist.", $"GameId: {id}"));
                }

                var fileName = Path.GetFileName(game.BoxArtLocalPath);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var file = await System.IO.File.ReadAllBytesAsync(game.BoxArtLocalPath);
                Response.Headers.Add(HeaderNames.ContentDisposition, cd.ToString());
                return File(file, mimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpGet("{id}/CiaFile")]
        public async Task<ActionResult<GameInfo>> GetForCiaFile(int id)
        {
            try
            {
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("That game doesn't exist.", $"GameId: {id}"));
                }

                var fileName = Path.GetFileName(game.CiaLocalPath);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var file = await System.IO.File.ReadAllBytesAsync(game.CiaLocalPath);
                Response.Headers.Add(HeaderNames.ContentDisposition, cd.ToString());
                return File(file, mimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<GameInfo>> Post([FromForm]GameRequest value)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<GameInfo>(value.Json);
                var folder = data.Name;
                Path.GetInvalidFileNameChars().ToList().ForEach(x =>
                {
                    folder = folder.Replace(x, '-');
                });

                var boxArt = value.BoxArtFile;
                var cia = value.CiaFile;
                var directory = Path.Combine(Paths.Attachment, folder);

                Filerectory.CreateDirectory(directory);

                if (boxArt != null)
                {
                    var path = Path.Combine(directory, boxArt.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await boxArt.CopyToAsync(stream);
                    }
                    data.BoxArtLocalPath = path;
                }

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    data.CiaLocalPath = path;
                }

                var game = new GameInfo
                {
                    BoxArtLocalPath = data.BoxArtLocalPath,
                    Categories = data.Categories.Select(x =>
                    {
                        return new CategoryInfo
                        {
                            Name = x.Name,
                        };
                    }).ToList(),
                    CiaLocalPath = data.CiaLocalPath,
                    Developer = data.Developer,
                    GameplayUrl = data.GameplayUrl,
                    Name = data.Name,
                    NumberOfPlayers = data.NumberOfPlayers,
                    Publisher = data.Publisher,
                    ReleaseDate = data.ReleaseDate,
                    Screenshots = value.ScreenshotFiles.Select(x =>
                    {
                        var screenshotDirectory = Path.Combine(directory, "Screenshot");
                        Filerectory.CreateDirectory(screenshotDirectory);

                        var path = Path.Combine(screenshotDirectory, x.FileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            x.CopyTo(stream);
                        }
                        return new ScreenshotInfo
                        {
                            ContentType = x.ContentType,
                            FileName = x.FileName,
                            LocalPath = path,
                        };
                    }).ToList(),
                };

                await _context.Games.AddAsync(game);
                await _context.SaveChangesAsync();

                return game;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPut("{id}")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<GameInfo>> Put(int id, [FromForm]GameRequest value)
        {
            try
            {
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("That game doesn't exist.", $"GameId: {id}"));
                }

                var data = JsonConvert.DeserializeObject<GameInfo>(value.Json);
                var folder = data.Name;
                Path.GetInvalidFileNameChars().ToList().ForEach(x =>
                {
                    folder = folder.Replace(x, '-');
                });

                var boxArt = value.BoxArtFile;
                var cia = value.CiaFile;
                var directory = Path.Combine(Paths.Attachment, folder);

                Filerectory.CreateDirectory(directory);

                if (boxArt != null)
                {

                    var path = Path.Combine(directory, boxArt.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await boxArt.CopyToAsync(stream);
                    }
                    Filerectory.DeleteFile(game.BoxArtLocalPath);
                    data.BoxArtLocalPath = path;
                }

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    Filerectory.DeleteFile(game.CiaLocalPath);
                    data.CiaLocalPath = path;
                }

                game.BoxArtLocalPath = data.BoxArtLocalPath;
                game.CiaLocalPath = data.CiaLocalPath;
                game.Developer = data.Developer;
                game.GameplayUrl = data.GameplayUrl;
                game.Name = data.Name;
                game.NumberOfPlayers = data.NumberOfPlayers;
                game.Publisher = data.Publisher;
                game.ReleaseDate = data.ReleaseDate;

                await _context.SaveChangesAsync();

                return game;
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
                var game = await _context.Games.Include(x => x.Categories).Include(x => x.Dlcs).Include(x => x.Screenshots).FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return Ok();
                }
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
                Filerectory.DeleteFile(game.BoxArtLocalPath);
                Filerectory.DeleteFile(game.CiaLocalPath);
                game.Dlcs.ForEach(x =>
                {
                    Filerectory.DeleteFile(x.LocalPath);
                });
                game.Screenshots.ForEach(x =>
                {
                    Filerectory.DeleteFile(x.LocalPath);
                });
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
