using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    [StructLayout(LayoutKind.Explicit)]
    public struct fl_un {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;
        [FieldOffset(2)]
        public byte byte3;
        [FieldOffset(3)]
        public byte byte4;

        [FieldOffset(0)]
        public float fl;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct it_un {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;

        [FieldOffset(0)]
        public int it;
    }
}

namespace HustonRTEMS {
    internal class GeneralFunctional {
        public static void ChangingPositionAccelerometer() {
            int jon = 5;
            for(int i = 0; i < 5; i++) {
                Thread.Sleep(100);
                jon++;
            }
        }

        public static async void SendMessageInSocket(Socket serverListener, fl_un fl, it_un it,
                byte[] hardBufWrite, TextBox logBox) {
            if(serverListener != null && serverListener.Connected) {
                hardBufWrite[20] = it.byte1;
                hardBufWrite[21] = it.byte2;

                hardBufWrite[26] = fl.byte1;
                hardBufWrite[27] = fl.byte2;
                hardBufWrite[28] = fl.byte3;
                hardBufWrite[29] = fl.byte4;
                _ = await serverListener.SendAsync(hardBufWrite, SocketFlags.None);
            } else {
                logBox.Text = "Socet don't open!";
            }
        }

        public static async void SendMessageInSocket(Socket serverListener,
                byte[] hardBufWrite, TextBox logBox) {
            if(serverListener != null && serverListener.Connected) {
                _ = await serverListener.SendAsync(hardBufWrite, SocketFlags.None);
            } else {
                logBox.Text = "Socet don't open!";
            }
        }

        public static void SendChangeKissFESC(ref byte[] chageByte) {
            byte KISS_FEND = 0xC0;
            byte KISS_FESC = 0xDB;
            byte KISS_TFEND = 0xDC;
            byte KISS_TFESC = 0xDD;
            bool fesk_was = false;

            for(int i = 18; i < chageByte.Length - 1; i++) {
                if(chageByte[i] == KISS_FEND) {
                    if(fesk_was) {
                        chageByte[i] = KISS_TFEND;
                        fesk_was = false;
                        i++;
                    } else {
                        chageByte[i] = KISS_FESC;
                        fesk_was = true;
                    }
                } else if(chageByte[i] == KISS_FESC) {
                    if(fesk_was) {
                        chageByte[i] = KISS_TFESC;
                        fesk_was = false;
                        i++;
                    } else {
                        chageByte[i] = KISS_FESC;
                        fesk_was = true;
                    }
                }
            }
        }

        public static void WriteChangeKissFESC(ref byte[] chageByte) {
            byte KISS_FEND = 0xC0;
            byte KISS_FESC = 0xDB;
            byte KISS_TFEND = 0xDC;
            byte KISS_TFESC = 0xDD;

            for(int i = 18; i < chageByte.Length - 1; i++) {
                if(chageByte[i] == KISS_TFEND)
                    chageByte[i] = KISS_FEND;
                else if(chageByte[i] == KISS_TFESC)
                    chageByte[i] = KISS_FESC;
            }
        }
    }
}
