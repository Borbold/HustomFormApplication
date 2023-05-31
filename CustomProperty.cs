using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustonRTEMS {
    struct SaveAddres {

    }
}

namespace HustonRTEMS {
    internal class CustomProperty : ConfigurationSection {
        [ConfigurationProperty("IP", IsRequired = false)]
        public string IP {
            get { return (string)base["IP"]; }
            set { base["IP"] = value; }
        }
        [ConfigurationProperty("PORT", IsRequired = false)]
        public string PORT {
            get { return (string)base["PORT"]; }
            set { base["PORT"] = value; }
        }


        [ConfigurationProperty("CANSpeed", IsRequired = false)]
        public string CANSpeed {
            get { return (string)base["CANSpeed"]; }
            set { base["CANSpeed"] = value; }
        }
        [ConfigurationProperty("CANPort", IsRequired = false)]
        public string CANPort {
            get { return (string)base["CANPort"]; }
            set { base["CANPort"] = value; }
        }

        [ConfigurationProperty("ReceiveMagAddres", IsRequired = false)]
        public string ReceiveMagAddres {
            get { return (string)base["ReceiveMagAddres"]; }
            set { base["ReceiveMagAddres"] = value; }
        }
        [ConfigurationProperty("IdReceiveMag", IsRequired = false)]
        public string IdReceiveMag {
            get { return (string)base["IdReceiveMag"]; }
            set { base["IdReceiveMag"] = value; }
        }
        [ConfigurationProperty("IdShipingMag", IsRequired = false)]
        public string IdShipingMag {
            get { return (string)base["IdShipingMag"]; }
            set { base["IdShipingMag"] = value; }
        }
    }
}
