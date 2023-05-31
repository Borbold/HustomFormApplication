using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace HustonRTEMS {
    public partial class Form1: Form {
        void TestCharToByte() {
            // С CAN приходит строка и преобразовываем в число
            // С сервера приходит число которое преобразовываем в строку
            byte[] testCan = new byte[255];
            it_un id_un = new() {
                byte1 = testInternetBuf[0],
                byte2 = testInternetBuf[1]
            }, to_addres = new() {
                byte1 = testInternetBuf[2],
                byte2 = testInternetBuf[3]
            }, out_addres = new() {
                byte1 = testInternetBuf[4],
                byte2 = testInternetBuf[5]
            };
            // Пример приема с сервера. Число в строку
            if(to_addres.it <= 31 && out_addres.it <= 31) {
                testCan[0] = 0x74;
                int offset_out = out_addres.it / 0xF;
                to_addres.it = to_addres.it * 2 + offset_out;
                testCan[1] = (byte)(to_addres.byte2 + 0x30);
                testCan[2] = (byte)(to_addres.byte1 + 0x30);
                testCan[3] = (byte)((out_addres.it & 0xF) + 0x37);
                testCan[4] = 0x32;
                testCan[5] = (byte)((id_un.byte1 >> 4) + 0x37);
                testCan[6] = (byte)(0x30);
                testCan[7] = 0x30;
                testCan[8] = 0x30;
                testCan[9] = 0xD;
                foreach(byte i in testCan) {
                    Debug.Write($"{i:X} ");
                }
            }
            Debug.WriteLine("");
            // Пример приема с CAN. Строку в число
            testCan = new byte[testCANCharBuf.Length];
            for(int i = 0; i < testCANCharBuf.Length; i++) {
                testCan[i] = Convert.ToByte(testCANCharBuf[i]);
            }
            foreach(byte i in testCan) {
                Debug.Write($"{i:X} ");
            }
            Debug.WriteLine("");
        }

        private readonly Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly GeneralFunctional GF = new();
        private readonly DefaultTransmission DT = new();
        private readonly byte[] hardBuf = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x1, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        private readonly byte[] hardBufWrite = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x09, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        /*private readonly byte[] canHardBufWrite = { //Get 11 - temperature
             0x74, 0x30, 0x33, 0x43, 0x32, 0x42, 0x30, 0x30, 0x30, 0x0D };*/
        /*private readonly byte[] canHardBufWrite = { //Set
             0x74, 0x34, 0x33, 0x32, 0x34, 0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x34, 0x34, 0x0D };*/
        /*private readonly byte[] canHardBufWrite = { //Get 29
            0x54, 0x42, 0x30, 0x00, 0x00, 0x31, 0x00, 0x31, 0x43, 0x33, 0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x0D };*/
        private readonly char[] testCANCharBuf = {
            't', '0', '3', 'C', '2', 'B', '0', '0', '0', '\r',
        };
        private readonly byte[] testInternetBuf = {
            0xB0, 0x00, 0x01, 0x00, 0x1C, 0x00, 0x04,
            0x00, 0x00, 0x98, 0xAD, 0x45,
            0xC0 };
        private readonly byte[] canHardBufWrite = { //Get 29
            0x54,
            0x31, 0x37, 0x00, 0x00, 0x31, 0x00, 0x32, 0x38, 0x34,
            0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x33, 0x33,
            0x0D };
        /*private readonly byte[] canHardBufWrite = { //Set RTR
             0x72, 0x32, 0x38, 0x38, 0x30, 0x0D };*/

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
            AddresTemperature.Text = $"{DT.temperatureTransmission.TShipAddres.addres:X}";
            IdTemperature.Text = $"{DT.temperatureTransmission.TId.getValue[0]:X}";

            AddresAcsel.Text = $"{DT.acselerometerTransmission.TShipAddres.addres:X}";
            IdAscelX.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdAscelY.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdAscelZ.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Z]:X}";
            IdAscelW.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.W]:X}";

            AddresRegul.Text = $"{DT.regulationTransmission.TShipAddres.addres:X}";
            IdRegulX.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdRegulY.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdRegulZ.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            AddresRates.Text = $"{DT.ratesensorTransmission.TShipAddres.addres:X}";
            IdRatesX.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdRatesY.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdRatesZ.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            AddresAccel.Text = $"{DT.accelsensorTransmission.TShipAddres.addres:X}";
            IdAccelX.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdAccelY.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdAccelZ.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            // Mag
            AddresReceiveMag.Text = $"{DT.ReceiveMagAddres.addres:X}";
            IdReceiveMag.Text = $"{DT.IdReceiveMag.addres:X}";
            IdShippingMag.Text = $"{DT.IdShipingMag.addres:X}";

            AddresMag1.Text = $"{DT.magnitudeTransmission1.TShipAddres.addres:X}";
            IdMag1X.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.X]:X}";
            IdMag1Y.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.Y]:X}";
            IdMag1Z.Text = $"{DT.magnitudeTransmission1.TId.getValue[(int)VarEnum.Z]:X}";

            AddresMag2.Text = $"{DT.magnitudeTransmission2.TShipAddres.addres:X}";
            IdMag2X.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.X]:X}";
            IdMag2Y.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.Y]:X}";
            IdMag2Z.Text = $"{DT.magnitudeTransmission2.TId.getValue[(int)VarEnum.Z]:X}";
            // Mag

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
            Change_Val_Track(TrackBarRotX.Value / 100.0f, LabRotX);
        }
        private void TrackBarRotY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotY.Value / 100.0f, LabRotY);
        }
        private void TrackBarRotZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotZ.Value / 100.0f, LabRotZ);
        }
        private void TrackBarRotW_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRotW.Value / 100.0f, LabRotW);
        }

        private void TrackBarPosX_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarPosX.Value / 100.0f, LabPosX);
        }
        private void TrackBarPosY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarPosY.Value / 100.0f, LabPosY);
        }
        private void TrackBarPosZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarPosZ.Value / 100.0f, LabPosZ);
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

        private void TrackBarRatesX_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRatesX.Value / 100.0f, LabRatesX);
        }
        private void TrackBarRatesY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRatesY.Value / 100.0f, LabRatesY);
        }
        private void TrackBarRatesZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarRatesZ.Value / 100.0f, LabRatesZ);
        }

        private void TrackBarAccelX_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarAccelX.Value / 100.0f, LabAccelX);
        }
        private void TrackBarAccelY_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarAccelY.Value / 100.0f, LabAccelY);
        }
        private void TrackBarAccelZ_Scroll(object sender, EventArgs e) {
            Change_Val_Track(TrackBarAccelZ.Value / 100.0f, LabAccelZ);
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
                        if(id.it == Convert.ToInt16(IdReceiveMag.Text)) {
                            LogBox.Text = "Get id";
                            if(addres.it == Convert.ToInt16(AddresReceiveMag.Text)) {
                                LogBox.Text += "Get addres";
                                it_un idSend = new() {
                                    it = Convert.ToInt16(IdShippingMag.Text)
                                };
                                hardBufWrite[18] = idSend.byte1;
                                hardBufWrite[19] = idSend.byte2;

                                hardBufWrite[20] = buffer[22];
                                hardBufWrite[21] = buffer[23];

                                hardBufWrite[22] = buffer[20];
                                hardBufWrite[23] = buffer[21];

                                byte[] sendBuf = new byte[buffer.Length + 3 * 4];
                                sendBuf[24] = 0x04;
                                sendBuf[25] = 0x00;

                                fl_un n_fl = new() {
                                    fl = (float)Convert.ToDecimal(LabMagX.Text)
                                };
                                sendBuf[26] = n_fl.byte1;
                                sendBuf[27] = n_fl.byte2;
                                sendBuf[28] = n_fl.byte3;
                                sendBuf[29] = n_fl.byte4;
                                n_fl = new() {
                                    fl = (float)Convert.ToDecimal(LabMagY.Text)
                                };
                                sendBuf[30] = n_fl.byte1;
                                sendBuf[31] = n_fl.byte2;
                                sendBuf[32] = n_fl.byte3;
                                sendBuf[33] = n_fl.byte4;
                                n_fl = new() {
                                    fl = (float)Convert.ToDecimal(LabMagZ.Text)
                                };
                                sendBuf[34] = n_fl.byte1;
                                sendBuf[35] = n_fl.byte2;
                                sendBuf[36] = n_fl.byte3;
                                sendBuf[37] = n_fl.byte4;

                                GeneralFunctional.SendMessageInSocket(serverListener,
                                    hardBufWrite, LogBox);
                            } else {
                                LogBox.Text = "Lost addres";
                            }
                        } else {
                            LogBox.Text = "Lost information";
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
            it.it = DT.temperatureTransmission.TShipAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabTemp.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }

        private void SendMagnetometer1_Click(object sender, EventArgs e) {
            it.it = Convert.ToInt16(AddresMag1.Text);

            fl.fl = (float)Convert.ToDouble(LabMagX.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagY.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }
        private void SendMagnetometer2_Click(object sender, EventArgs e) {
            it.it = DT.magnitudeTransmission2.TShipAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabMagX_2.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagY_2.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabMagZ_2.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }

        private void SendAcselerometer_Click(object sender, EventArgs e) {
            it.it = DT.acselerometerTransmission.TShipAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabRotX.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabRotY.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);

            fl.fl = (float)Convert.ToDouble(LabRotZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }
        // Send data

        private void IPTextBox_TextChanged(object sender, EventArgs e) {
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            //Read();
            int byteWrite = 0, offsetByte = 8;
            while(byteWrite < serialPort.BytesToRead) {
                int copyByte = byteWrite + offsetByte > serialPort.BytesToRead ?
                    serialPort.BytesToRead - byteWrite : offsetByte;
                byte[] data = new byte[copyByte];
                serialPort.Read(data, 0, copyByte);
                for(int i = 0; i < data.Length; i++) {
                    Debug.WriteLine(" " + $"{data[i]:X}");
                }
                byteWrite += offsetByte;
            }
        }
        private void Read() {
            int byteWrite = 0, offsetByte = 8;
            while(byteWrite < serialPort.BytesToRead) {
                int copyByte = byteWrite + offsetByte > serialPort.BytesToRead ?
                    serialPort.BytesToRead - byteWrite : offsetByte;
                byte[] data = new byte[copyByte];
                serialPort.Read(data, 0, copyByte);
                for(int i = 0; i < data.Length; i++) {
                    LogBox.Text += " " + $"{data[i]:X}";
                }
                byteWrite += offsetByte;
            }
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

            //serialPort.Write("T0000E3883223344\r");



            LogBox.Text += "\r\n";
            int byteWrite = 0, offsetByte = 8;
            while(byteWrite < canHardBufWrite.Length) {
                int copyByte = byteWrite + offsetByte >= canHardBufWrite.Length ?
                    canHardBufWrite.Length - byteWrite : offsetByte;
                byte[] data = new byte[copyByte];
                Array.Copy(canHardBufWrite, byteWrite, data, 0, copyByte);
                serialPort.Write(data, 0, copyByte);
                for(int i = 0; i < data.Length; i++) {
                    LogBox.Text += " " + $"{data[i]:X}";
                }
                byteWrite += offsetByte;
            }
            LogBox.Text += "\r\n";

            //serialPort.Write("t280411223344\r");
        }
        private void CANTestRead_Click(object sender, EventArgs e) {
            Read();
        }

        private SerialPort serialPort;
        private void OpenRKSCAN_Click(object sender, EventArgs e) {
            serialPort = new(CANPort.Text, 9600, Parity.None, 8, StopBits.One);

            if(!serialPort.IsOpen) {
                serialPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
                try {
                    serialPort.Open();
                    // Установить скорость
                    serialPort.Write(string.Format("S{0}\r", CANSpeed.SelectedIndex));
                    Thread.Sleep(100);
                    Read();
                    // Открыть
                    serialPort.Write("O\r");
                    Thread.Sleep(100);
                    Read();

                    LogBox.Text = "Port open";
                }
                catch(Exception ex) {
                    LogBox.Text = ex.Message;
                }
            } else {
                LogBox.Text = "serialPort is open";
            }
        }
        private void CloseRKSCAN_Click(object sender, EventArgs e) {
            if(serialPort.IsOpen) {
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
}