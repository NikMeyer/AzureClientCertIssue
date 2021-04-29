namespace Sample.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;

    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///   Method without authentication to test if headers are set correctly.
        /// </summary>
        /// <returns>A list of HTTP headers and other HTTP header settings.</returns>
        [HttpGet("info")]
        [Produces("application/json")]
        public IActionResult GetInfo()
        {
            var headers = new JObject();
            var tls = HttpContext.Features.Get<ITlsConnectionFeature>();

            headers.Add(new JProperty("IsHTTPS", HttpContext.Request.IsHttps));

            headers.Add(new JProperty("TLSType", tls?.ToString()));
            headers.Add(new JProperty("Cert", tls?.ClientCertificate?.ToString()));

            foreach (var header in HttpContext.Request.Headers)
            {
                headers.Add(new JProperty(header.Key, header.Value.ToString()));
            }

            foreach (var item in HttpContext.Items)
            {
                headers.Add(new JProperty(item.Key.ToString(), item.Value != null ? item.Value.ToString() : string.Empty));
            }

            return Ok(headers);
        }

        /// <summary>
        ///   An endpoint using client cert authentication.
        /// </summary>
        /// <returns>Http code.</returns>
        [HttpGet("auth")]
        [Authorize]
        public IActionResult GetWithAuth()
        {
            return Ok();
        }
    }
}
