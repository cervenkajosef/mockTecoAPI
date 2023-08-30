namespace mockTecoAPI.Models.TecoApi.Rooms
{
    public class Attic : Room
    {
        private Attic() { }

        public static Attic Instance { get; } = new();
        public bool at_switch_1
        {
            get => at_light_1;
            set
            {
                if (value)
                {
                    at_light_1 = !at_light_1;
                }
            }
        }

        public bool at_switch_2
        {
            get => at_light_2;
            set
            {
                if (value)
                {
                    at_light_2 = !at_light_2;
                }
            }
        }

        public bool at_switch_3
        {
            get => at_light_3;
            set
            {
                if (value)
                {
                    at_light_3 = !at_light_3;
                }
            }
        }

        public bool at_switch_4
        {
            get => at_light_4;
            set
            {
                if (value)
                {
                    at_light_4 = !at_light_4;
                }
            }
        }

        public bool at_switch_5
        {
            get => at_light_5;
            set
            {
                if (value)
                {
                    at_light_5 = !at_light_5;
                }
            }
        }

        public bool at_light_1 { get; private set; }
        public bool at_light_2 { get; private set; }
        public bool at_light_3 { get; private set; }
        public bool at_light_4 { get; private set; }
        public bool at_light_5 { get; private set; }
        public double at_temp
        {
            get
            {
                dateTime = DateTime.Now;
                int modSec = dateTime.Second % defaultRoomTemp.Length;
                return defaultRoomTemp[modSec] + 2;
            }
        }
    }
}
