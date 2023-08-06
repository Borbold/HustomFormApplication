namespace HustonRTEMS
{
    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(sbyte[] bytes)
        {
            ushort crc = 0;
            for(int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for(ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for(byte j = 0; j < 8; ++j)
                {
                    if(((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    } else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }

    internal class CanToUnican
    {
        private int UINT16LEFT(int val)
        {
            return (((val) >> 8) & 0x00FF);
        }
        private int UINT16RIGHT(int val)
        {
            return ((val) & 0x00FF);
        }

        const sbyte CAN_MAX_DLC = 8; //maximum value of data length code
        const sbyte CAN_MIN_DLC = 2; //minimum value of data length code
        const sbyte CRC_LENGTH = 2; //length of CRC field in bytes
        const int UNICAN_START_LONG_MESSAGE = 0xFFFE;

        const sbyte CAN_STANDART_HEADER = 0;
        const sbyte CAN_EXTENDED_HEADER = 1;

        public struct Unican_message
        {
            public UInt16 unican_msg_id; //MSG_ID of unican message
            public UInt16 unican_address_from; // address of sender in sattelite network
            public UInt16 unican_address_to; // address of receiver in sattelite network
            public UInt16 unican_length; //length of data
            public sbyte[] data; //pointer to data field
        };
        struct Can_message
        {
            public UInt32 can_identifier; // 11 or 29bit CAN identifier
            public sbyte can_rtr;// Remote transmission request bit
            public sbyte can_extbit;// Identifier extension bit. 0x00 indicate 11 bit message ID
            public sbyte can_dlc;// Data length code. Number of bytes of data (0–8 bytes)
            public volatile sbyte[] data;// Data field
        };

        /*
         X-Modem CRC16 x^16+x^12+x^5+1
         */
        private readonly UInt16[] crc16tab = { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084,
        0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad,
        0xe1ce, 0xf1ef, 0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7,
        0x62d6, 0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
        0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485, 0xa56a,
        0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d, 0x3653, 0x2672,
        0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4, 0xb75b, 0xa77a, 0x9719,
        0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc, 0x48c4, 0x58e5, 0x6886, 0x78a7,
        0x0840, 0x1861, 0x2802, 0x3823, 0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948,
        0x9969, 0xa90a, 0xb92b, 0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50,
        0x3a33, 0x2a12, 0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b,
        0xab1a, 0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
        0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49, 0x7e97,
        0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70, 0xff9f, 0xefbe,
        0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78, 0x9188, 0x81a9, 0xb1ca,
        0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f, 0x1080, 0x00a1, 0x30c2, 0x20e3,
        0x5004, 0x4025, 0x7046, 0x6067, 0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d,
        0xd31c, 0xe37f, 0xf35e, 0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214,
        0x6277, 0x7256, 0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c,
        0xc50d, 0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
        0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c, 0x26d3,
        0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634, 0xd94c, 0xc96d,
        0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab, 0x5844, 0x4865, 0x7806,
        0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3, 0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e,
        0x8bf9, 0x9bd8, 0xabbb, 0xbb9a, 0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1,
        0x1ad0, 0x2ab3, 0x3a92, 0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b,
        0x9de8, 0x8dc9, 0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0,
        0x0cc1, 0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
        0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0 };

        private void can_set_identifier(ref Unican_message msg, ref Can_message can_buff,
            sbyte data_bit)
        {
            if((msg.unican_address_from > 31) || (msg.unican_address_to > 31))
            {
                can_buff.can_extbit = CAN_EXTENDED_HEADER;
                can_buff.can_identifier = 0x00;
                can_buff.can_identifier |= msg.unican_address_to;
                can_buff.can_identifier |= (uint)msg.unican_address_from << 14;
                if(data_bit != 0)
                    can_buff.can_identifier |= 1 << 28;
            } else
            {
                can_buff.can_extbit = CAN_STANDART_HEADER;
                can_buff.can_identifier = 0x00;
                can_buff.can_identifier |= msg.unican_address_to;
                can_buff.can_identifier |= (uint)msg.unican_address_from << 5;
                if(data_bit != 0)
                    can_buff.can_identifier |= 1 << 10;
            }
        }

        class Can_tx_buffer_s
        {
            public Can_message cmsg;
            public Can_tx_buffer_s? next;
        };
        Can_tx_buffer_s? can_tx_buffer = null;

        private void Add_can_msg_to_buffer(Can_message cmsg)
        {
            Can_tx_buffer_s tx_queue = new();
            tx_queue.cmsg = cmsg;
            tx_queue.next = null;
            if(can_tx_buffer == null)
                can_tx_buffer = tx_queue;
            else
            {
                Can_tx_buffer_s can_buf = can_tx_buffer;
                while(can_buf.next != null)
                    can_buf = can_buf.next;
                can_buf.next = tx_queue;
            }
        }

        private void Send_can_msg(Can_message cmsg)
        {
            Add_can_msg_to_buffer(cmsg);
        }

        public sbyte Send_with_CAN(ref Unican_message umsg)
        {
            sbyte sc = 1;
            if(can_tx_buffer == null)
            {
                sc = 0;
                Can_message cmsg = new();
                uint i;
                cmsg.can_rtr = 0;
                if(umsg.unican_length < 7)
                {
                    can_set_identifier(ref umsg, ref cmsg, 0);
                    cmsg.can_dlc = (sbyte)(umsg.unican_length + CAN_MIN_DLC);
                    cmsg.data[0] = ((sbyte)(umsg.unican_msg_id & 0x00FF));
                    cmsg.data[1] = ((sbyte)((umsg.unican_msg_id >> 8) & 0x00FF));
                    for(i = 0; i < umsg.unican_length; i++)
                        cmsg.data[i + 2] = umsg.data[i];
                    Send_can_msg(cmsg);
                } else
                {
                    UInt16 crc;
                    can_set_identifier(ref umsg, ref cmsg, 0);
                    crc = Crc16.ComputeChecksum(umsg.data);
                    cmsg.can_dlc = 6;
                    cmsg.data[0] = (sbyte)UINT16RIGHT(UNICAN_START_LONG_MESSAGE);
                    cmsg.data[1] = (sbyte)UINT16LEFT(UNICAN_START_LONG_MESSAGE);
                    cmsg.data[2] = (sbyte)UINT16RIGHT(umsg.unican_msg_id);
                    cmsg.data[3] = (sbyte)UINT16LEFT(umsg.unican_msg_id);
                    cmsg.data[4] = (sbyte)UINT16RIGHT(umsg.unican_length + CRC_LENGTH);
                    cmsg.data[5] = (sbyte)UINT16LEFT(umsg.unican_length + CRC_LENGTH);


                    Can_message cbmsg = new();
                    cbmsg.can_rtr = 0;
                    can_set_identifier(ref umsg, ref cbmsg, 1);  //could be optimized
                    for(i = 0; i < umsg.unican_length; i++)
                    {
                        cbmsg.data[i % 8] = umsg.data[i];
                        if((i % 8) == 7)
                        {
                            cbmsg.can_dlc = 8;
                            Add_can_msg_to_buffer(cbmsg);
                            cbmsg = new();
                            can_set_identifier(ref umsg, ref cbmsg, 1);
                            cbmsg.can_rtr = 0;
                        }
                    }
                    if((i % 8) > 0)  //something left
                    {
                        cbmsg.can_dlc = (sbyte)(i % 8);
                        if((i % 8) < 7)
                        {
                            cbmsg.data[i % 8] = (sbyte)UINT16RIGHT(crc);
                            cbmsg.data[i % 8 + 1] = (sbyte)UINT16LEFT(crc);
                            cbmsg.can_dlc += 2;
                            Add_can_msg_to_buffer(cbmsg);
                        } else
                        {
                            cbmsg.data[7] = (sbyte)UINT16RIGHT(crc);
                            cbmsg.can_dlc = 8;
                            Add_can_msg_to_buffer(cbmsg);
                            cbmsg = new();
                            can_set_identifier(ref umsg, ref cbmsg, 1);
                            cbmsg.can_rtr = 0;
                            cbmsg.data[0] = (sbyte)UINT16LEFT(crc);
                            cbmsg.can_dlc = 1;
                            Add_can_msg_to_buffer(cbmsg);
                        }
                    } else
                    {
                        cbmsg.data[0] = (sbyte)UINT16RIGHT(crc);
                        cbmsg.data[1] = (sbyte)UINT16LEFT(crc);
                        cbmsg.can_dlc = 2;
                        Add_can_msg_to_buffer(cbmsg);
                    }
                    Send_can_msg(cmsg);
                }
            }
            return sc;
        }
    }
}
