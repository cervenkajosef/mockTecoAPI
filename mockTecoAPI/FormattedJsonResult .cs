using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mockTecoAPI
{
    public class FormattedJsonResult : ActionResult
    {
        private readonly object _data;
        private readonly int _statusCode;

        public FormattedJsonResult(object data, int statusCode)
        {
            this._data = data;
            this._statusCode = statusCode;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var httpResponse = context.HttpContext.Response;
            
            httpResponse.Headers.Clear();
            httpResponse.StatusCode = _statusCode;
            httpResponse.Headers.Add("Server", "TecoApi/1.0.1 (F2x CP2007I v2.3.058 N8 0105)");
            httpResponse.ContentType = "application/json";
            httpResponse.Headers.Add("Cache-Control", "no-cache");
            
            var formattedJson = _data == null ? "" : JsonConvert.SerializeObject(_data, Formatting.Indented);

            httpResponse.Headers.Add("Content-Length", formattedJson.Length.ToString());

            await httpResponse.WriteAsync(formattedJson);
        }
    }
}
