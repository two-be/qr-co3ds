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
        public string Get(int id)
        {
            return "value";
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
        public async Task<ActionResult<GameInfo>> Post([FromForm]GameInfo value)
        {
            try
            {
                var folder = value.Name;
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
                    value.BoxArtLocalPath = path;
                }

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    value.CiaLocalPath = path;
                }

                var game = new GameInfo
                {
                    BoxArtLocalPath = value.BoxArtLocalPath,
                    Categories = value.Categories.Select(x =>
                    {
                        return new CategoryInfo
                        {
                            Name = x.Name,
                        };
                    }).ToList(),
                    CiaLocalPath = value.CiaLocalPath,
                    Developer = value.Developer,
                    Dlcs = value.Dlcs.Select(x =>
                    {
                        var file = x.CiaFile;
                        var path = Path.Combine(directory, "Dlc", file.FileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            file.CopyTo(stream);
                        }
                        return new DlcInfo
                        {
                            LocalPath = path,
                            Name = x.Name,
                        };
                    }).ToList(),
                    GameplayUrl = value.GameplayUrl,
                    Name = value.Name,
                    NumberOfPlayers = value.NumberOfPlayers,
                    Publisher = value.Publisher,
                    ReleaseDate = value.ReleaseDate,
                    Screenshots = value.Screenshots.Select(x =>
                    {
                        var file = x.ScreenshotFile;
                        var path = Path.Combine(directory, "Screenshot", file.FileName);
                        using (var stream = System.IO.File.Create(path))
                        {
                            file.CopyTo(stream);
                        }
                        return new ScreenshotInfo
                        {
                            ContentType = file.ContentType,
                            FileName = file.FileName,
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
        public async Task<ActionResult<GameInfo>> Put(int id, [FromForm]GameInfo value)
        {
            try
            {
                var game = await _context.Games.FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return BadRequest(new ExceptionInfo("That game doesn't exist.", $"GameId: {id}"));
                }

                var boxArt = value.BoxArtFile;
                var cia = value.CiaFile;

                var folder = value.Name;
                Path.GetInvalidFileNameChars().ToList().ForEach(x =>
                {
                    folder = folder.Replace(x, '-');
                });

                var directory = Path.Combine(Paths.Attachment, folder);
                Filerectory.CreateDirectory(directory);

                if (boxArt != null)
                {

                    var path = Path.Combine(directory, boxArt.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await boxArt.CopyToAsync(stream);
                    }
                    Filerectory.DeleteFile(value.BoxArtLocalPath);
                    value.BoxArtLocalPath = path;
                }

                if (cia != null)
                {
                    var path = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    Filerectory.DeleteFile(value.CiaLocalPath);
                    value.CiaLocalPath = path;
                }

                game.BoxArtFile = value.BoxArtFile;
                game.CiaFile = value.CiaFile;
                game.Developer = value.Developer;
                game.GameplayUrl = value.GameplayUrl;
                game.Name = value.Name;
                game.NumberOfPlayers = value.NumberOfPlayers;
                game.Publisher = value.Publisher;
                game.ReleaseDate = value.ReleaseDate;

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
                var game = await _context.Games.Include(x => x.Categories).Include(x => x.Screenshots).FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return Ok();
                }
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
                Filerectory.DeleteFile(game.BoxArtLocalPath);
                Filerectory.DeleteFile(game.CiaLocalPath);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
