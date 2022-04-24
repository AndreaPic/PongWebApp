using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace PongWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PongController : ControllerBase
    {
        private string MemoryAllocation(int quantity)
        {
            string s = null;
            if (quantity > 0)
            {
                s = String.Empty;
                s = s.PadRight(10);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < quantity * 1000; i++)
                {
                    sb.Append(s);
                }
                s = sb.ToString();
            }
            return s;
        }

        private static int callNumber = 0;
        private IConfigurationRoot ConfigRoot;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public PongController(IHttpClientFactory httpClientFactory, IConfiguration configRoot, ILogger<PongController> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            ConfigRoot = (IConfigurationRoot)configRoot;
        }

        [HttpGet("Pong")]
        public async Task<IActionResult> Pong()
        {
            string ret = string.Empty;

            Interlocked.Increment(ref callNumber);
            _logger.LogInformation($"Pong call number: {callNumber}");

            try
            {
                int mallocQty = ConfigRoot.GetValue<int>("MemoryAllocation", 0);
                var s = MemoryAllocation(mallocQty);

                int delay = ConfigRoot.GetValue<int>("DelayMS", 0);

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }

                string pingBaseAddress = ConfigRoot.GetValue<string>("PingBaseAddress");
                string endpoint = string.Empty;
                if (pingBaseAddress != null)
                {
                    endpoint = $"{pingBaseAddress}api/Ping/Ping";
                }

                var httpRequestMessage = new HttpRequestMessage(
                                    HttpMethod.Get,
                                    endpoint);

                var httpClient = _httpClientFactory.CreateClient();
                int timeout = ConfigRoot.GetValue<int>("TimeoutMS", 0);

                if (timeout > 0)
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                }

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ret = $"Pong Call Number: {callNumber} - Ping Response: " + await httpResponseMessage.Content.ReadAsStringAsync();
                }
                else
                {
                    ret = $"Call To Ping Return Status Code: {httpResponseMessage.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok(ex.Message);
            }

            return Ok(ret);
        }
    }
}
