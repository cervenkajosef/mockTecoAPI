namespace mockTecoAPI.Models.TecoApi.Rooms
{
    public class Bedroom
    {
        private static Bedroom instance = new();
        private static readonly object padlock = new();

        private DateTime dateTime;
        private double[] temp = new double[] { 18.6, 19.0, 19.6, 18.9, 19.5, 19.6, 19.3, 19.4, 19.4, 19.1, 20.0, 20.3, 20.3, 19.9, 19.6, 20.0, 19.5, 19.4, 19.0, 19.4, 19.1, 19.5, 20.4, 20.1, 19.7, 19.5, 19.8, 20.0, 20.6, 20.7 };

        private volatile bool m_br_light;

        public static Bedroom Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance;
                }
            }
        }

        public bool br_switch_1
        {
            get
            {
                lock (padlock)
                {
                    return false;
                }
            }

            set
            {
                lock (padlock)
                {
                    if (value)
                    {
                        m_br_light = !m_br_light;
                    }
                }
            }
        }

        public bool br_light
        {
            get
            {
                lock (padlock)
                {
                    return m_br_light;
                }
            }
        }

        public double br_temp
        {
            get
            {
                lock (padlock)
                {
                    dateTime = DateTime.Now;
                    int modSec = dateTime.Second % temp.Length;
                    return temp[modSec];
                }
            }
        }

    }
}
