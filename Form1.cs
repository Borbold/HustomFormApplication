using System.Net;
using System.Net.Sockets;

namespace HustonRTEMS {
    public partial class Form1: Form {
        private readonly GeneralFunctional GF = new();
        private readonly DefaultTransmission DT = new();
        private readonly byte[] hardBuf = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x1, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        private readonly byte[] hardBufWrite = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x09, 0x00, 0x1C, 0x00, 0x04, 0x00, 0x00, 0x98, 0xAD, 0x45, 0xC0 };

        private int message_size;
        private Socket client;
        private byte[] buffer;

        public fl_un fl;
        public it_un it;

        public Form1() {
            InitializeComponent();
        }

        private void ClearLog_Click(object sender, EventArgs e) {
            LogBox.Text = "";
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
            if(UseInternet.Checked) {
                UseCan.Checked = false;
            }
        }
        private void UseCan_CheckedChanged(object sender, EventArgs e) {
            if(UseCan.Checked) {
                UseInternet.Checked = false;
            }
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
                    LogBox.Text = $"Socet open";
                    _ = serverListener.Send(hardBuf);
                }
                catch(Exception ex) {
                    LogBox.Text = ex.Message;
                }

                while(serverListener.Connected) {
                    try {
                        message_size = await serverListener.ReceiveAsync(buffer, SocketFlags.None);
                    }
                    catch(Exception ex) {
                        LogBox.Text = ex.Message;
                    }

                    if(message_size > 0) {
                        LogBox.Text +=
                            $"Socket server response message: \r\n";
                        while(raw_buffer_size < message_size) {
                            if(raw_buffer_size >= 0) {
                                LogBox.Text += $"{buffer[raw_buffer_size]:X} ";
                            }
                            raw_buffer_size++;
                        }
                        //break;
                        message_size = 0;
                        LogBox.Text += $"\r\nWait new message!\r\n";
                    }
                    raw_buffer_size = 0;
                }
            } else if(!CheckBox.Checked) {
                serverListener.Bind(ipep);
                serverListener.Listen(200);
                LogBox.Text = "Waiting for a client...";

                client = await serverListener.AcceptAsync();
                IPEndPoint? clientep = client.RemoteEndPoint as IPEndPoint;
                LogBox.Text = $"Connected with {clientep.Address} at port {clientep.Port}";
                // Receive message.
                while(true) {
                    message_size = await client.ReceiveAsync(buffer, SocketFlags.None);

                    if(message_size > 0) {
                        LogBox.Text =
                            $"RTEMS message: ";
                        while(raw_buffer_size < message_size) {
                            if(raw_buffer_size >= 0) {
                                LogBox.Text += $"{buffer[raw_buffer_size]:X} ";
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
        private void CloseSocketServer_Click(object sender, EventArgs e) {
            if(serverListener != null && serverListener.Connected) {
                serverListener.Disconnect(true);
            }
        }

        // Send data
        private void SendTemperature_Click(object sender, EventArgs e) {
            GF.SendMessageInSocket(serverListener, fl, it,
                DT, hardBufWrite, (float)Convert.ToDouble(LabTemp.Text), LogBox);
        }
        private void SendMagnetometer_Click(object sender, EventArgs e) {

        }
        private void SendAcselerometer_Click(object sender, EventArgs e) {

        }
        // Send data

        private void Form1_Load(object sender, EventArgs e) {
            AddresTemperature.Text = $"{DT.temperatureTransmission.TAddres.addres:X}";
            IdTemperature.Text = $"{DT.temperatureTransmission.TId.getValue[0]:X}";
        }
    }
}