using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoreController : ControllerBase
    {

        private readonly ILogger<CoreController> _logger;
        private const string targetResolverServiceURL = "http://targetresolverservice/targetresolver/ResolveTarget";
        public CoreController(ILogger<CoreController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Handle/{target_id}/{*target_endpoint}")]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string authorizationHeader, string target_id, string target_endpoint)
        {
            HttpResponseMessage targetResponse = await GetTargetURL(target_id);
            string targetJson = targetResponse.Content.ReadAsStringAsync().Result;
            if (targetResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var targetUrl = JObject.Parse(targetJson)["data"];
                HttpResponseMessage response = ForwardMessage($"{targetUrl}/{target_endpoint}", new Dictionary<string, string>()).GetAwaiter().GetResult();
                string responseJson = response.Content.ReadAsStringAsync().Result;
                return StatusCode(((int)response.StatusCode), responseJson);

            }
            else
            {
                return StatusCode(((int)targetResponse.StatusCode), targetJson);
            }

        }

        private async Task<HttpResponseMessage> GetTargetURL(string target_id)
        {

            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri($"{targetResolverServiceURL}/{target_id}");
            HttpResponseMessage response = httpClient.GetAsync(httpClient.BaseAddress).Result;
            return response;
        }

        private static async Task<HttpResponseMessage> ForwardMessage(string baseUrl, Dictionary<string, string> headers)
        {
            HttpClient httpClient = new();
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUrl),
            };
            foreach (KeyValuePair<string, string> header in headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            return response;
        }
    }
}