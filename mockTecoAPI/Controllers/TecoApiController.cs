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
        private TecoApi tecoApi;

        public TecoApiController(ILogger<TecoApiController> logger)
        {
            _logger = logger;
            tecoApi = new(logger);
        }

        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            try
            {
                _logger.LogInformation($"Session ID [{_requestId}]\nGetList method called.");
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
                var allParams = Request.Query.ToArray();
                var paramsString = string.Join("&", allParams.Select(param => $"{param.Key}={param.Value}"));
                _logger.LogInformation($"Session ID [{_requestId}]\nSetObject method called with param=value: {paramsString}");

                Result result = null;
                foreach (var param in allParams)
                {
                    result = tecoApi.GetObject(param.Key);
                    if (result.status != StatusCodes.Status200OK)
                    {
                        return BadResult(result.result, result.status);
                    }
                }

                foreach (var param in allParams)
                {
                    if (param.Value == "")
                    {
                        var errors = new Errors();
                        return BadResult(errors.ErrorNotFound(param.Key), StatusCodes.Status400BadRequest);
                    }
                    result = tecoApi.SetObject(param.Key, param.Value);
                }

                if (result != null)
                {
                    return OkResult(result.result);
                }

                throw new Exception("SetObject result is null");
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
                var allParams = Request.Query.ToArray();
                var paramsString = string.Join("&", allParams.Select(param => $"{param.Key}"));
                _logger.LogInformation($"Session ID [{_requestId}]\nGetObject method called with param: {paramsString}");
                JArray jsonArray = new JArray();
                
                foreach (var param in allParams)
                {
                    var result = tecoApi.GetObject(param.Key);
                    if (result.status == StatusCodes.Status200OK)
                    {
                        jsonArray.Add(result.result);
                    }
                    else
                    {
                        return BadResult(result.result, result.status);
                    }
                }
                var jsonObject = ConvertJArrayToJObject(jsonArray);
                return OkResult(jsonObject);
                
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

        private static JObject ConvertJArrayToJObject(JArray jsonArray)
        {
            var outputObject = new JObject();

            foreach (var jToken in jsonArray)
            {
                var item = (JObject)jToken;
                foreach (var property in item.Properties())
                {
                    var key = property.Name;
                    var subObject = (JObject)property.Value;

                    if (!outputObject.ContainsKey(key))
                    {
                        outputObject[key] = new JObject();
                    }

                    foreach (var subProperty in subObject.Properties())
                    {
                        outputObject[key]![subProperty.Name] = subProperty.Value;
                    }
                }
            }

            return outputObject;
        }
    }
}