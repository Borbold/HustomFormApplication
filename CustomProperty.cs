using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
