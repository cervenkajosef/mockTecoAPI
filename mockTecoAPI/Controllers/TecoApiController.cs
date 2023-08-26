using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mockTecoAPI.Models.Error;
using mockTecoAPI.Models.TecoApi;
using Newtonsoft.Json.Linq;

namespace mockTecoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("Basic")]
    public class TecoApiController : ControllerBase
    {
        private readonly ILogger<TecoApiController> _logger;
        string _requestId = Guid.NewGuid().ToString();

        public TecoApiController(ILogger<TecoApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            try
            {
                _logger.LogInformation($"Session ID [{_requestId}]\nGetList method called.");
                TecoApi tecoApi = new TecoApi();
                var result = tecoApi.GetList();

                return OkResult(result.result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetList method.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("SetObject")]
        public IActionResult SetObject()
        {
            try
            {
                string param = Request.Query.FirstOrDefault().Key;
                string value = Request.Query.FirstOrDefault().Value;
                _logger.LogInformation($"Session ID [{_requestId}]\nSetObject method called with param=value: {param}={value}");

                TecoApi tecoApi = new TecoApi();
                var result = tecoApi.SetObject(param, value);

                if (result.status == StatusCodes.Status200OK)
                {
                    return OkResult(result.result);
                }
                return BadResult(result.result, result.status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SetObject method.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetObject")]
        public IActionResult GetObject()
        {
            try
            {
                string param = Request.Query.FirstOrDefault().Key;
                _logger.LogInformation($"Session ID [{_requestId}]\nGetObject method called with param: {param}");

                TecoApi tecoApi = new TecoApi();
                var result = tecoApi.GetObject(param);
                if (result.status == StatusCodes.Status200OK)
                {
                    return OkResult(result.result);
                }
               
                return BadResult(result.result, result.status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetObject method.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("{*path}")]
        public IActionResult DefaultRoute()
        {
            _logger.LogInformation($"Session ID [{_requestId}]\nDefaultRoute method called.");
            string param = Request.Query.FirstOrDefault().Key;
            var jsonData = new ErrorResponse
            {
                error = new ErrorDetails
                {
                    code = "501.013",
                    message = $"Unimplemented service 'HTTP GET | {param}'",
                    time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };
            return BadResult(jsonData, StatusCodes.Status501NotImplemented);
        }

        private IActionResult OkResult(JObject result)
        {
            _logger.LogInformation($"Method succeeded and returned:\n{result}");
            return new FormattedJsonResult(result, StatusCodes.Status200OK);
        }
        private IActionResult BadResult(Object result, int statusCode)
        {
            JObject subObject = JObject.FromObject(result);
            _logger.LogWarning($"GetObject method failed and returned:\n{subObject}");
            return new FormattedJsonResult(result, statusCode);
        }
    }
}