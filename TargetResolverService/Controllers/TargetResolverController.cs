using Microsoft.AspNetCore.Mvc;

namespace TargetResolverService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TargetResolverController : ControllerBase
    {
        private static readonly Dictionary<string, string> lookup = new() { { "g", "https://google.com" }, { "b", "https://bing.com" } };

        private readonly ILogger<TargetResolverController> _logger;

        public TargetResolverController(ILogger<TargetResolverController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ResolveTarget/{target_id}")]
        public async Task<IActionResult> Get(string target_id)
        {
            try
            {
                if (string.IsNullOrEmpty(target_id))
                {
                    return BadRequest(new { error = "{target_id} cannot be empty" });
                }
                string res;
                if (lookup.TryGetValue(target_id, out res))
                {
                    return Ok(new { data = res });

                }
                else
                {
                    return NotFound(new { error = $"{target_id} is not a valid target, or not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}