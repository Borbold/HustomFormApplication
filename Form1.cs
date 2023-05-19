using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Security.Policy;
using System.Runtime.InteropServices;

namespace HustonRTEMS {
    public partial class Form1: Form {
        readonly GeneralFunctional GF = new();
        readonly DefaultTransmission DT = new();
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

        public fl_un fl;
        public it_un it;

        public Form1() {
            InitializeComponent();
        }

        private void ClearLog_Click(object sender, EventArgs e) {
            TestBox.Text = "";
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

        private Socket serverListener;
        private async void OpenSocetServer_Click(object sender, EventArgs e) {
            IPEndPoint ipep = new(IPAddress.Parse(IPTextBox.Text),
                Convert.ToInt16(PortTextBox.Text));
            serverListener = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            int KISSBUFFER_SIZE = 256;
            buffer = new byte[KISSBUFFER_SIZE];
            int raw_buffer_size = 0;
            if(CheckBox.Checked && !serverListener.Connected) {
                try {
                    serverListener.Connect(ipep);
                    TestBox.Text = $"Socet open";
                    serverListener.Send(hardBuf);
                }
                catch(Exception ex) {
                    TestBox.Text = ex.Message;
                }

                while(serverListener.Connected) {
                    try {
                        message_size = await serverListener.ReceiveAsync(buffer, SocketFlags.None);
                    }
                    catch(Exception ex) {
                        TestBox.Text = ex.Message;
                    }

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
                    }
                    raw_buffer_size = 0;
                }
            } else if(!CheckBox.Checked) {
                serverListener.Bind(ipep);
                serverListener.Listen(200);
                TestBox.Text = "Waiting for a client...";

                client = await serverListener.AcceptAsync();
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

            serverListener.Close();
        }
        private void SendOpenSocetServer_Click(object sender, EventArgs e) {

        }

        private void SendTemperature_Click(object sender, EventArgs e) {
            GF.SendMessageInSocket(serverListener, fl, it,
                DT, hardBufWrite, (float)Convert.ToDouble(LabTemp.Text), TestBox);
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