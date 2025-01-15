using EFCore.BulkExtensions;
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
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var items = new List<Item>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                bool isFirstLine = true;

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    // Skip the header line
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        try
                        {
                            items.Add(new Item
                            {
                                ItemNo = parts[0],
                                ItemDescription = parts[1],
                                Quantity = int.Parse(parts[2]),
                                Price = decimal.Parse(parts[3])
                            });
                        }
                        catch (FormatException)
                        {
                            return BadRequest($"Invalid data format on line: {line}");
                        }
                    }
                    else
                    {
                        return BadRequest($"Invalid line structure: {line}");
                    }
                }
            }

            // Remove duplicates programmatically
            var distinctItems = items
                .GroupBy(i => i.ItemNo) // Group by ItemNo
                .Select(g => g.First()) // Select the first occurrence in each group
                .ToList();

            try
            {
                // Perform bulk insert or update
                await _context.BulkInsertOrUpdateAsync(distinctItems);

                return Ok("File uploaded and processed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }



        [HttpGet("items")]
        public async Task<ActionResult<object>> GetItems(int page = 1, int pageSize = 20)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0.");

            // Calculate total items and pages
            var totalItems = await _context.Items.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Fetch the paginated data
            var items = await _context.Items
                .OrderBy(i => i.ItemNo) // Ensure consistent ordering
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return the paginated data with metadata
            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Data = items
            });
        }


    }
}
