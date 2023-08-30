namespace mockTecoAPI.Models.TecoApi.Rooms.Tools
{
    public class PubTools
    {
        public PubTools()
        {
            _dictionary["glob_bedroom"] = Bedroom.Instance;
            _dictionary["glob_attic"] = Attic.Instance;
            _dictionary["glob_boiler"] = Boiler.Instance;
            _dictionary["glob_garage"] = Garage.Instance;
        }
        public enum EResult { ENoAttr, EWithAttr, EError };
        private readonly Dictionary<string, object> _dictionary = new();

        public Dictionary<string, object> GetDictionary => _dictionary;

        public EResult FindRoom(string param, out RoomContext room)
        {
            room = new();
            var splitParam = param.Split(".");
            if (!CheckIfRoomsExists(splitParam))
                return EResult.EError;

            if (_dictionary.TryGetValue(splitParam.Last(), out var roomResult))
            {
                // last is class
                room.roomName = splitParam.Last();
                room.roomObject = roomResult;
                return EResult.ENoAttr;
            }
            if (1 < splitParam.Length && _dictionary.TryGetValue(splitParam[^2], out roomResult))
            {
                // last is attribute
                room.roomName = splitParam[^2];
                room.roomObject = roomResult;
                room.attribute = splitParam.Last();
                return EResult.EWithAttr;
            }

            return EResult.EError;
        }

        // checks if all rooms are in dictionary except the last one (that can be attribute) 
        bool CheckIfRoomsExists(string[] splitParam)
        {
            var lastItemIndex = splitParam.Length - 1;
            var secondLastItemIndex = splitParam.Length - 2 < 0 ? 0 : splitParam.Length - 2;

            for (int i = 0; i <= secondLastItemIndex; i++)
            {
                var substring = splitParam[i];
                var contains = _dictionary.TryGetValue(substring, out var roomResult);

                if (!contains && i == lastItemIndex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
