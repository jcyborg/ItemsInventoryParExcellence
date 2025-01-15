using ItemsInventoryParExcellence.DataLayer.ApplicationUsers;
using ItemsInventoryParExcellence.DataLayer.ApplicationUsers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItemsInventoryParExcellence.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly AppUsersContext _context;

        public ItemsController(AppUsersContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            using var stream = new StreamReader(file.OpenReadStream());
            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                var parts = line.Split('|');
                if (parts.Length != 4) continue;

                var item = new Item
                {
                    ItemNo = parts[0],
                    ItemDescription = parts[1],
                    Quantity = int.Parse(parts[2]),
                    Price = decimal.Parse(parts[3])
                };

                if (!_context.Items.Any(i => i.ItemNo == item.ItemNo))
                {
                    _context.Items.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("File uploaded and data imported.");
        }

        [HttpGet]
        public async Task<ActionResult<List<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }
    }
}
