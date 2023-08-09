using static HustonRTEMS.CanToUnican;

namespace HustonRTEMS
{
    public static class Crc16
    {
        private const ushort polynomial = 0xA001;
        private static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
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

    public struct Unican_message
    {
        public ushort unican_msg_id; //MSG_ID of unican message
        public ushort unican_address_from; // address of sender in sattelite network
        public ushort unican_address_to; // address of receiver in sattelite network
        public ushort unican_length; //length of data
        public byte[] data; //pointer to data field
    };
    public struct Can_message
    {
        public uint can_identifier; // 11 or 29bit CAN identifier
        public sbyte can_rtr;// Remote transmission request bit
        public sbyte can_extbit;// Identifier extension bit. 0x00 indicate 11 bit message ID
        public sbyte can_dlc;// Data length code. Number of bytes of data (0–8 bytes)
        public byte[] data;// Data field
    };

    public class CanTXBufferS
    {
        public Can_message cmsg;
        public CanTXBufferS? next;
    };

    internal class CanToUnican
    {
        private int UINT16LEFT(int val)
        {
            return ((val) >> 8) & 0x00FF;
        }
        private int UINT16RIGHT(int val)
        {
            return (val) & 0x00FF;
        }

        private const sbyte CAN_MAX_DLC = 8; //maximum value of data length code
        private const sbyte CAN_MIN_DLC = 2; //minimum value of data length code
        private const sbyte CRC_LENGTH = 2; //length of CRC field in bytes
        private const int UNICAN_START_LONG_MESSAGE = 0xFFFE;
        private const sbyte CAN_STANDART_HEADER = 0;
        private const sbyte CAN_EXTENDED_HEADER = 1;

        private CanTXBufferS? can_tx_buffer_s = null;

        private void AddCanMSGBuffer(ref Can_message cmsg)
        {
            CanTXBufferS tx_queue = new();
            tx_queue.cmsg = cmsg;
            tx_queue.next = null;
            if(can_tx_buffer_s == null)
                can_tx_buffer_s = tx_queue;
            else
            {
                CanTXBufferS can_buf = can_tx_buffer_s;
		        while (can_buf.next != null)
			        can_buf = can_buf.next;
		        can_buf.next = tx_queue;
            }
        }

        private void CanSetIdentifier(ref Unican_message msg, ref Can_message can_buff,
                    sbyte data_bit)
        {
            if((msg.unican_address_from > 31) || (msg.unican_address_to > 31))
            {
                can_buff.can_extbit = CAN_EXTENDED_HEADER;
                can_buff.can_identifier = 0x00;
                can_buff.can_identifier |= msg.unican_address_to;
                can_buff.can_identifier |= (uint)msg.unican_address_from << 14;
                if(data_bit != 0)
                {
                    can_buff.can_identifier |= 1 << 28;
                }
            } else
            {
                can_buff.can_extbit = CAN_STANDART_HEADER;
                can_buff.can_identifier = 0x00;
                can_buff.can_identifier |= msg.unican_address_to;
                can_buff.can_identifier |= (uint)msg.unican_address_from << 5;
                if(data_bit != 0)
                {
                    can_buff.can_identifier |= 1 << 10;
                }
            }
        }

        private class Can_tx_buffer_s
        {
            public Can_message cmsg;
            public Can_tx_buffer_s? next;
        };
        private readonly Can_tx_buffer_s? can_tx_buffer = null;

        public Can_message SendWithCAN(Unican_message umsg)
        {
            Can_message cmsg = new()
            {
                data = new byte[umsg.unican_length + CAN_MIN_DLC]
            };

            if(can_tx_buffer == null)
            {
                uint i;
                cmsg.can_rtr = 0;
                if(umsg.unican_length < 7)
                {
                    CanSetIdentifier(ref umsg, ref cmsg, 0);
                    cmsg.can_dlc = (sbyte)(umsg.unican_length + CAN_MIN_DLC);
                    cmsg.data[0] = (byte)(umsg.unican_msg_id & 0x00FF);
                    cmsg.data[1] = (byte)((umsg.unican_msg_id >> 8) & 0x00FF);
                    for(i = 0; i < umsg.unican_length; i++)
                    {
                        cmsg.data[i + 2] = umsg.data[i];
                    }
                } else
                {
                    ushort crc;
                    CanSetIdentifier(ref umsg, ref cmsg, 0);
                    crc = Crc16.ComputeChecksum(umsg.data);
                    cmsg.can_dlc = 6;
                    cmsg.data[0] = (byte)UINT16RIGHT(UNICAN_START_LONG_MESSAGE);
                    cmsg.data[1] = (byte)UINT16LEFT(UNICAN_START_LONG_MESSAGE);
                    cmsg.data[2] = (byte)UINT16RIGHT(umsg.unican_msg_id);
                    cmsg.data[3] = (byte)UINT16LEFT(umsg.unican_msg_id);
                    cmsg.data[4] = (byte)UINT16RIGHT(umsg.unican_length + CRC_LENGTH);
                    cmsg.data[5] = (byte)UINT16LEFT(umsg.unican_length + CRC_LENGTH);


                    Can_message cbmsg = new()
                    {
                        can_rtr = 0,
                        data = new byte[8]
                    };
                    CanSetIdentifier(ref umsg, ref cbmsg, 1);  //could be optimized
                    for(i = 0; i < umsg.unican_length; i++)
                    {
                        cbmsg.data[i % 8] = umsg.data[i];
                        if((i % 8) == 7)
                        {
                            cbmsg.can_dlc = 8;
                            AddCanMSGBuffer(ref cbmsg);
                            cbmsg = new()
                            {
                                data = new byte[8]
                            };
                            CanSetIdentifier(ref umsg, ref cbmsg, 1);
                            cbmsg.can_rtr = 0;
                        }
                    }
                    if((i % 8) > 0)  //something left
                    {
                        cbmsg.can_dlc = (sbyte)(i % 8);
                        if((i % 8) < 7)
                        {
                            cbmsg.data[i % 8] = (byte)UINT16RIGHT(crc);
                            cbmsg.data[(i % 8) + 1] = (byte)UINT16LEFT(crc);
                            cbmsg.can_dlc += 2;
                            AddCanMSGBuffer(ref cbmsg);
                        } else
                        {
                            cbmsg.data[7] = (byte)UINT16RIGHT(crc);
                            cbmsg.can_dlc = 8;
                            AddCanMSGBuffer(ref cbmsg);
                            cbmsg = new();
                            CanSetIdentifier(ref umsg, ref cbmsg, 1);
                            cbmsg.can_rtr = 0;
                            cbmsg.data[0] = (byte)UINT16LEFT(crc);
                            cbmsg.can_dlc = 1;
                            AddCanMSGBuffer(ref cbmsg);
                        }
                    } else
                    {
                        cbmsg.data[0] = (byte)UINT16RIGHT(crc);
                        cbmsg.data[1] = (byte)UINT16LEFT(crc);
                        cbmsg.can_dlc = 2;
                        AddCanMSGBuffer(ref cbmsg);
                    }
                }
            }
            return cmsg;
        }
    }
}
