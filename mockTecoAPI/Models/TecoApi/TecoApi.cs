using System.Reflection;
using mockTecoAPI.Controllers;
using mockTecoAPI.Models.Error;
using mockTecoAPI.Models.TecoApi.Rooms.Tools;
using Newtonsoft.Json.Linq;

namespace mockTecoAPI.Models.TecoApi
{
    public class TecoApi
    {
        private readonly ILogger<TecoApiController> _logger;
        private RoomDictionary _roomDictionary = new();

        public TecoApi(ILogger<TecoApiController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Result GetList()
        {
            JObject jsonObject = new();
            foreach (var key in _roomDictionary.GetDictionary.Keys)
            {
                jsonObject[key] = new JObject();
            }
            return new Result(jsonObject);
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
            var loValue = value.ToLower();
            var loParam = param.ToLower();

            var result = _roomDictionary.FindRoom(loParam, out var room);

            if (result != RoomDictionary.EResult.EWithAttr)
                return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);

            Type type = room.roomObject.GetType();
            var roomProperty = GetCaseInsensitiveProperty(type, room.attribute);

            try
            {
                if (roomProperty?.SetMethod != null && roomProperty.CanWrite && roomProperty.SetMethod.IsPublic)
                {
                    dynamic setValue;
                    var propType = roomProperty.PropertyType;
                    if (propType == typeof(bool))
                    {
                        if (loValue == "1")
                        {
                            setValue = true;
                        }
                        else if (loValue == "0")
                        {
                            setValue = false;
                        }
                        else
                            setValue = Convert.ToBoolean(loValue);
                    }
                    else if (propType == typeof(double))
                        setValue = Convert.ToDouble(loValue);
                    else if (propType == typeof(int))
                        setValue = Convert.ToInt32(loValue);
                    else
                        setValue = value;

                    roomProperty.SetValue(room.roomObject, setValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Setting variable failed!");
                //when param exists, TecoAPI always return Ok on setting, even with bad value
            }

            return new Result();
        }

        public Result GetObject(string param)
        {
            var errors = new Errors();

            if (string.IsNullOrEmpty(param))
            {
                return new Result(errors.BadParameters(), StatusCodes.Status400BadRequest);
            }

            var jsonResponse = new JObject();
            var loParam = param.ToLower();

            var result = _roomDictionary.FindRoom(loParam, out var room);

            if (result == RoomDictionary.EResult.EError || room == null)
                return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);

            if (result == RoomDictionary.EResult.ENoAttr)
                return new Result(room);

            Type type = room.roomObject.GetType();
            var roomProperty = GetCaseInsensitiveProperty(type, room.attribute);

            JObject subObject = new();
            if (roomProperty?.GetMethod != null)
            {
                var value = roomProperty.GetValue(room.roomObject);
                if (value == null)
                {
                    return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
                }
                subObject[roomProperty.Name] = value.ToString();
            }
            else
            {
                return new Result(errors.ErrorNotFound(param), StatusCodes.Status400BadRequest);
            }

            jsonResponse[room.roomName] = subObject;
            return new Result(jsonResponse);
        }

        private PropertyInfo GetCaseInsensitiveProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
