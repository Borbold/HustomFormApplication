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
        [ConfigurationProperty("PortRTEMS", IsRequired = false)]
        public string PortRTEMS {
            get { return (string)base["PortRTEMS"]; }
            set { base["PortRTEMS"] = value; }
        }
        [ConfigurationProperty("PortHUSTON", IsRequired = false)]
        public string PortHUSTON {
            get { return (string)base["PortHUSTON"]; }
            set { base["PortHUSTON"] = value; }
        }
        [ConfigurationProperty("PortHUSTONTelnet", IsRequired = false)]
        public string PortHUSTONTelnet {
            get { return (string)base["PortHUSTONTelnet"]; }
            set { base["PortHUSTONTelnet"] = value; }
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
        // Magnetometer
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
        [ConfigurationProperty("SensorMagAddress1", IsRequired = false)]
        public string SensorMagAddress1 {
            get { return (string)base["SensorMagAddress1"]; }
            set { base["SensorMagAddress1"] = value; }
        }
        [ConfigurationProperty("SensorMagAddress2", IsRequired = false)]
        public string SensorMagAddress2 {
            get { return (string)base["SensorMagAddress2"]; }
            set { base["SensorMagAddress2"] = value; }
        }
        // Acselerometer
        [ConfigurationProperty("ReceiveAcsAddres", IsRequired = false)]
        public string ReceiveAcsAddres {
            get { return (string)base["ReceiveAcsAddres"]; }
            set { base["ReceiveAcsAddres"] = value; }
        }
        [ConfigurationProperty("IdReceiveAcs", IsRequired = false)]
        public string IdReceiveAcs {
            get { return (string)base["IdReceiveAcs"]; }
            set { base["IdReceiveAcs"] = value; }
        }
        [ConfigurationProperty("IdShipingAcs", IsRequired = false)]
        public string IdShipingAcs {
            get { return (string)base["IdShipingAcs"]; }
            set { base["IdShipingAcs"] = value; }
        }
        [ConfigurationProperty("SensorAcsAddress", IsRequired = false)]
        public string SensorAcsAddress {
            get { return (string)base["SensorAcsAddress"]; }
            set { base["SensorAcsAddress"] = value; }
        }
        // Regulation
        [ConfigurationProperty("ReceiveRegAddres", IsRequired = false)]
        public string ReceiveRegAddres {
            get { return (string)base["ReceiveRegAddres"]; }
            set { base["ReceiveRegAddres"] = value; }
        }
        [ConfigurationProperty("IdReceiveReg", IsRequired = false)]
        public string IdReceiveReg {
            get { return (string)base["IdReceiveReg"]; }
            set { base["IdReceiveReg"] = value; }
        }
        [ConfigurationProperty("IdShipingReg", IsRequired = false)]
        public string IdShipingReg {
            get { return (string)base["IdShipingReg"]; }
            set { base["IdShipingReg"] = value; }
        }
        [ConfigurationProperty("SensorRegAddress", IsRequired = false)]
        public string SensorRegAddress {
            get { return (string)base["SensorRegAddress"]; }
            set { base["SensorRegAddress"] = value; }
        }
        // Ratesensor
        [ConfigurationProperty("ReceiveRatAddres", IsRequired = false)]
        public string ReceiveRatAddres {
            get { return (string)base["ReceiveRatAddres"]; }
            set { base["ReceiveRatAddres"] = value; }
        }
        [ConfigurationProperty("IdReceiveRat", IsRequired = false)]
        public string IdReceiveRat {
            get { return (string)base["IdReceiveRat"]; }
            set { base["IdReceiveRat"] = value; }
        }
        [ConfigurationProperty("IdShipingRat", IsRequired = false)]
        public string IdShipingRat {
            get { return (string)base["IdShipingRat"]; }
            set { base["IdShipingRat"] = value; }
        }
        [ConfigurationProperty("SensorRatAddress", IsRequired = false)]
        public string SensorRatAddress {
            get { return (string)base["SensorRatAddress"]; }
            set { base["SensorRatAddress"] = value; }
        }
        // Accelsensor
        [ConfigurationProperty("ReceiveAccAddres", IsRequired = false)]
        public string ReceiveAccAddres {
            get { return (string)base["ReceiveAccAddres"]; }
            set { base["ReceiveAccAddres"] = value; }
        }
        [ConfigurationProperty("IdReceiveAcc", IsRequired = false)]
        public string IdReceiveAcc {
            get { return (string)base["IdReceiveAcc"]; }
            set { base["IdReceiveAcc"] = value; }
        }
        [ConfigurationProperty("IdShipingAcc", IsRequired = false)]
        public string IdShipingAcc {
            get { return (string)base["IdShipingAcc"]; }
            set { base["IdShipingAcc"] = value; }
        }
        [ConfigurationProperty("SensorAccAddress", IsRequired = false)]
        public string SensorAccAddress {
            get { return (string)base["SensorAccAddress"]; }
            set { base["SensorAccAddress"] = value; }
        }
        // Time
        [ConfigurationProperty("ReceiveAddresTime", IsRequired = false)]
        public string ReceiveAddresTime {
            get { return (string)base["ReceiveAddresTime"]; }
            set { base["ReceiveAddresTime"] = value; }
        }
        [ConfigurationProperty("IdReceiveTime", IsRequired = false)]
        public string IdReceiveTime {
            get { return (string)base["IdReceiveTime"]; }
            set { base["IdReceiveTime"] = value; }
        }
        [ConfigurationProperty("IdShipingTime", IsRequired = false)]
        public string IdShipingTime {
            get { return (string)base["IdShipingTime"]; }
            set { base["IdShipingTime"] = value; }
        }
    }
}
