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

                var fileName = Path.GetFileName(game.BoxArtFile);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var file = await System.IO.File.ReadAllBytesAsync(game.BoxArtFile);
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

                var fileName = Path.GetFileName(game.CiaFile);
                var provider = new FileExtensionContentTypeProvider();
                provider.TryGetContentType(fileName, out var mimeType);
                var cd = new ContentDisposition
                {
                    FileName = fileName,
                    Inline = true,
                };
                var file = await System.IO.File.ReadAllBytesAsync(game.CiaFile);
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
                var boxArt = value.BoxArtFile;
                var cia = value.CiaFile;
                var data = value.Data;
                var directory = Path.Combine(Paths.Attachment, data.Name);
                Filerectory.CreateDirectory(directory);

                if (boxArt != null)
                {

                    var boxArtFile = Path.Combine(directory, boxArt.FileName);
                    using (var stream = System.IO.File.Create(boxArtFile))
                    {
                        await boxArt.CopyToAsync(stream);
                    }
                    data.BoxArtFile = boxArtFile;
                }

                if (cia != null)
                {
                    var ciaFile = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(ciaFile))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    data.CiaFile = ciaFile;
                }

                var game = new GameInfo
                {
                    BoxArtFile = data.BoxArtFile,
                    CiaFile = data.CiaFile,
                    Developer = data.Developer,
                    GameplayUrl = data.GameplayUrl,
                    Name = data.Name,
                    NumberOfPlayers = data.NumberOfPlayers,
                    Publisher = data.Publisher,
                    ReleaseDate = data.ReleaseDate,
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

                var boxArt = value.BoxArtFile;
                var cia = value.CiaFile;
                var data = value.Data;
                var directory = Path.Combine(Paths.Attachment, data.Name);
                Filerectory.CreateDirectory(directory);

                if (boxArt != null)
                {

                    var boxArtFile = Path.Combine(directory, boxArt.FileName);
                    using (var stream = System.IO.File.Create(boxArtFile))
                    {
                        await boxArt.CopyToAsync(stream);
                    }
                    data.BoxArtFile = boxArtFile;
                }

                if (cia != null)
                {
                    var ciaFile = Path.Combine(directory, cia.FileName);
                    using (var stream = System.IO.File.Create(ciaFile))
                    {
                        await cia.CopyToAsync(stream);
                    }
                    data.CiaFile = ciaFile;
                }

                game.BoxArtFile = data.BoxArtFile;
                game.CiaFile = data.CiaFile;
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
                var game = await _context.Games.Include(x => x.Categories).Include(x => x.Screenshots).FirstOrDefaultAsync(x => x.Id == id);
                if (game == null)
                {
                    return Ok();
                }
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
