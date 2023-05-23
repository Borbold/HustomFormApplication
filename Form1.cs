using System.Configuration;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace HustonRTEMS {
    public partial class Form1: Form {
        private readonly Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly GeneralFunctional GF = new();
        private readonly DefaultTransmission DT = new();
        private readonly byte[] hardBuf = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x1, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        private readonly byte[] hardBufWrite = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x09, 0x00, 0x1C, 0x00, 0x04, 0x00, 0x00, 0x98, 0xAD, 0x45, 0xC0 };

        private int message_size = new();
        private Socket client;
        private byte[] buffer;

        public fl_un fl = new();
        public it_un it = new();

#pragma warning disable CS8618
        public Form1() {
            InitializeComponent();
        }
#pragma warning restore CS8618
        private void Form1_Load(object sender, EventArgs e) {
            AddresTemperature.Text = $"{DT.temperatureTransmission.TAddres.addres:X}";
            IdTemperature.Text = $"{DT.temperatureTransmission.TId.getValue[0]:X}";

            AddresAcsel.Text = $"{DT.acselerometerTransmission.TAddres.addres:X}";
            IdAscelX.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdAscelY.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdAscelZ.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            AddresMag1.Text = $"{DT.magnitudeTransmission1.TAddres.addres:X}";
            IdMag1X.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.X]:X}";
            IdMag1Y.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.Y]:X}";
            IdMag1Z.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.Z]:X}";

            AddresMag2.Text = $"{DT.magnitudeTransmission2.TAddres.addres:X}";
            IdMag2X.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.X]:X}";
            IdMag2Y.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.Y]:X}";
            IdMag2Z.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.Z]:X}";

            if(cfg.GetSection("customProperty") is CustomProperty section) {
                IPTextBox.Text = section.IP;
                PortTextBox.Text = section.PORT;
                CANSpeed.Text = section.CANSpeed;
                CANPort.Text = section.CANPort;
                cfg.Save();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if(cfg.GetSection("customProperty") is CustomProperty section) {
                section.IP = IPTextBox.Text;
                section.PORT = PortTextBox.Text;
                section.CANSpeed = CANSpeed.Text;
                section.CANPort = CANPort.Text;
                cfg.Save();
            }
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
            int raw_buffer_size = 18; // Kiss header
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
                        message_size = 0;
                        LogBox.Text += $"\r\nWait new message!\r\n";

                        // Example of sending a power-on response
                        it_un id = new() {
                            byte1 = buffer[18],
                            byte2 = buffer[19]
                        }, addres = new() {
                            byte1 = buffer[20],
                            byte2 = buffer[21]
                        };
                        if(addres.it == 4) {
                            if(id.it == 0x40) {
                                hardBufWrite[18] = id.byte1;
                                hardBufWrite[19] = id.byte2;
                                it.it = DT.acknowledge.TAddres.addres;

                                GF.SendMessageInSocket(serverListener, fl, it,
                                    hardBufWrite, LogBox);
                            }
                        }
                    } else {
                        break;
                    }
                    raw_buffer_size = 0;
                }
            } else if(!CheckBox.Checked) {
                serverListener.Bind(ipep);
                serverListener.Listen(200);
                LogBox.Text = "Waiting for a client...";

                client = await serverListener.AcceptAsync();
                /*IPEndPoint? clientep = client.RemoteEndPoint as IPEndPoint;
                LogBox.Text = $"Connected with {clientep.Address} at port {clientep.Port}";*/
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
            it.it = DT.temperatureTransmission.TAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabTemp.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }

        private void SendMagnetometer1_Click(object sender, EventArgs e) {
            it.it = DT.magnitudeTransmission1.TAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabMagX.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagY.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagZ.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }
        private void SendMagnetometer2_Click(object sender, EventArgs e) {
            it.it = DT.magnitudeTransmission2.TAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabMagX_2.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagY_2.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagZ_2.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }

        private void SendAcselerometer_Click(object sender, EventArgs e) {
            it.it = DT.acselerometerTransmission.TAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabRotX.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabRotY.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabRotZ.Text);
            GF.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }
        // Send data

        private void IPTextBox_TextChanged(object sender, EventArgs e) {
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            Read();
        }

        private void Read() {
        }
        private void CANTestWrite_Click(object sender, EventArgs e) {
            /*serialPort.Write("V\r");
            Thread.Sleep(100);

            if(serialPort.BytesToRead > 0) {
                byte[] data = new byte[8];
                serialPort.Read(data, 0, 8);
                for(int i = 0; i < data.Length; i++)
                    LogBox.Text += " " + data[i].ToString();
            }*/

            serialPort.Write("t28011223344\r");
            Thread.Sleep(100);
        }
        private void CANTestRead_Click(object sender, EventArgs e) {
            if(serialPort.BytesToRead > 0) {
                byte[] data = new byte[8];
                _ = serialPort.Read(data, 0, 8);
                for(int i = 0; i < data.Length; i++) {
                    LogBox.Text += " " + data[i].ToString();
                }
            }
        }

        private SerialPort serialPort;
        private void OpenRKSCAN_Click(object sender, EventArgs e) {
            serialPort = new(CANPort.Text, 9600, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
            try {
                serialPort.Open();
                // Открыть
                serialPort.Write("O\r");
                // Установить скорость
                serialPort.Write(string.Format("S{0}\r", CANSpeed.SelectedIndex));

                LogBox.Text = "Port open";
            }
            catch(Exception ex) {
                LogBox.Text = ex.Message;
            }
        }
        private void CloseRKSCAN_Click(object sender, EventArgs e) {
            try {
                // Закрыть
                serialPort.Write("C\r");
                serialPort.Close();

                LogBox.Text = "Port close";
            }
            catch(Exception ex) {
                LogBox.Text = ex.Message;
            }
        }
    }
}