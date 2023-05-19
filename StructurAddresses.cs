using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                addres = 15
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0x120,
                    0x121,
                    0x122,
                }
            }
        };
        public Transmission magnitudeTransmission2 = new() {
            TAddres = new StructurAddresses {
                addres = 15
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0x123,
                    0x124,
                    0x125,
                }
            }
        };

        public Transmission acselerometerTransmission = new() {
            TAddres = new StructurAddresses {
                addres = 9
            },
            TId = new StructurId {
                getValue = new int[3] {
                    0xB0,
                    0xB1,
                    0xB2,
                }
            }
        };
    }
}
