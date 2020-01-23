using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrCo3ds.Extensions;
using QrCo3ds.Models;
using Softveloper.Extensions;

namespace QrCo3ds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private QrCo3dsContext _context;

        public ValuesController(QrCo3dsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]string value)
        {
            try
            {
                if (value.ToSha512() != "34D266CB3FD8C52B80D37E7E73B436F241C583B9AE5AC36FE2AFD722C322F7EC0E0A1A65A3226C0A4C728CD7BE233C9AC78BEF9B40A9C8972DB82B9BC7CF35C9")
                {
                    return NotFound();
                }
                await _context.Database.MigrateAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id.ToString().ToSha512() != "6AD275D26C200E81534D9996183C8748DDFABC7B0A011A90F46301626D709923474703CACAB0FF8B67CD846B6CB55B23A39B03FBDFB5218EEC3373CF7010A166")
                {
                    return NotFound();
                }
                await _context.Database.EnsureDeletedAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToInfo());
            }
        }
    }
}
