namespace mockTecoAPI.Models.TecoApi.Rooms
{
    public class Bedroom : Room
    {
        private Bedroom(){}
        public static Bedroom Instance { get; } = new();

        public bool br_button
        {
            get => false;

            set
            {
                if (value)
                {
                    br_light = !br_light;
                }
            }
        }

        public bool br_light
        {
            get;
            private set;
        }

        public double br_temp
        {
            get
            {
                dateTime = DateTime.Now;
                int modSec = dateTime.Second % defaultRoomTemp.Length;
                return defaultRoomTemp[modSec];
            }
        }

    }
}
