using mockTecoAPI.Models.Error;
using mockTecoAPI.Models.TecoApi.Rooms.Tools;
using Newtonsoft.Json.Linq;

namespace mockTecoAPI.Models.TecoApi
{
    public class Result
    {
        public Result(ErrorResponse result, int status)
        {
            this.result = result;
            this.status = status;
        }

        public Result(JObject result)
        {
            this.result = result;
            status = StatusCodes.Status200OK;
        }

        public Result(RoomContext result)
        {
            var jsonResponse = new JObject
            {
                [result.roomName] = JObject.FromObject(result.roomObject)
            };
            this.result = jsonResponse;
            status = StatusCodes.Status200OK;
        }
        public Result()
        {
            result = null;
            status = StatusCodes.Status200OK;
        }

        public dynamic result { get; set; }
        public int status { get; set; }
    }
}
