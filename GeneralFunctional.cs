using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    [StructLayout(LayoutKind.Explicit)]
    public struct FlUn {
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
    public struct ItUn {
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
        private readonly static byte KISS_FEND = 0xC0;
        private readonly static byte[] kissHeader = {
            0xC0, 0x00, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x00, 0xF0 };

        public static void ChangingPositionAccelerometer() {
            int jon = 5;
            for(int i = 0; i < 5; i++) {
                Thread.Sleep(100);
                jon++;
            }
        }

        public static async void SendMessageInSocket(Socket serverListener,
            int idShipping, int addresValue, int addresReceive,
            int iCount, int fCount, int[] arIValue, float[] arFValue,
            TextBox logBox) {
            FlUn fValue = new();
            ItUn iValue = new();
            // Header KISS
            byte[] sendBuf = new byte[27 + (fCount * 4) + (iCount * 2)];
            Array.Copy(kissHeader, sendBuf, 18);
            // Header UNICAN
            iValue.it = idShipping;
            sendBuf[18] = iValue.byte1;
            sendBuf[19] = iValue.byte2;
            iValue.it = addresValue;
            sendBuf[20] = iValue.byte1;
            sendBuf[21] = iValue.byte2;
            iValue.it = addresReceive;
            sendBuf[22] = iValue.byte1;
            sendBuf[23] = iValue.byte2;
            iValue.it = (fCount * 4) + (iCount * 2);
            sendBuf[24] = iValue.byte1;
            sendBuf[25] = iValue.byte2;

            for(int i = 0; i < iCount; i++) {
                iValue.it = arIValue[i];
                sendBuf[26 + (i * 2)] = iValue.byte1;
                sendBuf[27 + (i * 2)] = iValue.byte2;
            }
            for(int f = 0; f < fCount; f++) {
                fValue.fl = arFValue[f];
                sendBuf[26 + (f * 4)] = fValue.byte1;
                sendBuf[27 + (f * 4)] = fValue.byte2;
                sendBuf[28 + (f * 4)] = fValue.byte3;
                sendBuf[29 + (f * 4)] = fValue.byte4;
            }

            sendBuf[^1] = KISS_FEND;

            SendChangeKissFESC(ref sendBuf);

            if(serverListener != null && serverListener.Connected) {
                await serverListener.SendAsync(sendBuf, SocketFlags.None);
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public static async void SendMessageInSocket(Socket serverListener, FlUn fl, ItUn it,
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
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public static async void SendMessageInSocket(Socket serverListener,
                byte[] hardBufWrite, TextBox logBox) {
            if(serverListener != null && serverListener.Connected) {
                try {
                    await serverListener.SendAsync(hardBufWrite, SocketFlags.None);
                }catch (Exception) {}
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public static void SendChangeKissFESC(ref byte[] chageByte) {
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

        public static void CanToApp11(byte[] charkArray) {
            int coutn_byte = 14;
            byte[] testCan = new byte[coutn_byte];
            //----------------------------------------------------------
            if(charkArray[6] - 0x30 != 0)
                testCan[0] = (byte)((charkArray[6] - 0x30 - (charkArray[6] > 0x40 ? 0 : 7)) << 4);
            if(charkArray[5] - 0x30 != 0)
                testCan[0] += (byte)((charkArray[5] - 0x30 - (charkArray[6] > 0x40 ? 0 : 7)) << 4);
            //----------------------------------------------------------
            if(charkArray[3] - 0x30 != 0)
                testCan[2] = (byte)((charkArray[6] - 0x30 - (charkArray[6] > 0x40 ? 0 : 7)) << 4);
            //----------------------------------------------------------
            testCan[coutn_byte - 1] = 0xD;

            foreach(byte i in testCan) {
                Debug.Write($"{i:X} ");
            }

            Debug.WriteLine("");
        }
        public static void CanToApp29(byte[] charkArray) {
            // Пример приема с CAN. Строку в число
            byte[] testCan = new byte[charkArray.Length];
            for(int i = 0; i < charkArray.Length; i++) {
                testCan[i] = Convert.ToByte(charkArray[i]);
            }
            foreach(byte i in testCan) {
                Debug.Write($"{i:X} ");
            }
            Debug.WriteLine("");
        }

        public static byte[] AppToCan11(byte[] charkArray) {
            int coutn_byte = 14;
            byte[] testCan = new byte[coutn_byte];
            // Пример приема с сервера. Число в строку
            testCan[0] = Convert.ToByte('t');
            for(int i = 1; i < coutn_byte; i++) {
                testCan[i] = 0x30;
            }
            //----------------------------------------------------------
            if((charkArray[0] & 0xF) >= 0xA)
                testCan[6] = (byte)((charkArray[0] & 0xF) + 0x30 + 7);
            else
                testCan[6] = (byte)((charkArray[0] & 0xF) + 0x30);
            if((charkArray[0] >> 4) >= 0xA)
                testCan[5] = (byte)((charkArray[0] >> 4) + 0x30 + 7);
            else
                testCan[5] = (byte)((charkArray[0] >> 4) + 0x30);
            //----------------------------------------------------------
            testCan[4] = 0x32;
            //----------------------------------------------------------
            if((charkArray[2] & 0xF) >= 0xA)
                testCan[3] = (byte)((charkArray[2] & 0xF) + 0x30 + 7);
            else
                testCan[3] = (byte)((charkArray[2] & 0xF) + 0x30);
            //----------------------------------------------------------
            int sumCharck = Convert.ToInt16(charkArray[4]) + Convert.ToInt16(charkArray[5]);
            int sumToCharck = Convert.ToInt16(charkArray[2] >> 4);
            testCan[2] += (byte)sumToCharck;
            while(sumCharck > 0) {
                testCan[2] += 0x02;
                if(testCan[2] >= 0x3A) {
                    testCan[2] += 0x07;
                }
                if(testCan[2] > 0x46) {
                    testCan[2] = 0x30;
                    testCan[1] += 0x01;
                }
                sumCharck -= 1;
            }
            //----------------------------------------------------------
            testCan[coutn_byte - 1] = 0xD;

            Debug.WriteLine("");
            return testCan;
        }
        public static byte[] AppToCan29(byte[] charkArray) {
            int coutn_byte = 20;
            byte[] testCan = new byte[coutn_byte];
            // Пример приема с сервера. Число в строку
            testCan[0] = Convert.ToByte('T');
            for(int i = 1; i < coutn_byte; i++) {
                testCan[i] = 0x30;
            }
            //----------------------------------------------------------
            if((charkArray[0] & 0xF) >= 0xA)
                testCan[11] = (byte)((charkArray[0] & 0xF) + 0x30 + 7);
            else
                testCan[11] = (byte)((charkArray[0] & 0xF) + 0x30);
            if((charkArray[0] >> 4) >= 0xA)
                testCan[10] = (byte)((charkArray[0] >> 4) + 0x30 + 7);
            else
                testCan[10] = (byte)((charkArray[0] >> 4) + 0x30);
            //----------------------------------------------------------
            testCan[9] = 0x32;
            //----------------------------------------------------------
            if((charkArray[2] & 0xF) >= 0xA)
                testCan[8] = (byte)((charkArray[2] & 0xF) + 0x30 + 7);
            else
                testCan[8] = (byte)((charkArray[2] & 0xF) + 0x30);
            if((charkArray[2] >> 4) >= 0xA)
                testCan[7] = (byte)((charkArray[2] >> 4) + 0x30 + 7);
            else
                testCan[7] = (byte)((charkArray[2] >> 4) + 0x30);
            if((charkArray[3] & 0xF) >= 0xA)
                testCan[6] = (byte)((charkArray[3] & 0xF) + 0x30 + 7);
            else
                testCan[6] = (byte)((charkArray[3] & 0xF) + 0x30);
            //----------------------------------------------------------
            int sumCharck = Convert.ToInt16(charkArray[4]) + Convert.ToInt16(charkArray[5]);
            while(sumCharck > 0) {
                testCan[5] += 0x04;
                if(testCan[5] >= 0x3A) {
                    testCan[5] += 0x07;
                }
                if(testCan[5] > 0x46) {
                    testCan[5] = 0x30;
                    testCan[4] += 0x01;
                }
                //----------------------
                if(testCan[4] == 0x3A) {
                    testCan[4] += 0x07;
                }
                if(testCan[4] > 0x46) {
                    testCan[4] = 0x30;
                    testCan[3] += 0x01;
                }
                //----------------------
                if(testCan[3] == 0x3A) {
                    testCan[3] += 0x07;
                }
                if(testCan[3] > 0x46) {
                    testCan[3] = 0x30;
                    testCan[2] += 0x01;
                }
                //----------------------
                if(testCan[2] == 0x3A) {
                    testCan[2] += 0x07;
                }
                if(testCan[2] > 0x46) {
                    testCan[1] = 0x30;
                    testCan[1] += 0x01;
                }
                sumCharck -= 1;
            }
            //----------------------------------------------------------
            testCan[coutn_byte - 1] = 0xD;

            Debug.WriteLine("");
            return testCan;
        }
    }
}
