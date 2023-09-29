using System.Configuration;

namespace HustonRTEMS {
    internal class CustomProperty: ConfigurationSection {
        [ConfigurationProperty("IP", IsRequired = false)]
        public string IP
        {
            get => (string)base["IP"];
            set => base["IP"] = value;
        }
        [ConfigurationProperty("PortRTEMS", IsRequired = false)]
        public string PortRTEMS
        {
            get => (string)base["PortRTEMS"];
            set => base["PortRTEMS"] = value;
        }
        [ConfigurationProperty("PortHUSTON", IsRequired = false)]
        public string PortHUSTON
        {
            get => (string)base["PortHUSTON"];
            set => base["PortHUSTON"] = value;
        }
        [ConfigurationProperty("PortHUSTONTelnet", IsRequired = false)]
        public string PortHUSTONTelnet
        {
            get => (string)base["PortHUSTONTelnet"];
            set => base["PortHUSTONTelnet"] = value;
        }
        [ConfigurationProperty("PORT", IsRequired = false)]
        public string PORT
        {
            get => (string)base["PORT"];
            set => base["PORT"] = value;
        }

        [ConfigurationProperty("CANSpeed", IsRequired = false)]
        public string CANSpeed
        {
            get => (string)base["CANSpeed"];
            set => base["CANSpeed"] = value;
        }
        [ConfigurationProperty("CANPort", IsRequired = false)]
        public string CANPort
        {
            get => (string)base["CANPort"];
            set => base["CANPort"] = value;
        }
        // Temperature
        [ConfigurationProperty("ReceiveTemAddres", IsRequired = false)]
        public string ReceiveTemAddres
        {
            get => (string)base["ReceiveTemAddres"];
            set => base["ReceiveTemAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveTem", IsRequired = false)]
        public string IdReceiveTem
        {
            get => (string)base["IdReceiveTem"];
            set => base["IdReceiveTem"] = value;
        }
        [ConfigurationProperty("IdShipingTem", IsRequired = false)]
        public string IdShipingTem
        {
            get => (string)base["IdShipingTem"];
            set => base["IdShipingTem"] = value;
        }
        [ConfigurationProperty("SensorTemAddress", IsRequired = false)]
        public string SensorTemAddress
        {
            get => (string)base["SensorTemAddress"];
            set => base["SensorTemAddress"] = value;
        }
        // Magnetometer
        [ConfigurationProperty("ReceiveMagAddres", IsRequired = false)]
        public string ReceiveMagAddres
        {
            get => (string)base["ReceiveMagAddres"];
            set => base["ReceiveMagAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveMag", IsRequired = false)]
        public string IdReceiveMag
        {
            get => (string)base["IdReceiveMag"];
            set => base["IdReceiveMag"] = value;
        }
        [ConfigurationProperty("IdShipingMag", IsRequired = false)]
        public string IdShipingMag
        {
            get => (string)base["IdShipingMag"];
            set => base["IdShipingMag"] = value;
        }
        [ConfigurationProperty("SensorMagAddress1", IsRequired = false)]
        public string SensorMagAddress1
        {
            get => (string)base["SensorMagAddress1"];
            set => base["SensorMagAddress1"] = value;
        }
        [ConfigurationProperty("SensorMagAddress2", IsRequired = false)]
        public string SensorMagAddress2
        {
            get => (string)base["SensorMagAddress2"];
            set => base["SensorMagAddress2"] = value;
        }
        // Acselerometer
        [ConfigurationProperty("ReceiveAcsAddres", IsRequired = false)]
        public string ReceiveAcsAddres
        {
            get => (string)base["ReceiveAcsAddres"];
            set => base["ReceiveAcsAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveAcs", IsRequired = false)]
        public string IdReceiveAcs
        {
            get => (string)base["IdReceiveAcs"];
            set => base["IdReceiveAcs"] = value;
        }
        [ConfigurationProperty("IdShipingAcs", IsRequired = false)]
        public string IdShipingAcs
        {
            get => (string)base["IdShipingAcs"];
            set => base["IdShipingAcs"] = value;
        }
        [ConfigurationProperty("SensorAcsAddress", IsRequired = false)]
        public string SensorAcsAddress
        {
            get => (string)base["SensorAcsAddress"];
            set => base["SensorAcsAddress"] = value;
        }
        // Regulation
        [ConfigurationProperty("ReceiveRegAddres", IsRequired = false)]
        public string ReceiveRegAddres
        {
            get => (string)base["ReceiveRegAddres"];
            set => base["ReceiveRegAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveReg", IsRequired = false)]
        public string IdReceiveReg
        {
            get => (string)base["IdReceiveReg"];
            set => base["IdReceiveReg"] = value;
        }
        [ConfigurationProperty("IdShipingReg", IsRequired = false)]
        public string IdShipingReg
        {
            get => (string)base["IdShipingReg"];
            set => base["IdShipingReg"] = value;
        }
        [ConfigurationProperty("SensorRegAddress", IsRequired = false)]
        public string SensorRegAddress
        {
            get => (string)base["SensorRegAddress"];
            set => base["SensorRegAddress"] = value;
        }
        // Ratesensor
        [ConfigurationProperty("ReceiveRatAddres", IsRequired = false)]
        public string ReceiveRatAddres
        {
            get => (string)base["ReceiveRatAddres"];
            set => base["ReceiveRatAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveRat", IsRequired = false)]
        public string IdReceiveRat
        {
            get => (string)base["IdReceiveRat"];
            set => base["IdReceiveRat"] = value;
        }
        [ConfigurationProperty("IdShipingRat", IsRequired = false)]
        public string IdShipingRat
        {
            get => (string)base["IdShipingRat"];
            set => base["IdShipingRat"] = value;
        }
        [ConfigurationProperty("SensorRatAddress", IsRequired = false)]
        public string SensorRatAddress
        {
            get => (string)base["SensorRatAddress"];
            set => base["SensorRatAddress"] = value;
        }
        // Accelsensor
        [ConfigurationProperty("ReceiveAccAddres", IsRequired = false)]
        public string ReceiveAccAddres
        {
            get => (string)base["ReceiveAccAddres"];
            set => base["ReceiveAccAddres"] = value;
        }
        [ConfigurationProperty("IdReceiveAcc", IsRequired = false)]
        public string IdReceiveAcc
        {
            get => (string)base["IdReceiveAcc"];
            set => base["IdReceiveAcc"] = value;
        }
        [ConfigurationProperty("IdShipingAcc", IsRequired = false)]
        public string IdShipingAcc
        {
            get => (string)base["IdShipingAcc"];
            set => base["IdShipingAcc"] = value;
        }
        [ConfigurationProperty("SensorAccAddress", IsRequired = false)]
        public string SensorAccAddress
        {
            get => (string)base["SensorAccAddress"];
            set => base["SensorAccAddress"] = value;
        }
        // Time
        [ConfigurationProperty("ReceiveAddresTime", IsRequired = false)]
        public string ReceiveAddresTime
        {
            get => (string)base["ReceiveAddresTime"];
            set => base["ReceiveAddresTime"] = value;
        }
        [ConfigurationProperty("IdReceiveTime", IsRequired = false)]
        public string IdReceiveTime
        {
            get => (string)base["IdReceiveTime"];
            set => base["IdReceiveTime"] = value;
        }
        [ConfigurationProperty("IdShipingTime", IsRequired = false)]
        public string IdShipingTime
        {
            get => (string)base["IdShipingTime"];
            set => base["IdShipingTime"] = value;
        }
        [ConfigurationProperty("AddresTime", IsRequired = false)]
        public string AddresTime
        {
            get => (string)base["AddresTime"];
            set => base["AddresTime"] = value;
        }
        // Beacon
        [ConfigurationProperty("ReceiveAddresBeacon", IsRequired = false)]
        public string ReceiveAddresBeacon
        {
            get => (string)base["ReceiveAddresBeacon"];
            set => base["ReceiveAddresBeacon"] = value;
        }
        [ConfigurationProperty("IdReceiveBeacon", IsRequired = false)]
        public string IdReceiveBeacon
        {
            get => (string)base["IdReceiveBeacon"];
            set => base["IdReceiveBeacon"] = value;
        }
        [ConfigurationProperty("IdShipingBeacon", IsRequired = false)]
        public string IdShipingBeacon
        {
            get => (string)base["IdShipingBeacon"];
            set => base["IdShipingBeacon"] = value;
        }
        [ConfigurationProperty("SensorBeaconAddress", IsRequired = false)]
        public string SensorBeaconAddress
        {
            get => (string)base["SensorBeaconAddress"];
            set => base["SensorBeaconAddress"] = value;
        }
        // Extended Beacon
        [ConfigurationProperty("ReceiveAddresExBeacon", IsRequired = false)]
        public string ReceiveAddresExBeacon
        {
            get => (string)base["ReceiveAddresExBeacon"];
            set => base["ReceiveAddresExBeacon"] = value;
        }
        [ConfigurationProperty("IdReceiveExBeacon", IsRequired = false)]
        public string IdReceiveExBeacon
        {
            get => (string)base["IdReceiveExBeacon"];
            set => base["IdReceiveExBeacon"] = value;
        }
        [ConfigurationProperty("IdShipingExBeacon", IsRequired = false)]
        public string IdShipingExBeacon
        {
            get => (string)base["IdShipingExBeacon"];
            set => base["IdShipingExBeacon"] = value;
        }
        [ConfigurationProperty("SensorExBeaconAddress", IsRequired = false)]
        public string SensorExBeaconAddress
        {
            get => (string)base["SensorExBeaconAddress"];
            set => base["SensorExBeaconAddress"] = value;
        }
        // Adcs Beacon
        [ConfigurationProperty("ReceiveAddresAdcsBeacon", IsRequired = false)]
        public string ReceiveAddresAdcsBeacon
        {
            get => (string)base["ReceiveAddresAdcsBeacon"];
            set => base["ReceiveAddresAdcsBeacon"] = value;
        }
        [ConfigurationProperty("IdReceiveAdcsBeacon", IsRequired = false)]
        public string IdReceiveAdcsBeacon
        {
            get => (string)base["IdReceiveAdcsBeacon"];
            set => base["IdReceiveAdcsBeacon"] = value;
        }
        [ConfigurationProperty("IdShipingAdcsBeacon", IsRequired = false)]
        public string IdShipingAdcsBeacon
        {
            get => (string)base["IdShipingAdcsBeacon"];
            set => base["IdShipingAdcsBeacon"] = value;
        }
        [ConfigurationProperty("SensorAdcsBeaconAddress", IsRequired = false)]
        public string SensorAdcsBeaconAddress
        {
            get => (string)base["SensorAdcsBeaconAddress"];
            set => base["SensorAdcsBeaconAddress"] = value;
        }
    }
}
