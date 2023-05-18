using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Security.Policy;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    public partial class Form1: Form {
        private readonly FunctionalChecker FC = new();
        private readonly byte[] hardBuf = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x1, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        private readonly byte[] hardBufWrite = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x09, 0x00, 0x1C, 0x00, 0x04, 0x00, 0x00, 0x98, 0xAD, 0x45, 0xC0 };

        private readonly int headerLength = 18;
        private int message_size;
        private Socket client;
        private byte[] buffer;
        private readonly int kissBuff = 256;

        [StructLayout(LayoutKind.Explicit)]
        private struct fl_un {
            [FieldOffset(0)]
            public byte byte1;

            [FieldOffset(1)]
            public byte byte2;

            [FieldOffset(2)]
            public byte byte3;

            [FieldOffset(3)]
            public byte byte4;

            [FieldOffset(0)]
            public float fl1;
        }
        private fl_un fl;

        public Form1() {
            InitializeComponent();
        }

        private async void OpenSocet_Click(object sender, EventArgs e) {
            int unican_size = 32;
            int send_size = 18 + 1 + 8 + unican_size;
            int total_sended = headerLength + 1;
            byte[] buf = new byte[send_size];

            for(int i = 0; i <= headerLength; i++) {
                buf[i] = hardBuf[i];
            }

            int bufPos = headerLength + 1;
            int index = 0;
            while(total_sended < send_size) {
                if(index == 0) {
                    buf[bufPos] = 0x1B0 >> 8;
                } else if(index == 1) {
                    buf[bufPos] = 0x1C & 0xFF;
                } else if(index == 2) {
                    buf[bufPos] = 0x1C >> 8;
                } else if(index == 3) {
                    buf[bufPos] = 0x1 & 0xFF;
                } else if(index == 4) {
                    buf[bufPos] = 0x1 >> 8;
                } else if(index == 5) {
                    buf[bufPos] = (byte)(unican_size & 0xFF);
                } else if(index == 6) {
                    buf[bufPos] = (byte)(unican_size >> 8);
                }/**/ else if(index == 7) {
                    buf[bufPos] = 0x1;
                } else if(index == 8) {
                    buf[bufPos] = 0x2;
                } else if(index == 9) {
                    buf[bufPos] = (byte)(Convert.ToInt16(TrackBarTemp.Value) & 0xFF);
                    bufPos++;
                    buf[bufPos] = (byte)(Convert.ToInt16(TrackBarTemp.Value) >> 8);
                } else if(index == 11) {
                    buf[bufPos] = 0x2;
                } else if(index == 13) {
                    buf[bufPos] = 0x2;
                } else if(index == 15) {
                    buf[bufPos] = 0x2;
                } else if(index == 16) {
                    buf[bufPos] = 0x2;
                } else if(index == 18) {
                    buf[bufPos] = 0x2;
                } else if(index == 19) {
                    buf[bufPos] = 0x2;
                } else if(index == 21) {
                    buf[bufPos] = 0x2;
                } else if(index == 23) {
                    buf[bufPos] = 0x2;
                }
                total_sended++;
                bufPos++;
                index++;
            }
            buf[send_size - 1] = 192;

            _ = await client.SendAsync(buf, SocketFlags.None);
        }

        private async void OpenServer_Click(object sender, EventArgs e) {
            IPEndPoint ipep = new(IPAddress.Parse("127.0.0.1"), 5555);
            Socket listener = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            int KISSBUFFER_SIZE = 256;
            buffer = new byte[KISSBUFFER_SIZE];
            int raw_buffer_size = 0;
            if(CheckBox.Checked) {
                TestBox.Text = $"Wait connect";
                listener.Connect(ipep);
                TestBox.Text = $"Connected";
                _ = await listener.SendAsync(hardBuf, SocketFlags.None);
                TestBox.Text = $"Write";

                TestBox.Text = "";
                while(true) {
                    message_size = await listener.ReceiveAsync(buffer, SocketFlags.None);

                    if(message_size > 0) {
                        TestBox.Text +=
                            $"Socket server response message: \r\n";
                        while(raw_buffer_size < message_size) {
                            if(raw_buffer_size >= 0) {
                                TestBox.Text += $"{buffer[raw_buffer_size]:X} ";
                            }
                            raw_buffer_size++;
                        }
                        //break;
                        message_size = 0;
                        TestBox.Text += $"\r\nWait new message!\r\n";
                        fl.fl1 = (float)Convert.ToDouble(LabTemp.Text);
                        hardBufWrite[26] = fl.byte1;
                        hardBufWrite[27] = fl.byte2;
                        hardBufWrite[28] = fl.byte3;
                        hardBufWrite[29] = fl.byte4;
                        _ = await listener.SendAsync(hardBufWrite, SocketFlags.None);
                    }
                    raw_buffer_size = 0;
                }
            } else if(!CheckBox.Checked) {
                listener.Bind(ipep);
                listener.Listen(200);
                TestBox.Text = "Waiting for a client...";

                client = await listener.AcceptAsync();
                IPEndPoint? clientep = client.RemoteEndPoint as IPEndPoint;
                TestBox.Text = $"Connected with {clientep.Address} at port {clientep.Port}";
                // Receive message.
                while(true) {
                    message_size = await client.ReceiveAsync(buffer, SocketFlags.None);

                    if(message_size > 0) {
                        TestBox.Text =
                            $"RTEMS message: ";
                        while(raw_buffer_size < message_size) {
                            if(raw_buffer_size >= 0) {
                                TestBox.Text += $"{buffer[raw_buffer_size]:X} ";
                            }
                            raw_buffer_size++;
                        }
                        break;
                    }
                    raw_buffer_size = 0;
                }
            }

            listener.Close();
        }

        private void ListenPort_Click(object sender, EventArgs e) {
            TestBox.Text = "";
            if(!FC.CheckDevice(headerLength, hardBuf, TestBox)) {
                TestBox.Text = "Errore checker!";
            }
        }

        // Track bar
        private void Change_Val_Track(float value, Label lab) {
            lab.Text = value.ToString();
        }

        private void TrackBarTemp_Scroll(object sender, EventArgs e) {
        }
        private void TrackBarTemp_ValueChanged(object sender, EventArgs e) {
            Change_Val_Track(TrackBarTemp.Value / 5.0f, LabTemp);
        }

        private void TrackBarRotX_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotX.Value / 5.0f, LabRotX);
        }
        private void TrackBarRotY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotY.Value / 5.0f, LabRotY);
        }
        private void TrackBarRotZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotZ.Value / 5.0f, LabRotZ);
        }

        private void TrackMagX_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagX.Value / 5.0f, LabMagX);
        }
        private void TrackMagY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagY.Value / 5.0f, LabMagY);
        }
        private void TrackMagZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagZ.Value / 5.0f, LabMagZ);
        }

        private void TrackMagX_2_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagX_2.Value / 5.0f, LabMagX_2);
        }
        private void TrackMagY_2_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagY_2.Value / 5.0f, LabMagY_2);
        }
        private void TrackMagZ_2_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackMagZ_2.Value / 5.0f, LabMagZ_2);
        }
        // Track bar

        // Settings
        private void ChooseSenMet_SelectedIndexChanged(object sender, EventArgs e) {

        }
        // Settings

        private void UseInternet_CheckedChanged(object sender, EventArgs e) {
            if(UseInternet.Checked)
                UseCan.Checked = false;
        }
        private void UseCan_CheckedChanged(object sender, EventArgs e) {
            if(UseCan.Checked)
                UseInternet.Checked = false;
        }
    }
}

namespace HustonRTEMS {
    public class FunctionalChecker {
        public bool CheckDevice(int headerLength, byte[] hardBuf, TextBox TestBox) {
            int checkV = 0;
            for(int i = headerLength, j = 0; i < hardBuf.Length; i++, j++) {
                switch(j) {
                    case 0:
                        checkV = hardBuf[i];
                        break;
                    case 1:
                        checkV |= hardBuf[i] << 8;
                        TestBox.Text += $"id: {checkV:X} \r\n";
                        if(checkV != 0xB0) {
                            return false;
                        }

                        break;
                    case 2:
                        checkV = hardBuf[i];
                        break;
                    case 3:
                        checkV |= hardBuf[i] << 8;
                        TestBox.Text += $"Base station address: {checkV:X} \r\n";
                        if(checkV != 0x1) {
                            return false;
                        }

                        break;
                    case 4:
                        checkV = hardBuf[i];
                        break;
                    case 5:
                        checkV |= hardBuf[i] << 8;
                        TestBox.Text += $"Device address: {checkV:X} \r\n";
                        if(checkV != 0x1C) {
                            return false;
                        }

                        break;
                }
            }
            return true;
        }
    }
}