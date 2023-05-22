namespace HustonRTEMS {
    public struct StructurAddresses {
        public int addres;
    }
    public struct StructurId {
        public int[] getValue;
    }
    public struct Transmission {
        public StructurAddresses TAddres;
        public StructurId TId;
    }

    public enum VarEnum {
        X,
        Y,
        Z,
    }

    public class DefaultTransmission {
        public Transmission temperatureTransmission = new() {
            TAddres = new StructurAddresses {
                addres = 9
            },
            TId = new StructurId {
                getValue = new int[1] {
                    0xB0
                }
            }
        };

        public Transmission magnitudeTransmission1 = new() {
            TAddres = new StructurAddresses {
                addres = 0xA1
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0xB10,
                    0xB11,
                    0xB12,
                }
            }
        };
        public Transmission magnitudeTransmission2 = new() {
            TAddres = new StructurAddresses {
                addres = 0xA2
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0xB13,
                    0xB14,
                    0xB15,
                }
            }
        };

        public Transmission acselerometerTransmission = new() {
            TAddres = new StructurAddresses {
                addres = 8
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0xB16,
                    0xB17,
                    0xB18,
                }
            }
        };

        // Power on response
        public Transmission acknowledge = new() {
            TAddres = new StructurAddresses {
                addres = 4
            },
            TId = new StructurId {
                getValue = new int[1] {
                    0xDE24,
                }
            }
        };
    }
}
