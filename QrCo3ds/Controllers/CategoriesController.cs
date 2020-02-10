using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QrCo3ds.Extensions;
using QrCo3ds.Models;

namespace QrCo3ds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly QrCo3dsContext _context;

        public CategoriesController(QrCo3dsContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<List<CategoryInfo>>> Post([FromBody]List<CategoryInfo> value)
        {
            try
            {
                var categories = value.Where(x => x.Id == 0).Select(x =>
                {
                    var category = new CategoryInfo
                    {
                        GameId = x.GameId,
                        Name = x.Name,
                    };
                    return category;
                });
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
                return categories.ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery]List<int> ids)
        {
            try
            {
                var categories = _context.Categories.Where(x => ids.Contains(x.Id));
                _context.Categories.RemoveRange(categories);
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
