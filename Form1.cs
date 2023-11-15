using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace HustonRTEMS {
    public partial class MainForm: Form {
        private SerialPort _serialPort;
        private XMLReader _xmlRreader;
        private LogReader _logReader;
        private Socket serverListener, client;
        private readonly CanToUnican CTU = new();
        private readonly Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly GeneralFunctional GF = new();
        private readonly byte[] hardBuf = {
            //0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x01, 0x00, 0x1C, 0x00, 0x00, 0x00, /*0xC0*/ };

        private int message_size = new();
        private byte[] buffer;

        public FlUn fl = new();
        public ItUn it = new();

#pragma warning disable CS8618
        public MainForm() {
            InitializeComponent();
        }
#pragma warning restore CS8618
        private void MainForm_ResizeBegin(object sender, EventArgs e) {
            if(LogBox.DataBindings.Count == 0) {
                int startWF = ActiveForm.Size.Width;
                int startHF = ActiveForm.Size.Height;
                int startW = LogBox.Width;
                int startH = LogBox.Height;
                Binding newBinding = new("Size", ActiveForm, "Size");
                newBinding.Format += (sender, e) => e.Value = new Size(
                    startW + ((ActiveForm.Size.Width - startWF) / 2),
                    startH + ((ActiveForm.Size.Height - startHF) / 2));
                LogBox.DataBindings.Add(newBinding);
            }
            if(LogBox2.DataBindings.Count == 0) {
                int startWF = ActiveForm.Size.Width;
                int startHF = ActiveForm.Size.Height;
                int startW = LogBox.Width;
                int startH = LogBox.Height;
                Binding newBinding = new("Size", ActiveForm, "Size");
                newBinding.Format += (sender, e) => e.Value = new Size(
                    startW + ((ActiveForm.Size.Width - startWF) / 2),
                    startH + ((ActiveForm.Size.Height - startHF) / 2));
                LogBox2.DataBindings.Add(newBinding);
            }
        }
        private void MainForm_Load(object sender, EventArgs e) {
            if(cfg.GetSection("customProperty") is CustomProperty section) {
                IPTextBox.Text = section.IP;
                PortRTEMS.Text = section.PortRTEMS;
                PortHUSTON.Text = section.PortHUSTON;
                PortHUSTONTelnet.Text = section.PortHUSTONTelnet;
                CANSpeed.Text = section.CANSpeed;
                CANPort.Text = section.CANPort;

                AddresReceiveTem.Text = section.ReceiveTemAddres;
                IdReceiveTem.Text = section.IdReceiveTem;
                IdShippingTem.Text = section.IdShipingTem;
                AddresTemperature.Text = section.SensorTemAddress;

                AddresReceiveAcs.Text = section.ReceiveAcsAddres;
                IdReceiveAcs.Text = section.IdReceiveAcs;
                IdShippingAcs.Text = section.IdShipingAcs;
                AddresAcs.Text = section.SensorAcsAddress;

                AddresReceiveReg.Text = section.ReceiveRegAddres;
                IdReceiveReg.Text = section.IdReceiveReg;
                IdShippingReg.Text = section.IdShipingReg;
                AddresReg.Text = section.SensorRegAddress;

                AddresReceiveRat.Text = section.ReceiveRatAddres;
                IdReceiveRat.Text = section.IdReceiveRat;
                IdShippingRat.Text = section.IdShipingRat;
                AddresRat.Text = section.SensorRatAddress;

                AddresReceiveAcc.Text = section.ReceiveAccAddres;
                IdReceiveAcc.Text = section.IdReceiveAcc;
                IdShippingAcc.Text = section.IdShipingAcc;
                AddresAcc.Text = section.SensorAccAddress;

                AddresReceiveTime.Text = section.ReceiveAddresTime;
                IdReceiveTime.Text = section.IdReceiveTime;
                IdShippingTime.Text = section.IdShipingTime;
                AddresTime.Text = section.AddresTime;

                AddresReceiveBeacon.Text = section.ReceiveAddresBeacon;
                IdReceiveBeacon.Text = section.IdReceiveBeacon;
                IdShippingBeacon.Text = section.IdShipingBeacon;
                AddresBeacon.Text = section.SensorBeaconAddress;

                AddresReceiveExBeacon.Text = section.ReceiveAddresExBeacon;
                IdReceiveExBeacon.Text = section.IdReceiveExBeacon;
                IdShippingExBeacon.Text = section.IdShipingExBeacon;
                AddresExBeacon.Text = section.SensorExBeaconAddress;

                AddresReceiveAdcsBeacon.Text = section.ReceiveAddresAdcsBeacon;
                IdReceiveAdcsBeacon.Text = section.IdReceiveAdcsBeacon;
                IdShippingAdcsBeacon.Text = section.IdShipingAdcsBeacon;
                AddresAdcsBeacon.Text = section.SensorAdcsBeaconAddress;

                AddresReceiveRSX.Text = section.ReceiveAddresRSX;
                IdReceiveRSX.Text = section.IdReceiveRSX;
                IdShippingRSX.Text = section.IdShipingRSX;
                AddresRSX.Text = section.AddressRSX;

                AddresReceiveRSY.Text = section.ReceiveAddresRSY;
                IdReceiveRSY.Text = section.IdReceiveRSY;
                IdShippingRSY.Text = section.IdShipingRSY;
                AddresRSY.Text = section.AddressRSY;

                AddresReceiveRSZ.Text = section.ReceiveAddresRSZ;
                IdReceiveRSZ.Text = section.IdReceiveRSZ;
                IdShippingRSZ.Text = section.IdShipingRSZ;
                AddresRSZ.Text = section.AddressRSZ;

                AddresReceiveMX.Text = section.ReceiveAddresMagX;
                IdReceiveMX.Text = section.IdReceiveMagX;
                IdShippingMX.Text = section.IdShipingMagX;
                AddresMX.Text = section.AddressMagX;

                AddresReceiveMY.Text = section.ReceiveAddresMagY;
                IdReceiveMY.Text = section.IdReceiveMagY;
                IdShippingMY.Text = section.IdShipingMagY;
                AddresMY.Text = section.AddressMagY;

                AddresReceiveMZ.Text = section.ReceiveAddresMagZ;
                IdReceiveMZ.Text = section.IdReceiveMagZ;
                IdShippingMZ.Text = section.IdShipingMagZ;
                AddresMZ.Text = section.AddressMagZ;

                AddresReceiveCutFile.Text = section.AddresReceiveCutFile;
                IdShippingCutFile.Text = section.IdShippingCutFile;
                AddresCutFile.Text = section.AddresCutFile;
            }
            // XML
            string[] files = Directory.GetFiles("C:\\Users\\Ivar\\Documents\\SX-Houston-app_v214\\resources\\devices");
            _xmlRreader = new(files,
                CommandPanel1, CommandPanel2, CommandPanel3, ToServer, _serialPort, BaseStationAd, DeviceAd,
                LogBox);
            _xmlRreader.MoldComboBoxName();
            // Log
            _logReader = new("C:\\Users\\Ivar\\Documents\\SX-Houston-app_v214\\logs\\HistoryLog_2023-10-24_10-21-35.csv",
                CommandPanel2, CommandPanel4);
            _logReader.ReadLog();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            CloseRKSCAN_Click(null, null);
            if(cfg.GetSection("customProperty") is CustomProperty section) {
                section.IP = IPTextBox.Text;
                section.PortRTEMS = PortRTEMS.Text;
                section.PortHUSTON = PortHUSTON.Text;
                section.PortHUSTONTelnet = PortHUSTONTelnet.Text;
                section.CANSpeed = CANSpeed.Text;
                section.CANPort = CANPort.Text;

                section.ReceiveTemAddres = AddresReceiveTem.Text;
                section.IdReceiveTem = IdReceiveTem.Text;
                section.IdShipingTem = IdShippingTem.Text;
                section.SensorTemAddress = AddresTemperature.Text;

                section.ReceiveAcsAddres = AddresReceiveAcs.Text;
                section.IdReceiveAcs = IdReceiveAcs.Text;
                section.IdShipingAcs = IdShippingAcs.Text;
                section.SensorAcsAddress = AddresAcs.Text;

                section.ReceiveRegAddres = AddresReceiveReg.Text;
                section.IdReceiveReg = IdReceiveReg.Text;
                section.IdShipingReg = IdShippingReg.Text;
                section.SensorRegAddress = AddresReg.Text;

                section.ReceiveRatAddres = AddresReceiveRat.Text;
                section.IdReceiveRat = IdReceiveRat.Text;
                section.IdShipingRat = IdShippingRat.Text;
                section.SensorRatAddress = AddresRat.Text;

                section.ReceiveAccAddres = AddresReceiveAcc.Text;
                section.IdReceiveAcc = IdReceiveAcc.Text;
                section.IdShipingAcc = IdShippingAcc.Text;
                section.SensorAccAddress = AddresAcc.Text;

                section.ReceiveAddresTime = AddresReceiveTime.Text;
                section.IdReceiveTime = IdReceiveTime.Text;
                section.IdShipingTime = IdShippingTime.Text;
                section.AddresTime = AddresTime.Text;

                section.ReceiveAddresBeacon = AddresReceiveBeacon.Text;
                section.IdReceiveBeacon = IdReceiveBeacon.Text;
                section.IdShipingBeacon = IdShippingBeacon.Text;
                section.SensorBeaconAddress = AddresBeacon.Text;

                section.ReceiveAddresExBeacon = AddresReceiveExBeacon.Text;
                section.IdReceiveExBeacon = IdReceiveExBeacon.Text;
                section.IdShipingExBeacon = IdShippingExBeacon.Text;
                section.SensorExBeaconAddress = AddresExBeacon.Text;

                section.ReceiveAddresAdcsBeacon = AddresReceiveAdcsBeacon.Text;
                section.IdReceiveAdcsBeacon = IdReceiveAdcsBeacon.Text;
                section.IdShipingAdcsBeacon = IdShippingAdcsBeacon.Text;
                section.SensorAdcsBeaconAddress = AddresAdcsBeacon.Text;

                section.ReceiveAddresRSX = AddresReceiveRSX.Text;
                section.IdReceiveRSX = IdReceiveRSX.Text;
                section.IdShipingRSX = IdShippingRSX.Text;
                section.AddressRSX = AddresRSX.Text;

                section.ReceiveAddresRSY = AddresReceiveRSY.Text;
                section.IdReceiveRSY = IdReceiveRSY.Text;
                section.IdShipingRSY = IdShippingRSY.Text;
                section.AddressRSY = AddresRSY.Text;

                section.ReceiveAddresRSZ = AddresReceiveRSZ.Text;
                section.IdReceiveRSZ = IdReceiveRSZ.Text;
                section.IdShipingRSZ = IdShippingRSZ.Text;
                section.AddressRSZ = AddresRSZ.Text;

                section.ReceiveAddresMagX = AddresReceiveMX.Text;
                section.IdReceiveMagX = IdReceiveMX.Text;
                section.IdShipingMagX = IdShippingMX.Text;
                section.AddressMagX = AddresMX.Text;

                section.ReceiveAddresMagY = AddresReceiveMY.Text;
                section.IdReceiveMagY = IdReceiveMY.Text;
                section.IdShipingMagY = IdShippingMY.Text;
                section.AddressMagY = AddresMY.Text;

                section.ReceiveAddresMagZ = AddresReceiveMZ.Text;
                section.IdReceiveMagZ = IdReceiveMZ.Text;
                section.IdShipingMagZ = IdShippingMZ.Text;
                section.AddressMagZ = AddresMZ.Text;

                section.AddresReceiveCutFile = AddresReceiveCutFile.Text;
                section.IdShippingCutFile = IdShippingCutFile.Text;
                section.AddresCutFile = AddresCutFile.Text;

                cfg.Save();
            }
        }

        private void ClearLog_Click(object sender, EventArgs e) {
            LogBox.Text = "";
            LogBox2.Text = "";
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
        private void BTemp_Scroll(object sender, EventArgs e) {
            Change_Val_Track(BTemp.Value / 5.0f, LabBTemp);
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

        private async void Open_thread() {
            while(flagRead) {
                Thread.Sleep(1000);
                GF.ClearInvokeTextBox(LogBox2);
                GF.InvokeTextBox(LogBox2, "Search socet\r\n");
                IPEndPoint? ipep = null;
                try {
                    ipep = new(IPAddress.Parse(IPTextBox.Text),
                        Convert.ToInt16(PortHUSTON.Text));
                }
                catch(Exception) {
                    break;
                }
                serverListener = new(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                int KISSBUFFER_SIZE = 256;
                buffer = new byte[KISSBUFFER_SIZE];
                int raw_buffer_size = GF.kissHeader.Length; // Kiss header
                if(!serverListener.Connected) {
                    try {
                        serverListener.Connect(ipep);
                        GF.InvokeTextBox(LogBox2, $"Socet open\r\n");
                        _ = await serverListener.SendAsync(hardBuf, SocketFlags.None);
                    }
                    catch(Exception ex) {
                        GF.InvokeTextBox(LogBox2, ex.Message + "\r\n");
                    }

                    while(serverListener.Connected) {
                        try {
                            GF.InvokeTextBox(LogBox2, $"Wait message, message_count: ");
                            message_size = await serverListener.ReceiveAsync(buffer, SocketFlags.None);
                            GF.InvokeTextBox(LogBox2, message_size.ToString() + "\r\n");
                        }
                        catch(Exception ex) {
                            GF.InvokeTextBox(LogBox2, ex.Message + "\r\n");
                        }
                        Thread.Sleep(1000);

                        if(message_size > 0) {
                            GF.InvokeTextBox(LogBox2, $"Socket server response message: \r\n");
                            if(CheckBoxRTEMS.Checked) {
                                while(raw_buffer_size < message_size) {
                                    if(raw_buffer_size >= 0) {
                                        GF.InvokeTextBox(LogBox2, $"{buffer[raw_buffer_size]:X} ");
                                    }
                                    GF.WriteChangeKissFESC(ref buffer);
                                    raw_buffer_size++;
                                }
                                raw_buffer_size = 0;
                            } else {
                                raw_buffer_size = GF.kissHeader.Length;
                            }

                            // Example of sending a power-on response
                            ItUn id = new() {
                                byte1 = buffer[18 - raw_buffer_size],
                                byte2 = buffer[19 - raw_buffer_size]
                            }, addresIn = new() {
                                byte1 = buffer[20 - raw_buffer_size],
                                byte2 = buffer[21 - raw_buffer_size]
                            }, addresOut = new() {
                                byte1 = buffer[22 - raw_buffer_size],
                                byte2 = buffer[23 - raw_buffer_size]
                            };
                            if(id.it == Convert.ToInt16(IdReceiveAcs.Text, 16) &&
                                  addresIn.it == Convert.ToInt16(AddresReceiveAcs.Text, 16) &&
                                  addresOut.it == Convert.ToInt16(AddresAcs.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendAcselerometer_Click(null, null);
                            } else if(id.it == Convert.ToInt16(IdReceiveReg.Text, 16) &&
                                  addresIn.it == Convert.ToInt16(AddresReceiveReg.Text, 16) &&
                                  addresOut.it == Convert.ToInt16(AddresReg.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendRegulation_Click(null, null);
                            } else if(id.it == Convert.ToInt16(IdReceiveRat.Text, 16) &&
                                  addresIn.it == Convert.ToInt16(AddresReceiveRat.Text, 16) &&
                                  addresOut.it == Convert.ToInt16(AddresRat.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendRatesensor_Click(null, null);
                            } else if(id.it == Convert.ToInt16(IdReceiveAcc.Text, 16) &&
                                  addresIn.it == Convert.ToInt16(AddresReceiveAcc.Text, 16) &&
                                  addresOut.it == Convert.ToInt16(AddresAcc.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendAccelsensor_ClickAsync(null, null);
                            } else if(id.it == 0x1B0 &&
                                  addresIn.it == 0x1C &&
                                  addresOut.it == 0x01) {
                                // Send temperature
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                if(CheckBoxRTEMS.Checked) {
                                    _ = await client.SendAsync(buffer, SocketFlags.None);
                                }
                            } else if(id.it == Convert.ToInt16(IdReceiveTime.Text, 16) &&
                                  addresIn.it == Convert.ToInt16(AddresReceiveTime.Text, 16) &&
                                  addresOut.it == Convert.ToInt16(IdShippingTime.Text, 16)) {
                                // Send time
                                DateTimeOffset dto = new(DateTime.Now);
                                long nowTime = dto.ToUnixTimeSeconds();
                                int[] arIValue = new int[5];
                                while(nowTime > byte.MaxValue) {
                                    nowTime -= byte.MaxValue + 1;
                                    arIValue[1] += 0x01;
                                    while(arIValue[1] > byte.MaxValue) {
                                        arIValue[1] -= byte.MaxValue + 1;
                                        arIValue[2] += 0x01;
                                        while(arIValue[2] > byte.MaxValue) {
                                            arIValue[2] -= byte.MaxValue + 1;
                                            arIValue[3] += 0x01;
                                            while(arIValue[3] > byte.MaxValue) {
                                                arIValue[3] -= byte.MaxValue + 1;
                                                arIValue[4] += 0x01;
                                            }
                                        }
                                    }
                                }
                                arIValue[0] += (int)nowTime;
                                GF.SendMessageInSocketTime(client,
                                    id.it, addresOut.it, addresIn.it,
                                    5, arIValue,
                                    LogBox, true);
                            } else {
                                LogBox2.Invoke(new Action(() => {
                                    LogBox2.Text = string.Format(
                                        "Wrong address or id.\r\nid:'{0}'\r\naddIn:'{1}'\r\naddOut:'{2}'\r\nmesCount'{3}'",
                                        id.it, addresIn.it, addresOut.it, message_size
                                    );
                                }));
                            }
                            message_size = 0;
                        } else {
                            break;
                        }
                        Thread.Sleep(Convert.ToInt16(textBoxDelay.Text) * 1000);
                        GF.InvokeTextBox(LogBox2, $"\r\nWait new message!\r\n");
                    }
                }

                serverListener.Close();
            }
        }

        private Thread nThread;
        private bool flagRead;
        private async void OpenSocetServer_ClickAsync(object sender, EventArgs e) {
            flagRead = true;
            nThread = new(Open_thread);
            nThread.Start();

            if(CheckBoxRTEMS.Checked) {
                IPEndPoint ipep = new(IPAddress.Parse(IPTextBox.Text),
                Convert.ToInt16(PortRTEMS.Text));
                Socket serverListener_S = new(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                serverListener_S.Bind(ipep);
                serverListener_S.Listen(200);
                LogBox.Text = "Waiting for a client...";

                int KISSBUFFER_SIZE = 256;
                buffer = new byte[KISSBUFFER_SIZE];
                int raw_buffer_size = 0;
                client = await serverListener_S.AcceptAsync();
                LogBox.Text = "Listen open";
                // Receive message.
                while(true) {
                    message_size = await client.ReceiveAsync(buffer, SocketFlags.None);

                    if(message_size > 0) {
                        LogBox.Text =
                            $"HUSTON message: ";
                        while(raw_buffer_size < message_size) {
                            if(raw_buffer_size >= 0) {
                                LogBox.Text += $"{buffer[raw_buffer_size]:X} ";
                            }
                            raw_buffer_size++;
                        }
                        raw_buffer_size = 0;
                        if(serverListener != null && serverListener.Connected) {
                            LogBox.Text += "\r\nSend to RTEMS";
                            if(!CheckKISS.Checked) {
                                byte[] mBuffer = buffer;
                                buffer = new byte[mBuffer.Length];
                                for(int i = GF.kissHeader.Length; i < mBuffer.Length; i++) {
                                    buffer[i] = mBuffer[i];
                                }
                            }
                            _ = await serverListener.SendAsync(buffer, SocketFlags.None);
                        } else {
                            LogBox.Text += "\r\nPort for RTEMS don't open";
                        }
                    } else {
                        break;
                    }
                }
                client.Disconnect(true);
                serverListener_S.Close();
            }
        }
        private async void CloseSocketServer_Click(object sender, EventArgs e) {
            if(nThread != null && nThread.IsAlive) {
                flagRead = false;
            }
            if(serverListener != null && serverListener.Connected) {
                try {
                    await serverListener.DisconnectAsync(true);
                }
                catch(Exception) {
                }
            }
        }

        // Send data
        private async void SendTemperature_ClickAsync(object sender, EventArgs e) {
            if(UseInternet.Checked) {
                int idShipping = Convert.ToInt16(IdShippingTem.Text, 16);
                int addresValue = Convert.ToInt16(AddresTemperature.Text, 16);
                int addresReceive = Convert.ToInt16(AddresReceiveTem.Text, 16);
                int iCount = 0;
                int fCount = 1;
                int[] arIValue = Array.Empty<int>();
                float[] arFValue = new float[fCount];
                arFValue[0] = Convert.ToSingle(LabTemp.Text);
                GF.SendMessageInSocket(serverListener,
                    idShipping, addresValue, addresReceive,
                    iCount, fCount, arIValue, arFValue,
                    LogBox, CheckKISS.Checked);

                if(CheckBoxRTEMS.Checked) {
                    _ = await client.SendAsync(buffer, SocketFlags.None);
                }
            } else if(UseCan.Checked) {
                const int unicanLenght = 1 * sizeof(float);
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingTem.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveTem.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresTemperature.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn fuX = new() {
                    fl = Convert.ToSingle(LabTemp.Text)
                };
                test.data = new byte[unicanLenght]
                {
                    fuX.byte1, fuX.byte2, fuX.byte3, fuX.byte4
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }

        private async void SendAcselerometer_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                int idShipping = Convert.ToInt16(IdShippingAcs.Text, 16);
                int addresValue = Convert.ToInt16(AddresAcs.Text, 16);
                int addresReceive = Convert.ToInt16(AddresReceiveAcs.Text, 16);
                int iCount = 0;
                int fCount = 4;
                int[] arIValue = Array.Empty<int>();
                float[] arFValue = new float[fCount];
                arFValue[0] = Convert.ToSingle(LabRotX.Text);
                arFValue[1] = Convert.ToSingle(LabRotY.Text);
                arFValue[2] = Convert.ToSingle(LabRotZ.Text);
                arFValue[3] = Convert.ToSingle(LabRotW.Text);
                GF.SendMessageInSocket(serverListener,
                    idShipping, addresValue, addresReceive,
                    iCount, fCount, arIValue, arFValue,
                    LogBox, CheckKISS.Checked);

                if(CheckBoxRTEMS.Checked) {
                    _ = await client.SendAsync(buffer, SocketFlags.None);
                }
            } else if(UseCan.Checked) {
                const int unicanLenght = 4 * sizeof(float);
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingAcc.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveAcc.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresAcc.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn fuX = new() {
                    fl = Convert.ToSingle(LabRotX.Text)
                };
                FlUn fuY = new() {
                    fl = Convert.ToSingle(LabRotY.Text)
                };
                FlUn fuZ = new() {
                    fl = Convert.ToSingle(LabRotZ.Text)
                };
                FlUn fuW = new() {
                    fl = Convert.ToSingle(LabRotW.Text)
                };
                test.data = new byte[unicanLenght]
                {
                    fuX.byte1, fuX.byte2, fuX.byte3, fuX.byte4,
                    fuY.byte1, fuY.byte2, fuY.byte3, fuY.byte4,
                    fuZ.byte1, fuZ.byte2, fuZ.byte3, fuZ.byte4,
                    fuW.byte1, fuW.byte2, fuW.byte3, fuW.byte4,
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }

        private async void SendRegulation_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                int idShipping = Convert.ToInt16(IdShippingReg.Text, 16);
                int addresValue = Convert.ToInt16(AddresReg.Text, 16);
                int addresReceive = Convert.ToInt16(AddresReceiveReg.Text, 16);
                int iCount = 0;
                int fCount = 3;
                int[] arIValue = Array.Empty<int>();
                float[] arFValue = new float[fCount];
                arFValue[0] = Convert.ToSingle(LabPosX.Text);
                arFValue[1] = Convert.ToSingle(LabPosY.Text);
                arFValue[2] = Convert.ToSingle(LabPosZ.Text);
                GF.SendMessageInSocket(serverListener,
                    idShipping, addresValue, addresReceive,
                    iCount, fCount, arIValue, arFValue,
                    LogBox, CheckKISS.Checked);

                if(CheckBoxRTEMS.Checked) {
                    _ = await client.SendAsync(buffer, SocketFlags.None);
                }
            } else if(UseCan.Checked) {
            }
        }

        private async void SendRatesensor_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                int idShipping = Convert.ToInt16(IdShippingRat.Text, 16);
                int addresValue = Convert.ToInt16(AddresRat.Text, 16);
                int addresReceive = Convert.ToInt16(AddresReceiveRat.Text, 16);
                int iCount = 0;
                int fCount = 3;
                int[] arIValue = Array.Empty<int>();
                float[] arFValue = new float[fCount];
                arFValue[0] = Convert.ToSingle(LabRatesX.Text);
                arFValue[1] = Convert.ToSingle(LabRatesY.Text);
                arFValue[2] = Convert.ToSingle(LabRatesZ.Text);
                GF.SendMessageInSocket(serverListener,
                    idShipping, addresValue, addresReceive,
                    iCount, fCount, arIValue, arFValue,
                    LogBox, CheckKISS.Checked);

                if(CheckBoxRTEMS.Checked) {
                    _ = await client.SendAsync(buffer, SocketFlags.None);
                }
            } else if(UseCan.Checked) {
                const int unicanLenght = 3 * sizeof(float);
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingRat.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveRat.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresRat.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn fuX = new() {
                    fl = Convert.ToSingle(LabRatesX.Text)
                };
                FlUn fuY = new() {
                    fl = Convert.ToSingle(LabRatesY.Text)
                };
                FlUn fuZ = new() {
                    fl = Convert.ToSingle(LabRatesZ.Text)
                };
                test.data = new byte[unicanLenght]
                {
                    fuX.byte1, fuX.byte2, fuX.byte3, fuX.byte4,
                    fuY.byte1, fuY.byte2, fuY.byte3, fuY.byte4,
                    fuZ.byte1, fuZ.byte2, fuZ.byte3, fuZ.byte4,
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }

        private async void SendAccelsensor_ClickAsync(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                int idShipping = Convert.ToInt16(IdShippingAcc.Text, 16);
                int addresValue = Convert.ToInt16(AddresAcc.Text, 16);
                int addresReceive = Convert.ToInt16(AddresReceiveAcc.Text, 16);
                int iCount = 0;
                int fCount = 3;
                int[] arIValue = Array.Empty<int>();
                float[] arFValue = new float[fCount];
                arFValue[0] = Convert.ToSingle(LabAccelX.Text);
                arFValue[1] = Convert.ToSingle(LabAccelY.Text);
                arFValue[2] = Convert.ToSingle(LabAccelZ.Text);
                GF.SendMessageInSocket(serverListener,
                    idShipping, addresValue, addresReceive,
                    iCount, fCount, arIValue, arFValue,
                    LogBox, CheckKISS.Checked);

                if(CheckBoxRTEMS.Checked) {
                    _ = await client.SendAsync(buffer, SocketFlags.None);
                }
            } else if(UseCan.Checked) {
            }
        }
        private void SendBeacon_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                const int psVar = 15 * sizeof(ushort);
                const int checkVar = 2 * sizeof(byte);
                const int reserveVar = 1 * sizeof(ushort);
                const int PSUAB = 1 * sizeof(ushort);
                const int regTelId = 1 * sizeof(int);
                const int PS_time = 1 * sizeof(int);
                const int psResetCounter = 1 * sizeof(byte);
                const int PS_FL = 1 * sizeof(byte);
                const int tAMP = 1 * sizeof(byte);
                const int tUHF = 1 * sizeof(byte);
                const int PSSIrx = 1 * sizeof(byte);
                const int PSSIdle = 1 * sizeof(byte);
                const int Pf = 1 * sizeof(byte);
                const int Pb = 1 * sizeof(byte);
                const int uhfResetCounter = 1 * sizeof(byte);
                const int UHF_FL = 1 * sizeof(byte);
                const int UHF_time = 1 * sizeof(int);
                const int upTime = 1 * sizeof(int);
                const int current = 1 * sizeof(ushort);
                const int Uuhf = 1 * sizeof(ushort);
                const int unicanLenght = psVar + checkVar + reserveVar + PSUAB + regTelId
                    + PS_time + psResetCounter + PS_FL + tAMP + tUHF + PSSIrx + PSSIdle + Pf + Pb
                    + uhfResetCounter + UHF_FL + UHF_time + upTime + current + Uuhf;
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingBeacon.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveBeacon.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresBeacon.Text, 16),
                    unicanLength = unicanLenght
                };
                ItUn[] PS = new ItUn[unicanLenght];
                ItUn occupiedValueI = new();
                test.data = new byte[unicanLenght];
                //----
                PS[0].it = Convert.ToInt16(PSUSB1.Text);
                PS[1].it = Convert.ToInt16(PSUSB2.Text);
                PS[2].it = Convert.ToInt16(PSUSB3.Text);
                PS[3].it = Convert.ToInt16(PSIsb1.Text);
                PS[4].it = Convert.ToInt16(PSIsb2.Text);
                PS[5].it = Convert.ToInt16(PSIsb3.Text);
                PS[6].it = Convert.ToInt16(PSIab.Text);
                PS[7].it = Convert.ToInt16(PSIch1.Text);
                PS[8].it = Convert.ToInt16(PSIch2.Text);
                PS[9].it = Convert.ToInt16(PSIch3.Text);
                PS[10].it = Convert.ToInt16(PSIch4.Text);
                PS[11].it = Convert.ToInt16(PSt1_pw.Text);
                PS[12].it = Convert.ToInt16(PSt2_pw.Text);
                PS[13].it = Convert.ToInt16(PSt3_pw.Text);
                PS[14].it = Convert.ToInt16(PSt4_pw.Text);
                PS[15].it = Convert.ToInt16(PSt4_pw.Text);
                //----
                for(int i = 0, j = 0; i < unicanLenght; i++, j++) {
                    if(i < psVar) {
                        test.data[i] = PS[j].byte1;
                        test.data[++i] = PS[j].byte2;
                    } else if(i == (psVar + checkVar + reserveVar)) {
                        occupiedValueI.it = Convert.ToInt16(PSUab.Text);
                        test.data[i] = occupiedValueI.byte1;
                        test.data[++i] = occupiedValueI.byte2;
                    } else if(i == (psVar + checkVar + reserveVar + PSUAB + regTelId)) {
                        int psTime = Convert.ToInt32(PStime.Text);
                        test.data[i] = (byte)psTime;
                        test.data[++i] = (byte)(psTime >> 8);
                        test.data[++i] = (byte)(psTime >> 16);
                        test.data[++i] = (byte)(psTime >> 24);
                    } else if(i == (psVar + checkVar + reserveVar + PSUAB + regTelId + PS_time)) {
                        test.data[i] = Convert.ToByte(PSps_reset_counter.Text);
                    } else if(i == (psVar + checkVar + reserveVar + PSUAB + regTelId + PS_time + psResetCounter
                            + PS_FL)) {
                        test.data[i] = Convert.ToByte(UHFt_amp.Text);
                    } else if(i == (psVar + checkVar + reserveVar + PSUAB + regTelId + PS_time + psResetCounter
                            + PS_FL + tAMP)) {
                        test.data[i] = Convert.ToByte(UHFt_uhf.Text);
                    } else {
                        test.data[i] = i == (psVar + checkVar + reserveVar + PSUAB + regTelId + PS_time + psResetCounter
                                                    + PS_FL + tAMP + tUHF + PSSIrx + PSSIdle + Pf + Pb)
                            ? Convert.ToByte(UHFuhf_reset_counter.Text)
                            : (byte)0;
                    }
                }
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }
        private void SendExBeacon_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                const int temVar = 1 * sizeof(short);
                const int rotVar = 3 * sizeof(float);
                const int accVar = 3 * sizeof(float);
                const int unicanLenght = temVar + rotVar + accVar;
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingExBeacon.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveExBeacon.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresExBeacon.Text, 16),
                    unicanLength = unicanLenght,
                    data = new byte[unicanLenght]
                };
                ItUn[] temV = new ItUn[temVar];
                FlUn[] rootVar = new FlUn[rotVar];
                FlUn[] acVar = new FlUn[accVar];
                for(int i = 0; i < unicanLenght; i++) {
                    if(i < temVar) {
                        temV[i].it = Convert.ToInt16(LabBTemp.Text);
                    } else if(i < temVar + rotVar) {
                        int j = i - temVar;
                        rootVar[j].fl = Convert.ToSingle(LabRatesX.Text);
                        rootVar[j + 1].fl = Convert.ToSingle(LabRatesY.Text);
                        rootVar[j + 2].fl = Convert.ToSingle(LabRatesZ.Text);
                        i += 2;
                    } else {
                        int j = i - (temVar + rotVar);
                        acVar[j].fl = Convert.ToSingle(LabAccelX.Text);
                        acVar[j + 1].fl = Convert.ToSingle(LabAccelY.Text);
                        acVar[j + 2].fl = Convert.ToSingle(LabAccelZ.Text);
                        i += 2;
                    }
                }
                for(int i = 0; i < unicanLenght; i++) {
                    if(i < temVar) {
                        test.data[i] = temV[i].byte1;
                        test.data[++i] = temV[i].byte2;
                    } else if(i < temVar + rotVar) {
                        int j = i - temVar;
                        test.data[i] = rootVar[j].byte1;
                        test.data[++i] = rootVar[j].byte2;
                        test.data[++i] = rootVar[j].byte3;
                        test.data[++i] = rootVar[j].byte4;
                    } else {
                        int j = i - (temVar + rotVar);
                        test.data[i] = acVar[j].byte1;
                        test.data[++i] = acVar[j].byte2;
                        test.data[++i] = acVar[j].byte3;
                        test.data[++i] = acVar[j].byte4;
                    }
                }
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }

        private void SendTime(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                const int timeVar = 1 * sizeof(int);
                const int unicanLenght = timeVar;
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingTime.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveTime.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresTime.Text, 16),
                    unicanLength = unicanLenght
                };
                ItUn timeV = new();
                test.data = new byte[unicanLenght];
                // Time
                DateTimeOffset dto = new(DateTime.Now);
                long nowTime = dto.ToUnixTimeSeconds();
                int[] arIValue = new int[5];
                while(nowTime > byte.MaxValue) {
                    nowTime -= byte.MaxValue + 1;
                    arIValue[1] += 0x01;
                    while(arIValue[1] > byte.MaxValue) {
                        arIValue[1] -= byte.MaxValue + 1;
                        arIValue[2] += 0x01;
                        while(arIValue[2] > byte.MaxValue) {
                            arIValue[2] -= byte.MaxValue + 1;
                            arIValue[3] += 0x01;
                            while(arIValue[3] > byte.MaxValue) {
                                arIValue[3] -= byte.MaxValue + 1;
                                arIValue[4] += 0x01;
                            }
                        }
                    }
                }
                arIValue[0] += (int)nowTime;
                // Time
                for(int i = 0; i < timeVar; i++) {
                    timeV.it = arIValue[i];
                    test.data[i] = timeV.byte1;
                }
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }

        private byte countRateSensSend = 0;
        private void SendRateSens(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                // X
                const int RSVar = 1 * sizeof(float);
                const int RSTem = 1 * sizeof(byte);
                const int unicanLenght = RSVar + RSTem + 1;
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingRSX.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveRSX.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresRSX.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn RSXV = new() {
                    fl = Convert.ToSingle(RateSensValueX.Text)
                };
                byte RSXT = Convert.ToByte(RateSensTemperatureX.Text);
                test.data = new byte[unicanLenght] {
                    RSXV.byte1, RSXV.byte2, RSXV.byte3, RSXV.byte4,
                    RSXT,
                    (byte)(countRateSensSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
                // Y
                test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingRSY.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveRSY.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresRSY.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn RSYV = new() {
                    fl = Convert.ToSingle(RateSensValueY.Text)
                };
                byte RSYT = Convert.ToByte(RateSensTemperatureY.Text);
                test.data = new byte[unicanLenght] {
                    RSYV.byte1, RSYV.byte2, RSYV.byte3, RSYV.byte4,
                    RSYT,
                    (byte)(countRateSensSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
                // Z
                test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingRSZ.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveRSZ.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresRSZ.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn RSZV = new() {
                    fl = Convert.ToSingle(RateSensValueZ.Text)
                };
                byte RSZT = Convert.ToByte(RateSensTemperatureZ.Text);
                test.data = new byte[unicanLenght] {
                    RSZV.byte1, RSZV.byte2, RSZV.byte3, RSZV.byte4,
                    RSZT,
                    (byte)(countRateSensSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);

                if(countRateSensSend == 128) countRateSensSend = 0;
                countRateSensSend++;
                CountSendingRateSens.Invoke(new Action(() => {
                    CountSendingRateSens.Text = $"Количество отправок: {countRateSensSend}";
                }));
            }
        }

        private byte countMagSend = 0;
        private void SendMagnetometer(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                // X
                const int MVar = 1 * sizeof(float);
                const int MTem = 1 * sizeof(byte);
                const int unicanLenght = MVar + MTem + 1;
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingMX.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveMX.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresMX.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn MXV = new() {
                    fl = Convert.ToSingle(MagValueX.Text)
                };
                byte MXT = Convert.ToByte(MagTemperatureX.Text);
                test.data = new byte[unicanLenght] {
                    MXV.byte1, MXV.byte2, MXV.byte3, MXV.byte4,
                    MXT,
                    (byte)(countMagSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
                // Y
                test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingMY.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveMY.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresMY.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn MYV = new() {
                    fl = Convert.ToSingle(MagValueY.Text)
                };
                byte MYT = Convert.ToByte(MagTemperatureY.Text);
                test.data = new byte[unicanLenght] {
                    MYV.byte1, MYV.byte2, MYV.byte3, MYV.byte4,
                    MYT,
                    (byte)(countMagSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);
                // Z
                test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingMZ.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveMZ.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresMZ.Text, 16),
                    unicanLength = unicanLenght
                };
                FlUn MZV = new() {
                    fl = Convert.ToSingle(MagValueZ.Text)
                };
                byte MZT = Convert.ToByte(MagTemperatureZ.Text);
                test.data = new byte[unicanLenght] {
                    MZV.byte1, MZV.byte2, MZV.byte3, MZV.byte4,
                    MZT,
                    (byte)(countMagSend << 1)
                };
                CTU.SendWithCAN(test, _serialPort, LogBox);

                if(countMagSend == 128) countMagSend = 0;
                countMagSend++;
                CountSendingMag.Invoke(new Action(() => {
                    CountSendingMag.Text = $"Количество отправок: {countMagSend}";
                }));
            }
        }

        private void SendAdcsBeacon_Click(object? sender, EventArgs? e) {
            if(UseInternet.Checked) {
                //Need?
            } else if(UseCan.Checked) {
                const int time = 1 * sizeof(long);
                const int uptime = 1 * sizeof(int);
                const int eciQuatW = 1 * sizeof(int);
                const int eciQuatX = 1 * sizeof(int);
                const int eciQuatY = 1 * sizeof(int);
                const int eciQuatZ = 1 * sizeof(int);
                const int eciAVX = 1 * sizeof(int);
                const int eciAVY = 1 * sizeof(int);
                const int eciAVZ = 1 * sizeof(int);
                const int orbQuatW = 1 * sizeof(int);
                const int orbQuatX = 1 * sizeof(int);
                const int orbQuatY = 1 * sizeof(int);
                const int orbQuatZ = 1 * sizeof(int);
                const int wheelRpmXPlus = 1 * sizeof(short);
                const int wheelRpmXMinus = 1 * sizeof(short);
                const int wheelRpmYPlus = 1 * sizeof(short);
                const int wheelRpmYMinus = 1 * sizeof(short);
                const int otherVal = (3 * sizeof(int)) + sizeof(byte) + (7 * sizeof(int)) + sizeof(byte) + (3 * sizeof(int)) + (3 * sizeof(byte));
                const int unicanLenght = time + uptime +
                    eciQuatW + eciQuatX + eciQuatY + eciQuatZ + eciAVX + eciAVY + eciAVZ
                    + sizeof(byte) +
                    orbQuatW + orbQuatX + orbQuatY + orbQuatZ
                    + otherVal +
                    wheelRpmXPlus + wheelRpmXMinus + wheelRpmYPlus + wheelRpmYMinus
                    + (3 * sizeof(byte));
                UnicanMessage test = new() {
                    unicanMSGId = Convert.ToUInt16(IdShippingAdcsBeacon.Text, 16),
                    unicanAddressTo = Convert.ToUInt16(AddresReceiveAdcsBeacon.Text, 16),
                    unicanAddressFrom = Convert.ToUInt16(AddresAdcsBeacon.Text, 16),
                    unicanLength = unicanLenght,
                    data = new byte[unicanLenght]
                };
                FlUn quat = new(), AV = new();
                for(int i = 0; i < unicanLenght; i++) {
                    if(i == (time + uptime)) {
                        quat.fl = Convert.ToSingle(eci_quat_w.Text);
                        test.data[i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(eci_quat_vect_x.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(eci_quat_vect_y.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(eci_quat_vect_z.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        AV.fl = Convert.ToSingle(eci_AV_x.Text);
                        test.data[++i] = AV.byte1;
                        test.data[++i] = AV.byte2;
                        test.data[++i] = AV.byte3;
                        test.data[++i] = AV.byte4;
                        AV.fl = Convert.ToSingle(eci_AV_y.Text);
                        test.data[++i] = AV.byte1;
                        test.data[++i] = AV.byte2;
                        test.data[++i] = AV.byte3;
                        test.data[++i] = AV.byte4;
                        AV.fl = Convert.ToSingle(eci_AV_z.Text);
                        test.data[++i] = AV.byte1;
                        test.data[++i] = AV.byte2;
                        test.data[++i] = AV.byte3;
                        test.data[++i] = AV.byte4;
                    } else if(i == (time + uptime +
                            eciQuatW + eciQuatX + eciQuatY + eciQuatZ + eciAVX + eciAVY + eciAVZ +
                            sizeof(byte))) {
                        quat.fl = Convert.ToSingle(orb_quat_w.Text);
                        test.data[i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(orb_quat_vect_x.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(orb_quat_vect_y.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                        quat.fl = Convert.ToSingle(orb_quat_vect_z.Text);
                        test.data[++i] = quat.byte1;
                        test.data[++i] = quat.byte2;
                        test.data[++i] = quat.byte3;
                        test.data[++i] = quat.byte4;
                    } else if(i == (time + uptime +
                            eciQuatW + eciQuatX + eciQuatY + eciQuatZ + eciAVX + eciAVY + eciAVZ +
                            sizeof(byte) +
                            orbQuatW + orbQuatX + orbQuatY + orbQuatZ +
                            otherVal)) {
                        int wheel = Convert.ToInt16(wheel_rpm_x_plus.Text);
                        test.data[i] = (byte)wheel;
                        test.data[++i] = (byte)(wheel >> 8);
                        wheel = Convert.ToInt16(wheel_rpm_x_minus.Text);
                        test.data[++i] = (byte)wheel;
                        test.data[++i] = (byte)(wheel >> 8);
                        wheel = Convert.ToInt16(wheel_rpm_y_plus.Text);
                        test.data[++i] = (byte)wheel;
                        test.data[++i] = (byte)(wheel >> 8);
                        wheel = Convert.ToInt16(wheel_rpm_y_minus.Text);
                        test.data[++i] = (byte)wheel;
                        test.data[++i] = (byte)(wheel >> 8);
                    } else {
                        test.data[i] = 0;
                    }
                }
                CTU.SendWithCAN(test, _serialPort, LogBox);
            }
        }
        // Send data

        private void IPTextBox_TextChanged(object sender, EventArgs e) {
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            Thread.Sleep(1000);
            CancellationToken token = timeSource.Token;
            TaskFactory factory = new(token);
            _ = factory.StartNew(() => {
                Read();
            }, token);
            Thread.Sleep(1000);
        }
        private void Read() {
            try {
                if(flagRead) {
                    char[] data;
                    LogBox.Invoke(new Action(() => {
                        LogBox.Text += "\r\n " + _serialPort.BytesToRead + ": ";
                    }));
                    int byteWrite = 0, offsetByte = _serialPort.BytesToRead;
                    do {
                        int copyByte = byteWrite + offsetByte > _serialPort.BytesToRead ?
                            _serialPort.BytesToRead - byteWrite : offsetByte;
                        data = new char[copyByte];
                        _ = _serialPort.Read(data, 0, copyByte);
                        for(int i = 0; i < data.Length; i++) {
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += $"{data[i]}";
                            }));
                        }

                        byteWrite += copyByte;
                    } while(byteWrite < _serialPort.BytesToRead);
                    //--------------------------------
                    List<UnicanMessage> lUM = new();
                    for(int k = 0; k + 4 < data.Length && k + 8 + (data[k + 4] - '0') <= data.Length;) {
                        LogBox.Invoke(new Action(() => {
                            LogBox.Text += $"\r\nk lenght: {k + 8 + (data[k + 4] - '0')} lenght data: {data.Length}";
                        }));
                        if(data.Length > 0 && data[k] == 't') {
                            string CI = string.Format("{0}{1}{2}", data[k + 1], data[k + 2], data[k + 3]);
                            string byteS = string.Format("{0}{1}{2}{3}", data[k + 5], data[k + 6], data[k + 7], data[k + 8]);
                            CanMessage canBuf = new() {
                                canExtbit = 0,
                                canIdentifier = Convert.ToUInt32(CI, 16),
                                canDLC = Convert.ToSByte(data[k + 4]),
                                data = Convert.FromHexString(byteS)
                            };
                            k += 8 + (data[k + 4] - '0');
                            //---------------------------
                            UnicanMessage test = new();
                            CTU.ConvertCan(ref test, canBuf);
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += "\r\nПринято\r\n";
                                LogBox.Text += $"To - {test.unicanAddressTo:X}; From - {test.unicanAddressFrom:X}; Id - {test.unicanMSGId:X};\r\n";
                            }));
                            lUM.Add(test);
                        } else {
                            k++;
                        }
                        Thread.Sleep(1000);
                    }
                    for(int i = 0; i < lUM.Count; i++) {
                        if(lUM[i].unicanAddressFrom == Convert.ToInt16(AddresReceiveBeacon.Text, 16) &&
                             lUM[i].unicanAddressTo == Convert.ToInt16(AddresBeacon.Text, 16) &&
                             lUM[i].unicanMSGId == Convert.ToInt64(IdReceiveBeacon.Text, 16)) {
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += "\r\nSendBeacon\r\n";
                            }));
                            CancellationToken token = timeSource.Token;
                            SendBeacon_Click(null, null);
                        } else if(lUM[i].unicanAddressFrom == Convert.ToInt16(AddresReceiveExBeacon.Text, 16) &&
                             lUM[i].unicanAddressTo == Convert.ToInt16(AddresExBeacon.Text, 16) &&
                             lUM[i].unicanMSGId == Convert.ToInt64(IdReceiveExBeacon.Text, 16)) {
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += "\r\nSendExBeacon\r\n";
                            }));
                            SendExBeacon_Click(null, null);
                        } else if(lUM[i].unicanAddressFrom == Convert.ToInt16(AddresReceiveTime.Text, 16) &&
                             lUM[i].unicanAddressTo == Convert.ToInt16(AddresTime.Text, 16) &&
                             lUM[i].unicanMSGId == Convert.ToInt64(IdReceiveTime.Text, 16)) {
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += "\r\nSendTime\r\n";
                            }));
                            CancellationToken token = timeSource.Token;
                            SendTime(null, null);
                        } else if(lUM[i].unicanAddressFrom == Convert.ToInt16(AddresReceiveAdcsBeacon.Text, 16) &&
                             lUM[i].unicanAddressTo == Convert.ToInt16(AddresAdcsBeacon.Text, 16) &&
                             lUM[i].unicanMSGId == Convert.ToInt64(IdReceiveAdcsBeacon.Text, 16)) {
                            LogBox.Invoke(new Action(() => {
                                LogBox.Text += "\r\nSendAdcsBeacon\r\n";
                            }));
                            CancellationToken token = timeSource.Token;
                            SendAdcsBeacon_Click(null, null);
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
            catch(Exception ex) {
                LogBox2.Invoke(new Action(() => {
                    LogBox2.Text += ex.Message;
                    LogBox2.Text += "\r\n";
                    LogBox2.Text += ex.StackTrace;
                }));
            }
        }
        private void CANTestWrite_Click(object sender, EventArgs e) {
            LogBox.Text += "\r\nZero code\r\n";
        }
        private void CANTestRead_Click(object sender, EventArgs e) {
            Read();
        }

        private void OpenRKSCAN_Click(object sender, EventArgs e) {
            flagRead = true;
            _serialPort = new(CANPort.Text, 9600, Parity.None, 8, StopBits.One);

            if(!_serialPort.IsOpen) {
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
                try {
                    _serialPort.Open();
                    // Установить скорость
                    _serialPort.Write(string.Format("S{0}\r", CANSpeed.SelectedIndex));
                    Thread.Sleep(100);
                    Read();
                    // Открыть
                    _serialPort.Write("O\r");
                    Thread.Sleep(100);
                    Read();

                    LogBox.Text += "\r\nPort open";
                }
                catch(Exception ex) {
                    LogBox.Text = ex.Message;
                }
            } else {
                LogBox.Text = "serialPort is open";
            }
        }
        private void CloseRKSCAN_Click(object? sender, EventArgs? e) {
            flagRead = false;
            if(_serialPort != null && _serialPort.IsOpen) {
                try {
                    // Закрыть
                    _serialPort.Write("C\r");
                    Thread.Sleep(1000);
                    _serialPort.Close();

                    LogBox.Text = "Port close";
                }
                catch(Exception ex) {
                    LogBox.Text = ex.Message;
                }
            }
        }

        private readonly HandlerLittleD HLD = new();
        private void GetDBFileInfo_Click(object sender, EventArgs e) {
            GF.GetFileInfo(NameDBFile);
        }
        private void ReadDBFile_Click(object sender, EventArgs e) {
            HLD.ReadDBFile(NameDBFile, DBAllText, LogBox2);
        }

        private void FilterComboBox_SelectedIndexChanged(object? sender, EventArgs? e) {
            HLD.SelectedIndexChanged(DBAllText, FilterComboBox, HowFilter, FilterTextBox, LogBox2);
        }
        private void FilterTextBox_TextChanged(object sender, EventArgs e) {
            FilterComboBox_SelectedIndexChanged(null, null);
        }
        private void HowFilter_SelectedIndexChanged(object sender, EventArgs e) {
            FilterComboBox_SelectedIndexChanged(null, null);
        }

        // Automatic dispatch
        private readonly CancellationTokenSource timeSource = new();
        private void AutoTimeStamp_CheckedChanged(object sender, EventArgs e) {
            if(AutoTimeStamp.Checked) {
                LabelTimeSendingPeriod.Visible = true;
                TimeSendingPeriod.Visible = true;
            } else {
                LabelTimeSendingPeriod.Visible = false;
                TimeSendingPeriod.Visible = false;
                TimeSendingPeriod.Text = "";
            }
        }
        private void TimeSendingPeriod_TextChanged(object sender, EventArgs e) {
            if(TimeSendingPeriod.Text.Length == 0) timeSource.Cancel();
            CancellationToken token = timeSource.Token;
            TaskFactory factory = new(token);
            _ = factory.StartNew(() => {
                int sleepTime = Convert.ToInt16(TimeSendingPeriod.Text);
                while(CheckSendingPeriod(TimeSendingPeriod, sleepTime)) {
                    Thread.Sleep(sleepTime * 1000);
                    Debug.WriteLine($"Send time {sleepTime}");
                    SendTime(null, null);
                }
            }, token);
        }

        private CancellationTokenSource rateSensSource = new();
        private void AutoRateSens_CheckedChanged(object sender, EventArgs e) {
            if(AutoRateSens.Checked) {
                LabelRateSensSendingPeriod.Visible = true;
                RateSensSendingPeriod.Visible = true;
            } else {
                LabelRateSensSendingPeriod.Visible = false;
                RateSensSendingPeriod.Visible = false;
                RateSensSendingPeriod.Text = "";
                rateSensSource = new();
            }
        }
        private void RateSensSendingPeriod_TextChanged(object sender, EventArgs e) {
            if(RateSensSendingPeriod.Text.Length == 0) { rateSensSource.Cancel(); return; }
            CancellationToken token = rateSensSource.Token;
            TaskFactory factory = new(token);
            _ = factory.StartNew(() => {
                int sleepTime = Convert.ToInt16(RateSensSendingPeriod.Text);
                while(CheckSendingPeriod(RateSensSendingPeriod, sleepTime)) {
                    Thread.Sleep(sleepTime * 1000);
                    Debug.WriteLine($"Send rate sens {sleepTime}");
                    SendRateSens(null, null);
                }
            }, token);
        }

        private CancellationTokenSource magSource = new();
        private void AutoMag_CheckedChanged(object sender, EventArgs e) {
            if(AutoMag.Checked) {
                LabelMagSendingPeriod.Visible = true;
                MagSendingPeriod.Visible = true;
            } else {
                LabelMagSendingPeriod.Visible = false;
                MagSendingPeriod.Visible = false;
                MagSendingPeriod.Text = "";
                magSource = new();
            }
        }
        private void MagSendingPeriod_TextChanged(object sender, EventArgs e) {
            if(MagSendingPeriod.Text.Length == 0) { magSource.Cancel(); return; }
            CancellationToken token = magSource.Token;
            TaskFactory factory = new(token);
            _ = factory.StartNew(() => {
                int sleepTime = Convert.ToInt16(MagSendingPeriod.Text);
                while(CheckSendingPeriod(MagSendingPeriod, sleepTime)) {
                    Thread.Sleep(sleepTime * 1000);
                    Debug.WriteLine($"Send magnetometer {sleepTime}");
                    SendMagnetometer(null, null);
                }
            }, token);
        }

        private bool CheckSendingPeriod(TextBox sendPerionText, int sleepTime) {
            bool check = false;
            sendPerionText.Invoke(new Action(() => {
                if(_serialPort == null) { LogBox2.Text = "serialPort is no open"; return; }
                if(sendPerionText.TextLength > 0 &&
                sleepTime == Convert.ToInt16(sendPerionText.Text) &&
                _serialPort.IsOpen)
                    check = true;
            }));
            return check;
        }
        // Automatic dispatch

        private void LogBox_TextChanged(object sender, EventArgs e) {
            LogBox.SelectionStart = LogBox.TextLength;
            LogBox.ScrollToCaret();
            if(LogBox.TextLength > LogBox.MaxLength - 1000) {
                LogBox.Text = "";
            }
        }

        // Cutting
        private CuttingFile CF;
        private void GetCutFileInfo_Click(object sender, EventArgs e) {
            string cutFolder = "D:\\CutFile";
            CF = new(PanelCutFileName, DisplayCRCFile, cutFolder,
                Convert.ToInt32(LengthOneFile.Text));
            GF.GetFileInfo(NameCutFile);
        }

        private void ButtonCutFile_Click(object sender, EventArgs e) {
            try {
                CF.ReadFileForCut(NameCutFile.Text);
                CF.InteractiveTextBox();
            }
            catch(Exception ex) {
                LogBox2.Text = ex.Message;
            }
        }

        private void SendCutFile_Click(object sender, EventArgs e) {
            try {
                CF.FlipCutFile(PanelReceiveCutFiles, PanelCutFileName);
            }
            catch(Exception ex) {
                LogBox2.Text = ex.Message;
            }
        }

        private void FlipCutFile_Click(object sender, EventArgs e) {
            try {
                CF.FlipCutFile(PanelCutFileName, PanelReceiveCutFiles);
            }
            catch(Exception ex) {
                LogBox2.Text = ex.Message;
            }
        }

        private void SendReceiveCutFiles_Click(object sender, EventArgs e) {
            try {
                CF.SendCutFile(PanelReceiveCutFiles,
                    Convert.ToUInt16(IdShippingCutFile.Text, 16),
                    Convert.ToUInt16(AddresReceiveCutFile.Text, 16),
                    Convert.ToUInt16(AddresCutFile.Text, 16),
                    _serialPort, LogBox);
            }
            catch(Exception ex) {
                LogBox2.Text = ex.Message;
            }
        }
        // Cutting
    }
}