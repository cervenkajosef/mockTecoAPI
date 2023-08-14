using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mockTecoAPI
{
    public class FormattedJsonResult : ActionResult
    {
        private readonly object data;
        private readonly int statusCode;

        public FormattedJsonResult(object data, int statusCode)
        {
            this.data = data;
            this.statusCode = statusCode;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var httpResponse = context.HttpContext.Response;
            
            httpResponse.Headers.Clear();
            httpResponse.StatusCode = statusCode;
            httpResponse.Headers.Add("Server", "TecoApi/1.0.1 (F2x CP2007I v2.3.058 N8 0105)");
            httpResponse.ContentType = "application/json";
            httpResponse.Headers.Add("Cache-Control", "no-cache");

            var formattedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            httpResponse.Headers.Add("Content-Length", formattedJson.Length.ToString());

            await httpResponse.WriteAsync(formattedJson);
        }
    }
}
