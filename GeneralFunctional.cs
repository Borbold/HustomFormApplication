using System;
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

    [StructLayout(LayoutKind.Explicit)]
    public struct LItUn {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;
        [FieldOffset(3)]
        public byte byte3;
        [FieldOffset(4)]
        public byte byte4;

        [FieldOffset(0)]
        public int it;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UItUn {
        [FieldOffset(0)]
        public byte byte1;
        [FieldOffset(1)]
        public byte byte2;
        [FieldOffset(3)]
        public byte byte3;
        [FieldOffset(4)]
        public byte byte4;

        [FieldOffset(0)]
        public uint it;
    }
}

namespace HustonRTEMS {
    internal class GeneralFunctional {
        private readonly byte KISS_FEND = 0xC0;
        public readonly byte[] kissHeader = {
            0xC0, 0x00, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x00, 0xF0 };

        public static void ChangingPositionAccelerometer() {
            int jon = 5;
            for(int i = 0; i < 5; i++) {
                Thread.Sleep(100);
                jon++;
            }
        }

        public async void SendMessageInSocketTime(Socket serverListener,
            int idShipping, int addresValue, int addresReceive,
            int iCount, int[] arIValue, TextBox logBox, bool isKiss) {
            ItUn iValue = new();
            byte[] sendBuf;
            // Header KISS
            int raw_buffer_size = 18;
            if(isKiss) {
                sendBuf = new byte[27 + iCount];
                Array.Copy(kissHeader, sendBuf, raw_buffer_size);
                raw_buffer_size = 0;
            } else {
                sendBuf = new byte[26 + iCount - raw_buffer_size];
            }
            // Header UNICAN
            iValue.it = idShipping;
            sendBuf[18 - raw_buffer_size] = iValue.byte1;
            sendBuf[19 - raw_buffer_size] = iValue.byte2;
            iValue.it = addresValue;
            sendBuf[20 - raw_buffer_size] = iValue.byte1;
            sendBuf[21 - raw_buffer_size] = iValue.byte2;
            iValue.it = addresReceive;
            sendBuf[22 - raw_buffer_size] = iValue.byte1;
            sendBuf[23 - raw_buffer_size] = iValue.byte2;
            iValue.it = iCount;
            sendBuf[24 - raw_buffer_size] = iValue.byte1;
            sendBuf[25 - raw_buffer_size] = iValue.byte2;

            for(int i = 0; i < iCount; i++) {
                iValue.it = arIValue[i];
                sendBuf[26 + i - raw_buffer_size] = iValue.byte1;
            }

            if(isKiss) {
                sendBuf[^1] = KISS_FEND;
            }

            logBox.Text += "\r\n";
            foreach(var a in sendBuf) {
                logBox.Text += $"{a:X} ";
            }

            SendChangeKissFESC(ref sendBuf);

            if(serverListener != null && serverListener.Connected) {
                _ = await serverListener.SendAsync(sendBuf, SocketFlags.None);
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public async void SendMessageInSocket(Socket serverListener,
            int idShipping, int addresValue, int addresReceive,
            int iCount, int fCount, int[] arIValue, float[] arFValue,
            TextBox logBox, bool isKiss) {
            FlUn fValue = new();
            ItUn iValue = new();
            byte[] sendBuf;
            // Header KISS
            int raw_buffer_size = 18;
            if(isKiss) {
                sendBuf = new byte[27 + (fCount * 4) + (iCount * 2)];
                Array.Copy(kissHeader, sendBuf, raw_buffer_size);
                raw_buffer_size = 0;
            } else {
                sendBuf = new byte[26 + (fCount * 4) + (iCount * 2) - raw_buffer_size];
            }
            // Header UNICAN
            iValue.it = idShipping;
            sendBuf[18 - raw_buffer_size] = iValue.byte1;
            sendBuf[19 - raw_buffer_size] = iValue.byte2;
            iValue.it = addresValue;
            sendBuf[20 - raw_buffer_size] = iValue.byte1;
            sendBuf[21 - raw_buffer_size] = iValue.byte2;
            iValue.it = addresReceive;
            sendBuf[22 - raw_buffer_size] = iValue.byte1;
            sendBuf[23 - raw_buffer_size] = iValue.byte2;
            iValue.it = (fCount * 4) + (iCount * 2);
            sendBuf[24 - raw_buffer_size] = iValue.byte1;
            sendBuf[25 - raw_buffer_size] = iValue.byte2;

            for(int i = 0; i < iCount; i++) {
                iValue.it = arIValue[i];
                sendBuf[26 + (i * 2) - raw_buffer_size] = iValue.byte1;
                sendBuf[27 + (i * 2) - raw_buffer_size] = iValue.byte2;
            }
            for(int f = 0; f < fCount; f++) {
                fValue.fl = arFValue[f];
                sendBuf[26 + (f * 4) - raw_buffer_size] = fValue.byte1;
                sendBuf[27 + (f * 4) - raw_buffer_size] = fValue.byte2;
                sendBuf[28 + (f * 4) - raw_buffer_size] = fValue.byte3;
                sendBuf[29 + (f * 4) - raw_buffer_size] = fValue.byte4;
            }

            if(isKiss) {
                sendBuf[^1] = KISS_FEND;
            }

            SendChangeKissFESC(ref sendBuf);

            if(serverListener != null && serverListener.Connected) {
                _ = await serverListener.SendAsync(sendBuf, SocketFlags.None);
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public void SendMessageTime() {

        }

        public async void SendMessageInSocket(Socket serverListener, FlUn fl, ItUn it,
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

        public async void SendMessageInSocket(Socket serverListener,
                byte[] hardBufWrite, TextBox logBox) {
            if(serverListener != null && serverListener.Connected) {
                try {
                    _ = await serverListener.SendAsync(hardBufWrite, SocketFlags.None);
                }
                catch(Exception) { }
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
        }

        public void SendChangeKissFESC(ref byte[] chageByte) {
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

        public void WriteChangeKissFESC(ref byte[] chageByte) {
            byte KISS_FESC = 0xDB;
            byte KISS_TFEND = 0xDC;
            byte KISS_TFESC = 0xDD;

            for(int i = 18; i < chageByte.Length - 1; i++) {
                if(chageByte[i] == KISS_TFEND) {
                    chageByte[i] = KISS_FEND;
                } else if(chageByte[i] == KISS_TFESC) {
                    chageByte[i] = KISS_FESC;
                }
            }
        }

        public byte[] CanToApp11(byte[] charkArray) {
            int coutn_byte = charkArray.Length;
            byte[] testCan = new byte[coutn_byte];
            //Addres----------------------------------------------------------
            uint suchAd = 0;
            if(charkArray[1] - 0x30 != 0) {
                suchAd += (uint)(charkArray[1] - 0x30 - (charkArray[1] > 0x40 ? 7 : 0)) * 100;
            }
            if(charkArray[2] - 0x30 != 0) {
                suchAd += (uint)(charkArray[2] - 0x30 - (charkArray[2] > 0x40 ? 7 : 0)) * 10;
            }
            if(charkArray[3] - 0x30 != 0) {
                suchAd += (uint)(charkArray[3] - 0x30 - (charkArray[3] > 0x40 ? 7 : 0));
            }
            testCan[2] = 0;
            testCan[3] = (byte)(suchAd & 0x1F);
            testCan[4] = 0;
            testCan[5] = (byte)((suchAd & 0x3E0) >> 5);
            //ID----------------------------------------------------------
            testCan[0] = 0;
            if(charkArray[5] - 0x30 != 0) {
                testCan[1] |= (byte)(charkArray[5] - 0x30 - (charkArray[5] > 0x40 ? 7 : 0));
            }
            if(charkArray[6] - 0x30 != 0) {
                testCan[1] |= (byte)(charkArray[6] - 0x30 - (charkArray[6] > 0x40 ? 7 : 0));
            }
            if(charkArray[7] - 0x30 != 0) {
                testCan[1] |= (byte)(charkArray[7] - 0x30 - (charkArray[7] > 0x40 ? 7 : 0));
            }
            if(charkArray[8] - 0x30 != 0) {
                testCan[1] |= (byte)(charkArray[8] - 0x30 - (charkArray[8] > 0x40 ? 7 : 0));
            }
            //Count message----------------------------------------------------------
            testCan[6] = 0;
            if(charkArray[4] - 0x30 != 0) {
                testCan[7] |= (byte)(charkArray[4] - 0x30 - (charkArray[4] > 0x40 ? 7 : 0) - 2);
            }
            //----------------------------------------------------------
            testCan[coutn_byte - 1] = 0xC0;

            return testCan;
        }
        public void CanToApp29(byte[] charkArray) {
            int coutn_byte = 20;
            byte[] testCan = new byte[coutn_byte];
            //----------------------------------------------------------
            if(charkArray[11] - 0x30 != 0) {
                testCan[0] = (byte)((charkArray[11] - 0x30 - (charkArray[11] > 0x40 ? 7 : 0)) << 4);
            }

            if(charkArray[10] - 0x30 != 0) {
                testCan[0] += (byte)((charkArray[10] - 0x30 - (charkArray[10] > 0x40 ? 7 : 0)) << 4);
            }
            //----------------------------------------------------------
            if(charkArray[8] - 0x30 != 0) {
                testCan[2] = (byte)((charkArray[8] - 0x30 - (charkArray[8] > 0x40 ? 7 : 0)) << 4);
            }

            if(charkArray[7] - 0x30 != 0) {
                testCan[2] += (byte)((charkArray[7] - 0x30 - (charkArray[7] > 0x40 ? 7 : 0)) << 4);
            }
            //----------------------------------------------------------
            if(charkArray[5] - 0x30 != 0) {
                testCan[4] = (byte)(((charkArray[5] - 0x30 - (charkArray[5] > 0x40 ? 7 : 0)) & 0xF) / 4);
            }

            if(charkArray[4] - 0x30 != 0) {
                testCan[4] += (byte)((charkArray[4] - 0x30 - (charkArray[4] > 0x40 ? 7 : 0)) & 0xF);
            }
            //----------------------------------------------------------
            testCan[coutn_byte - 1] = 0xC0;

            foreach(byte i in testCan) {
                Debug.Write($"{i:X} ");
            }

            Debug.WriteLine("");
        }

        public byte[] AppToCan11(byte[] charkArray) {
            int coutn_byte = 14;
            byte[] testCan = new byte[coutn_byte];
            // Пример приема с сервера. Число в строку
            testCan[0] = Convert.ToByte('t');
            for(int i = 1; i < coutn_byte; i++) {
                testCan[i] = 0x30;
            }
            //----------------------------------------------------------
            testCan[6] = (charkArray[0] & 0xF) >= 0xA ? (byte)((charkArray[0] & 0xF) + 0x30 + 7) : (byte)((charkArray[0] & 0xF) + 0x30);
            testCan[5] = (charkArray[0] >> 4) >= 0xA ? (byte)((charkArray[0] >> 4) + 0x30 + 7) : (byte)((charkArray[0] >> 4) + 0x30);
            //----------------------------------------------------------
            testCan[4] = 0x32;
            //----------------------------------------------------------
            testCan[3] = (charkArray[2] & 0xF) >= 0xA ? (byte)((charkArray[2] & 0xF) + 0x30 + 7) : (byte)((charkArray[2] & 0xF) + 0x30);
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
        public byte[] AppToCan29(byte[] charkArray) {
            int coutn_byte = 20;
            byte[] testCan = new byte[coutn_byte];
            // Пример приема с сервера. Число в строку
            testCan[0] = Convert.ToByte('T');
            for(int i = 1; i < coutn_byte; i++) {
                testCan[i] = 0x30;
            }
            //----------------------------------------------------------
            testCan[11] = (charkArray[0] & 0xF) >= 0xA ? (byte)((charkArray[0] & 0xF) + 0x30 + 7) : (byte)((charkArray[0] & 0xF) + 0x30);
            testCan[10] = (charkArray[0] >> 4) >= 0xA ? (byte)((charkArray[0] >> 4) + 0x30 + 7) : (byte)((charkArray[0] >> 4) + 0x30);
            //----------------------------------------------------------
            testCan[9] = 0x32;
            //----------------------------------------------------------
            testCan[8] = (charkArray[2] & 0xF) >= 0xA ? (byte)((charkArray[2] & 0xF) + 0x30 + 7) : (byte)((charkArray[2] & 0xF) + 0x30);
            testCan[7] = (charkArray[2] >> 4) >= 0xA ? (byte)((charkArray[2] >> 4) + 0x30 + 7) : (byte)((charkArray[2] >> 4) + 0x30);
            testCan[6] = (charkArray[3] & 0xF) >= 0xA ? (byte)((charkArray[3] & 0xF) + 0x30 + 7) : (byte)((charkArray[3] & 0xF) + 0x30);
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

        public void ClearInvokeTextBox(TextBox textBox) {
            try {
                textBox.Invoke(new Action(() => {
                    textBox.Text = "";
                }));
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
        }
        public void InvokeTextBox(TextBox textBox, string text) {
            try {
                textBox.Invoke(new Action(() => {
                    textBox.Text += text;
                }));
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
