using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly Okta.IJwtValidator _validationService;

        public GatewayController(ILogger<GatewayController> logger, Okta.IJwtValidator validationService)
        {
            _logger = logger;
            _validationService = validationService;
        }

        [HttpGet("proxy/{target_id}/{*target_endpoint}")]
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


            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://dev-88454646.okta.com/oauth2/ausf2z5fpnJX9G6VR5d7/.well-known/openid-configuration");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationHeader);
            HttpResponseMessage response = httpClient.GetAsync(httpClient.BaseAddress).Result;
            return new JsonResult(response);
        }
    }
}