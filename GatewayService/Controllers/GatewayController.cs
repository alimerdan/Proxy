using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly Okta.IJwtValidator _validationService;
        private const string coreServiceURL = "http://coreservice/core/Handle";
        public GatewayController(ILogger<GatewayController> logger, Okta.IJwtValidator validationService)
        {
            _logger = logger;
            _validationService = validationService;
        }

        [HttpGet("Proxy/{target_id}/{*target_endpoint}")]
        public async Task<IActionResult> GetProxied([FromHeader(Name = "Proxy-Authorization")] string proxyAuthorizationHeader, [FromHeader(Name = "Authorization")] string authorizationHeader, string target_id, string target_endpoint)
        {
            //var authToken = this.HttpContext.Request.Headers["Authorization"].ToString();
            var authToken = proxyAuthorizationHeader;

            if (String.IsNullOrEmpty(authToken))
            {
                return Unauthorized();
            }

            var validatedToken = await _validationService.ValidateToken(authToken.Split(" ")[1]);

            if (validatedToken == null)
            {
                return Unauthorized();
            }


            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri($"{coreServiceURL}/{target_id}/{target_endpoint}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationHeader);
            HttpResponseMessage response = httpClient.GetAsync(httpClient.BaseAddress).Result;
            string responseJson = response.Content.ReadAsStringAsync().Result;
            return StatusCode(((int)response.StatusCode), responseJson);
        }
    }
}