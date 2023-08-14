using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mockTecoAPI.Rooms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace mockTecoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("Basic")]
    public class TecoApiController : ControllerBase
    {
        private Bedroom bedroom = Bedroom.Instance;
        private readonly ILogger<TecoApiController> _logger;
        public TecoApiController(ILogger<TecoApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetList")]
        public IActionResult GetList()
        {
            var jsonResponse = new JObject
            {
                ["glob_bedroom"] = new JObject()
            };

            return OkResult(jsonResponse);
        }

        [HttpGet("SetObject")]
        public IActionResult SetObject()
        {
            string param = Request.Query.FirstOrDefault().Key;
            string value = Request.Query.FirstOrDefault().Value;

            var jsonError = ErrorTemplate();

            if (string.IsNullOrEmpty(param))
            {
                return BadResult(BadParameters(), StatusCodes.Status400BadRequest);
            }
            if (string.IsNullOrEmpty(value))
            {
                return BadResult(ErrorNotFound(param), StatusCodes.Status400BadRequest);
            }
            value.ToLower();

            if (param.StartsWith("glob_bedroom."))
            {
                string[] subParams = param.Split('.');

                if(subParams.Last() == "br_switch_1")
                {
                    if(value == "1" || value == "true")
                        bedroom.br_switch_1 = true;
                }
                //when param exists, TecoAPI always return Ok on setting, even with bad value
                return Ok();
            }
            return BadResult(ErrorNotFound(param), StatusCodes.Status400BadRequest);
        }

        [HttpGet("GetObject")]
        public IActionResult GetObject()
        {
            string param = Request.Query.FirstOrDefault().Key;

            var jsonError = ErrorTemplate();

            if (string.IsNullOrEmpty(param))
            {
                return BadResult(BadParameters(), StatusCodes.Status400BadRequest);
            }

            var jsonResponse = new JObject();
            param.ToLower();

            if (param == "glob_bedroom")
            {
                jsonResponse[param] = JObject.FromObject(bedroom);

                return OkResult(jsonResponse);
            }
            else if (param.StartsWith("glob_bedroom."))
            {
                string[] subParams = param.Split('.');
                JObject subObject = new JObject();

                JToken currentToken = JObject.FromObject(bedroom);
                foreach (string subParam in subParams.Skip(1))
                {
                    if (currentToken is JObject jObject && jObject.TryGetValue(subParam, out JToken subToken))
                    {
                        subObject[subParam] = subToken;
                        currentToken = subToken;
                    }
                    else
                    {
                        return BadResult(ErrorNotFound(param), StatusCodes.Status400BadRequest);
                    }
                }
                jsonResponse[subParams.First()] = subObject;
                return OkResult(jsonResponse);
            }
            return BadResult(ErrorNotFound(param), StatusCodes.Status400BadRequest);
        }
        [Route("{*path}")]
        public IActionResult DefaultRoute()
        {
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
            return new FormattedJsonResult(result, StatusCodes.Status200OK);
        }
        private IActionResult BadResult(Object result, int statusCode)
        {
            JObject subObject = JObject.FromObject(result);
            return new FormattedJsonResult(result, statusCode);
        }

        private ErrorResponse ErrorTemplate()
        {
            var jsonData = new ErrorResponse
            {
                error = new ErrorDetails
                {
                    code = "",
                    message = "",
                    time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };
            return jsonData;
        }

        private ErrorResponse ErrorNotFound(string param)
        {
            var jsonError = ErrorTemplate();
            jsonError.error.code = "400.005";
            jsonError.error.message = $"Variable {param} is not found";
            return jsonError;
        }
        private ErrorResponse BadParameters()
        {
            var jsonError = ErrorTemplate();
            jsonError.error.code = "400.001";
            jsonError.error.message = "Bad parameters (any PLC variable is not specified)";
            return jsonError;
        }
        private class ErrorDetails
        {
            public string code { get; set; }
            public string message { get; set; }
            public string time { get; set; }
        }
        private  class ErrorResponse
        {
            public ErrorDetails error { get; set; }
        }
    }
}