using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace mockTecoAPI.Rooms
{
    public class Bedroom
    {
        private static Bedroom instance = null;
        private static readonly object padlock = new object();

        private DateTime dateTime;
        private double[] temp = new double[] { 18.6, 19.0, 19.6, 18.9, 19.5, 19.6, 19.3, 19.4, 19.4, 19.1, 20.0, 20.3, 20.3, 19.9, 19.6, 20.0, 19.5, 19.4, 19.0, 19.4, 19.1, 19.5, 20.4, 20.1, 19.7, 19.5, 19.8, 20.0, 20.6, 20.7 };
        public bool br_switch_1 {
            get { return false; }
            
            set{
                if (value)
                {
                    br_light = !br_light;
                }
            } 
        }

        public static Bedroom Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Bedroom();
                    }
                    return instance;
                }
            }
        }

        public bool br_light { get; set;}
        public double br_temp { 
            get {

            dateTime = DateTime.Now;
            int modSec = dateTime.Second % temp.Length;
            return temp[modSec];           
            
            }
            set { }
        }

    }
}
