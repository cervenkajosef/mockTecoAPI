using mockTecoAPI.Models.Error;
using mockTecoAPI.Models.TecoApi.Rooms;
using Newtonsoft.Json.Linq;

namespace mockTecoAPI.Models.TecoApi
{
    public class TecoApi
    {
        private Bedroom bedroom = Bedroom.Instance;
        public Result GetList()
        {
            var jsonResponse = new JObject
            {
                ["glob_bedroom"] = new JObject()
            };

            return new Result(jsonResponse);
        }

        public Result SetObject(string param, string value)
        {
            var errors = new Errors();

            if (string.IsNullOrEmpty(param))
            {
                return new Result(errors.BadParameters(), StatusCodes.Status400BadRequest);
            }
            if (string.IsNullOrEmpty(value))
            {
                return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
            }
            value.ToLower();

            if (param.StartsWith("glob_bedroom."))
            {
                string[] subParams = param.Split('.');

                if (subParams.Last() == "br_switch_1")
                {
                    if (value == "1" || value == "true")
                        bedroom.br_switch_1 = true;
                }
                //when param exists, TecoAPI always return Ok on setting, even with bad value
                return new Result();
            }
            return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
        }

        public Result GetObject(string param)
        {
            var errors = new Errors();

            if (string.IsNullOrEmpty(param))
            {
                return new Result(errors.BadParameters(), StatusCodes.Status400BadRequest);
            }

            var jsonResponse = new JObject();
            param.ToLower();

            if (param == "glob_bedroom")
            {
                jsonResponse[param] = JObject.FromObject(bedroom);

                return new Result(jsonResponse);
            }
            else if (param.StartsWith("glob_bedroom."))
            {
                string[] subParams = param.Split('.');
                JObject subObject = new JObject();

                JToken currentToken = JObject.FromObject(bedroom);
                var subParam = subParams.Last();
                if (currentToken is JObject jObject && jObject.TryGetValue(subParam, out JToken subToken))
                {
                    subObject[subParam] = subToken;
                    currentToken = subToken;
                }
                else
                {
                    return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
                }

                jsonResponse[subParams.First()] = subObject;
                return new Result(jsonResponse);
            }
            return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
        }
    }
}
