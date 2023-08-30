namespace mockTecoAPI.Models.TecoApi.Rooms
{
    public class Garage : Room
    {
        private static readonly Garage _instance = new();

        private Garage()
        {
            garage_sensor_door_closed = true;
            openingTime = TimeSpan.FromSeconds(10);
        }
        public static Garage Instance
        {
            get
            {
                _instance.SimulateGarage();
                return _instance;
            }
        } 

        private DateTime garageDoorStarTime;
        private TimeSpan openingTime;

        private bool _doorClosing;
        private bool _doorOpening;
        private bool _door_opened;
        private bool _door_closed;

        public int garage_opening_closing_time
        {
            get
            {
                return openingTime.Seconds;
            }
            set
            {
                openingTime = TimeSpan.FromSeconds(value);
            }
        }

        public bool garage_button_door_open
        {
            get => false;
            set
            {
                if(garage_sensor_door_closed && value)
                {
                    garage_door_door_opening = true;
                    garage_sensor_door_closed = false;
                    garageDoorStarTime = DateTime.Now;
                }
            }
        }

        public bool garage_button_door_close
        {
            get => false;
            set
            {
                if (garage_sensor_door_opened && value)
                {
                    garage_door_door_closing = true;
                    garage_sensor_door_opened = false;
                    garageDoorStarTime = DateTime.Now;
                }
            }
        }

        public bool garage_door_door_closing
        {
            get
            {
                return _doorClosing;
            }
            private set => _doorClosing = value;
        }

        public bool garage_door_door_opening
        {
            get
            {
                return _doorOpening;
            }
            private set => _doorOpening = value;
        }

        public bool garage_sensor_door_opened {
            get
            {
                return _door_opened;
            }
            private set => _door_opened = value;
        }
        public bool garage_sensor_door_closed {
            get
            {
                return _door_closed;
            }
            private set => _door_closed = value;
        }

        public double garage_temp
        {
            get
            {
                dateTime = DateTime.Now;
                int modSec = dateTime.Second % _defaultRoomTemp.Length;
                return _defaultRoomTemp[modSec];
            }
        }

        private new readonly double[] _defaultRoomTemp =
        {
            16.9, 16.5, 17.2, 17.5, 17.6, 17.7, 18.1, 17.4, 17.5, 17.3,
            17.3, 17.6, 18.2, 18.4, 17.8, 17.3, 18.0, 18.4, 17.9, 18.3,
            18.3, 19.4, 19.1, 19.3, 19.5, 20.2, 20.0, 19.0, 18.3, 18.5
        };

        private void SimulateGarage()
        {
            if (garage_door_door_opening && garage_sensor_door_closed)
            {
                garage_sensor_door_closed = false;
            }

            if (garage_door_door_closing && garage_sensor_door_opened)
            {
                garage_sensor_door_opened = false;
            }

            var dif = DateTime.Now - garageDoorStarTime;
            if (openingTime < dif )
            {
                if (garage_door_door_opening)
                {
                    garage_sensor_door_opened = true;
                    garage_door_door_opening = false;
                }
                if (garage_door_door_closing)
                {
                    garage_sensor_door_closed = true;
                    garage_door_door_closing = false;
                }
            }
        }
    }
}
