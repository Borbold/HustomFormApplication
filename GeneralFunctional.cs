using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    public enum EnVal {
    }

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
            foreach(byte a in sendBuf) {
                logBox.Text += $"{a:X} ";
            }

            SendChangeKissFESC(ref sendBuf);

            if(serverListener != null && serverListener.Connected) {
                try {
                    _ = await serverListener.SendAsync(sendBuf, SocketFlags.None);
                }
                catch(Exception) { }
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
                try {
                    _ = await serverListener.SendAsync(sendBuf, SocketFlags.None);
                }
                catch(Exception) { }
            } else {
                logBox.Invoke(new Action(() => {
                    logBox.Text = "Socet don't open!";
                }));
            }
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

        public void CreateFilterDB(ref Dictionary<int, EnVal> sortDV,
                int countDB, ComboBox howFilter, EnVal filterVal) {
            switch(howFilter.SelectedIndex) {
                case 0:
                    for(int i = 0; i < countDB; i++) {
                        if(sortDV[i] < filterVal) {
                            _ = sortDV.Remove(i);
                        }
                    }
                    break;
                case 1:
                    for(int i = 0; i < countDB; i++) {
                        if(sortDV[i] > filterVal) {
                            _ = sortDV.Remove(i);
                        }
                    }
                    break;
                case 2:
                    for(int i = 0; i < countDB; i++) {
                        if(sortDV[i] != filterVal) {
                            _ = sortDV.Remove(i);
                        }
                    }
                    break;
                case 3:
                    for(int i = 0; i < countDB; i++) {
                        if(sortDV[i] == filterVal) {
                            _ = sortDV.Remove(i);
                        }
                    }
                    break;
            }
        }

        public void WriteDBInformation(byte[] result, int i,
                RichTextBox DBAllText, string[] variableNameLD, int valI) {
            LItUn intV = new() {
                byte1 = result[i],
                byte2 = result[i + 1],
                byte3 = result[i + 2],
                byte4 = result[i + 3]
            };
            DBAllText.Text += $"{variableNameLD[valI]}: ";
            DBAllText.Text += intV.it;
            DBAllText.Text += ";\t";
        }

        public void GetFileInfo(TextBox nameDBFile) {
            OpenFileDialog ofd = new() {
                Title = "Select file",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt*",
                FilterIndex = 1,
            };
            if(ofd.ShowDialog() == DialogResult.OK) {
                nameDBFile.Text = ofd.FileName;
            }
        }
    }
}