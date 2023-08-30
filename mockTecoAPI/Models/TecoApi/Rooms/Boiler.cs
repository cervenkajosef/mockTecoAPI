namespace mockTecoAPI.Models.TecoApi.Rooms
{
    public class Boiler : Room
    {
        private static readonly Boiler _instance = new();
        private DateTime lastUpdate;
        private bool _boiler_ON;
        private double _roomTemp;
        private Boiler()
        {
            boiler_set_temp_topLimit = 70;
            boiler_set_temp_bottomLimit = 40;
            boiler_temp = 20;
            _roomTemp = 20;
            boiler_on_off_switch = true;
            boiler_ID = "BPL2023-XYZ-4567";
        }

        public static Boiler Instance 
        {
            get
            {
                _instance.SimulateBoiler();
                return _instance;
            }
        }
        public string boiler_ID { get; set; }
        public int boiler_set_temp_topLimit { get; set; }
        public int boiler_set_temp_bottomLimit { get; set; }

        public double boiler_temp { get; private set; }
        public bool boiler_heating { get; private set; }
        public bool boiler_on_off_switch
        {
            get => _boiler_ON;
            set
            {
                lastUpdate = DateTime.Now;
                _boiler_ON = value;
                boiler_heating = value;
            }
        }

        private void SimulateBoiler()
        {
            var tempDif = (DateTime.Now - lastUpdate).Seconds;
            lastUpdate = DateTime.Now;
            if (_boiler_ON)
            {
                if (boiler_heating)
                    boiler_temp += tempDif;
                else
                    boiler_temp -= tempDif;

                if (boiler_set_temp_topLimit <= boiler_temp)
                {
                    boiler_heating = false;
                }
                if (boiler_temp <= boiler_set_temp_bottomLimit)
                {
                    boiler_heating = true;
                }
            }
            else
            {
                if (_roomTemp < boiler_temp)
                {
                    boiler_temp -= tempDif;
                }
            }
        }
    }
}
