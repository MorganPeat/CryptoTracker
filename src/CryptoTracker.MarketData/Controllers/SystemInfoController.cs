using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CryptoTracker.MarketData.Controllers
{
    /// <summary>
    /// Provides information about the current system
    /// </summary>
    [Route("api/systeminfo")]
    public class SystemInfoController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// DI ctor
        /// </summary>
        public SystemInfoController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets system info for this web api process.
        /// </summary>
        /// <response code="200">System info</response>
        /// <response code="500">Something has gone horribly wrong; cannot get system info</response>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                _env.ApplicationName,
                _env.EnvironmentName,
                _env.ContentRootPath,
                ServerUrls = _configuration[WebHostDefaults.ServerUrlsKey]
            });
        }
    }
}