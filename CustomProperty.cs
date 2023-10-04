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
        // RateSens X
        [ConfigurationProperty("ReceiveAddresRSX", IsRequired = false)]
        public string ReceiveAddresRSX
        {
            get => (string)base["ReceiveAddresRSX"];
            set => base["ReceiveAddresRSX"] = value;
        }
        [ConfigurationProperty("IdReceiveRSX", IsRequired = false)]
        public string IdReceiveRSX
        {
            get => (string)base["IdReceiveRSX"];
            set => base["IdReceiveRSX"] = value;
        }
        [ConfigurationProperty("IdShipingRSX", IsRequired = false)]
        public string IdShipingRSX
        {
            get => (string)base["IdShipingRSX"];
            set => base["IdShipingRSX"] = value;
        }
        [ConfigurationProperty("AddressRSX", IsRequired = false)]
        public string AddressRSX
        {
            get => (string)base["AddressRSX"];
            set => base["AddressRSX"] = value;
        }
        // RateSens Y
        [ConfigurationProperty("ReceiveAddresRSY", IsRequired = false)]
        public string ReceiveAddresRSY
        {
            get => (string)base["ReceiveAddresRSY"];
            set => base["ReceiveAddresRSY"] = value;
        }
        [ConfigurationProperty("IdReceiveRSY", IsRequired = false)]
        public string IdReceiveRSY
        {
            get => (string)base["IdReceiveRSY"];
            set => base["IdReceiveRSY"] = value;
        }
        [ConfigurationProperty("IdShipingRSY", IsRequired = false)]
        public string IdShipingRSY
        {
            get => (string)base["IdShipingRSY"];
            set => base["IdShipingRSY"] = value;
        }
        [ConfigurationProperty("AddressRSY", IsRequired = false)]
        public string AddressRSY
        {
            get => (string)base["AddressRSY"];
            set => base["AddressRSY"] = value;
        }
        // RateSens Z
        [ConfigurationProperty("ReceiveAddresRSZ", IsRequired = false)]
        public string ReceiveAddresRSZ
        {
            get => (string)base["ReceiveAddresRSZ"];
            set => base["ReceiveAddresRSZ"] = value;
        }
        [ConfigurationProperty("IdReceiveRSZ", IsRequired = false)]
        public string IdReceiveRSZ
        {
            get => (string)base["IdReceiveRSZ"];
            set => base["IdReceiveRSZ"] = value;
        }
        [ConfigurationProperty("IdShipingRSZ", IsRequired = false)]
        public string IdShipingRSZ
        {
            get => (string)base["IdShipingRSZ"];
            set => base["IdShipingRSZ"] = value;
        }
        [ConfigurationProperty("AddressRSZ", IsRequired = false)]
        public string AddressRSZ
        {
            get => (string)base["AddressRSZ"];
            set => base["AddressRSZ"] = value;
        }
        // Magnetometer X
        [ConfigurationProperty("ReceiveAddresMagX", IsRequired = false)]
        public string ReceiveAddresMagX
        {
            get => (string)base["ReceiveAddresMagX"];
            set => base["ReceiveAddresMagX"] = value;
        }
        [ConfigurationProperty("IdReceiveMagX", IsRequired = false)]
        public string IdReceiveMagX
        {
            get => (string)base["IdReceiveMagX"];
            set => base["IdReceiveMagX"] = value;
        }
        [ConfigurationProperty("IdShipingMagX", IsRequired = false)]
        public string IdShipingMagX
        {
            get => (string)base["IdShipingMagX"];
            set => base["IdShipingMagX"] = value;
        }
        [ConfigurationProperty("AddressMagX", IsRequired = false)]
        public string AddressMagX
        {
            get => (string)base["AddressMagX"];
            set => base["AddressMagX"] = value;
        }
        // Magnetometer Y
        [ConfigurationProperty("ReceiveAddresMagY", IsRequired = false)]
        public string ReceiveAddresMagY
        {
            get => (string)base["ReceiveAddresMagY"];
            set => base["ReceiveAddresMagY"] = value;
        }
        [ConfigurationProperty("IdReceiveMagY", IsRequired = false)]
        public string IdReceiveMagY
        {
            get => (string)base["IdReceiveMagY"];
            set => base["IdReceiveMagY"] = value;
        }
        [ConfigurationProperty("IdShipingMagY", IsRequired = false)]
        public string IdShipingMagY
        {
            get => (string)base["IdShipingMagY"];
            set => base["IdShipingMagY"] = value;
        }
        [ConfigurationProperty("AddressMagY", IsRequired = false)]
        public string AddressMagY
        {
            get => (string)base["AddressMagY"];
            set => base["AddressMagY"] = value;
        }
        // Magnetometer Z
        [ConfigurationProperty("ReceiveAddresMagZ", IsRequired = false)]
        public string ReceiveAddresMagZ
        {
            get => (string)base["ReceiveAddresMagZ"];
            set => base["ReceiveAddresMagZ"] = value;
        }
        [ConfigurationProperty("IdReceiveMagZ", IsRequired = false)]
        public string IdReceiveMagZ
        {
            get => (string)base["IdReceiveMagZ"];
            set => base["IdReceiveMagZ"] = value;
        }
        [ConfigurationProperty("IdShipingMagZ", IsRequired = false)]
        public string IdShipingMagZ
        {
            get => (string)base["IdShipingMagZ"];
            set => base["IdShipingMagZ"] = value;
        }
        [ConfigurationProperty("AddressMagZ", IsRequired = false)]
        public string AddressMagZ
        {
            get => (string)base["AddressMagZ"];
            set => base["AddressMagZ"] = value;
        }
    }
}
