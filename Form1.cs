using MPAPI;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;

namespace HustonRTEMS {
    public partial class MainForm: Form {
        private readonly Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly GeneralFunctional GF = new();
        private readonly DefaultTransmission DT = new();
        private readonly byte[] kissHeader = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0
        };
        private readonly byte[] hardBuf = {
            //0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x01, 0x00, 0x1C, 0x00, 0x00, 0x00, /*0xC0*/ };
        private byte[] hardBufWrite = {
            0xC0, 0x0, 0xA4, 0x64, 82, 0x9C, 0x8C, 0x40, 0x62, 0xA4, 0x64, 0x82, 0x9C, 0x8C, 0x40, 0x61, 0x0, 0xF0,
            0xB0, 0x00, 0x09, 0x00, 0x1C, 0x00, 0x00, 0x00, 0xC0 };
        /*private readonly byte[] canHardBufWrite = { //Get 11 - temperature
             0x74, 0x30, 0x33, 0x43, 0x32, 0x42, 0x30, 0x30, 0x30, 0x0D };*/
        /*private readonly byte[] canHardBufWrite = { //Set
             0x74, 0x34, 0x33, 0x32, 0x34, 0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x34, 0x34, 0x0D };*/
        /*private readonly byte[] canHardBufWrite = { //Get 29
            0x54, 0x42, 0x30, 0x00, 0x00, 0x31, 0x00, 0x31, 0x43, 0x33, 0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x0D };*/
        //74 30 35 43 32 42 30 30
        //30 30 30 30 30 D
        private readonly char[] testCANCharBuf = {
            't', '0', '3', 'C', '2', 'B', '0', '0', '0', '\r',
        };
        private readonly byte[] testInternetBuf = {
            0xB0, 0x00,
            0x1C, 0x00,
            0x01, 0x00,
            0x04, 0x00,
            0x00, 0x98, 0xAD, 0x45,
            0xC0 };
        /*private readonly byte[] canHardBufWrite = { //Get 29
            0x54,
            0x31, 0x37, 0x00, 0x00, 0x31, 0x00, 0x32, 0x38, 0x34,
            0x31, 0x31, 0x32, 0x32, 0x33, 0x33, 0x33, 0x33,
            0x0D };*/
        private readonly byte[] canHardBufWrite = { //Get 11
            0x74,
            0x30, 0x33, 0x43,
            0x32, 0x42, 0x30,
            0x30,
            0x30, 0x30, 0x30, 0x30, 0x30,
            0x0D };
        /*private readonly byte[] canHardBufWrite = { //Set RTR
             0x72, 0x32, 0x38, 0x38, 0x30, 0x0D };*/

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
                    startW + (ActiveForm.Size.Width - startWF) / 2,
                    startH + (ActiveForm.Size.Height - startHF) / 2);
                LogBox.DataBindings.Add(newBinding);
            }
            if(LogBox2.DataBindings.Count == 0) {
                int startWF = ActiveForm.Size.Width;
                int startHF = ActiveForm.Size.Height;
                int startW = LogBox.Width;
                int startH = LogBox.Height;
                Binding newBinding = new("Size", ActiveForm, "Size");
                newBinding.Format += (sender, e) => e.Value = new Size(
                    startW + (ActiveForm.Size.Width - startWF) / 2,
                    startH + (ActiveForm.Size.Height - startHF) / 2);
                LogBox2.DataBindings.Add(newBinding);
            }
        }
        private void Form1_Load(object sender, EventArgs e) {
            AddresTemperature.Text = $"{DT.temperatureTransmission.TShipAddres.addres:X}";
            IdTemperature.Text = $"{DT.temperatureTransmission.TId.getValue[0]:X}";

            AddresAcs.Text = $"{DT.acselerometerTransmission.TShipAddres.addres:X}";
            IdAscelX.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdAscelY.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdAscelZ.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.Z]:X}";
            IdAscelW.Text = $"{DT.acselerometerTransmission.TId.getValue[(int)VarEnum.W]:X}";

            AddresReg.Text = $"{DT.regulationTransmission.TShipAddres.addres:X}";
            IdRegulX.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdRegulY.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdRegulZ.Text = $"{DT.regulationTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            AddresRat.Text = $"{DT.ratesensorTransmission.TShipAddres.addres:X}";
            IdRatesX.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdRatesY.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdRatesZ.Text = $"{DT.ratesensorTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            AddresAcc.Text = $"{DT.accelsensorTransmission.TShipAddres.addres:X}";
            IdAccelX.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.X]:X}";
            IdAccelY.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.Y]:X}";
            IdAccelZ.Text = $"{DT.accelsensorTransmission.TId.getValue[(int)VarEnum.Z]:X}";

            // Mag
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
                PortRTEMS.Text = section.PortRTEMS;
                PortHUSTON.Text = section.PortHUSTON;
                PortHUSTONTelnet.Text = section.PortHUSTONTelnet;
                CANSpeed.Text = section.CANSpeed;
                CANPort.Text = section.CANPort;

                AddresReceiveMag.Text = section.ReceiveMagAddres;
                IdReceiveMag.Text = section.IdReceiveMag;
                IdShippingMag.Text = section.IdShipingMag;
                AddresMag1.Text = section.SensorMagAddress1;
                AddresMag2.Text = section.SensorMagAddress2;

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

                cfg.Save();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if(cfg.GetSection("customProperty") is CustomProperty section) {
                section.IP = IPTextBox.Text;
                section.PortRTEMS = PortRTEMS.Text;
                section.PortHUSTON = PortHUSTON.Text;
                section.PortHUSTONTelnet = PortHUSTONTelnet.Text;
                section.CANSpeed = CANSpeed.Text;
                section.CANPort = CANPort.Text;

                section.ReceiveMagAddres = AddresReceiveMag.Text;
                section.IdReceiveMag = IdReceiveMag.Text;
                section.IdShipingMag = IdShippingMag.Text;
                section.SensorMagAddress1 = AddresMag1.Text;
                section.SensorMagAddress2 = AddresMag2.Text;

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

        private Socket serverListener, client;
        private async void Open_thread() {
            while(flagRead) {
                Thread.Sleep(1000);
                GeneralFunctional.ClearInvokeTextBox(LogBox2);
                GeneralFunctional.InvokeTextBox(LogBox2, "Search socet\r\n");
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
                int raw_buffer_size = kissHeader.Length; // Kiss header
                if(!serverListener.Connected) {
                    try {
                        serverListener.Connect(ipep);
                        GeneralFunctional.InvokeTextBox(LogBox2, $"Socet open\r\n");
                        await serverListener.SendAsync(hardBuf, SocketFlags.None);
                    }
                    catch(Exception ex) {
                        GeneralFunctional.InvokeTextBox(LogBox2, ex.Message + "\r\n");
                    }

                    while(serverListener.Connected) {
                        try {
                            GeneralFunctional.InvokeTextBox(LogBox2, $"Wait message, message_count: ");
                            message_size = await serverListener.ReceiveAsync(buffer, SocketFlags.None);
                            GeneralFunctional.InvokeTextBox(LogBox2, message_size.ToString() + "\r\n");
                        }
                        catch(Exception ex) {
                            GeneralFunctional.InvokeTextBox(LogBox2, ex.Message + "\r\n");
                        }
                        Thread.Sleep(1000);

                        if(message_size > 0) {
                            GeneralFunctional.InvokeTextBox(LogBox2, $"Socket server response message: \r\n");
                            if(CheckBox.Checked) {
                                while(raw_buffer_size < message_size) {
                                    if(raw_buffer_size >= 0) {
                                        GeneralFunctional.InvokeTextBox(LogBox2, $"{buffer[raw_buffer_size]:X} ");
                                    }
                                    GeneralFunctional.WriteChangeKissFESC(ref buffer);
                                    raw_buffer_size++;
                                }
                                raw_buffer_size = 0;
                            } else {
                                raw_buffer_size = kissHeader.Length;
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
                            if(id.it == Convert.ToInt16(IdReceiveMag.Text, 16) &&
                                addresIn.it == Convert.ToInt16(AddresReceiveMag.Text, 16) &&
                                addresOut.it == Convert.ToInt16(AddresMag1.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendMagnetometer1_Click(null, null);
                            } else if(id.it == Convert.ToInt16(IdReceiveMag.Text, 16) &&
                                addresIn.it == Convert.ToInt16(AddresReceiveMag.Text, 16) &&
                                addresOut.it == Convert.ToInt16(AddresMag2.Text, 16)) {
                                LogBox2.Invoke(new Action(() => { LogBox2.Text += "Get information\r\n"; }));
                                for(int i = 0; i < message_size; i++) {
                                    LogBox2.Invoke(new Action(() => { LogBox2.Text += buffer[i] + " "; }));
                                }
                                SendMagnetometer2_Click(null, null);
                            } else if(id.it == Convert.ToInt16(IdReceiveAcs.Text, 16) &&
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
                                if(CheckBox.Checked)
                                    await client.SendAsync(buffer, SocketFlags.None);
                            } else {
                                LogBox2.Invoke(new Action(() => {
                                    LogBox2.Text = string.Format("Wrong address or id.\r\nid:'{0}'\r\nadIn:'{1}'\r\nadOut:'{2}'\r\nmesCount'{3}'",
                                        id.it, addresIn.it, addresOut.it, message_size);
                                }));
                            }
                            message_size = 0;
                        } else {
                            break;
                        }
                        Thread.Sleep(Convert.ToInt16(textBoxDelay.Text) * 1000);
                        GeneralFunctional.InvokeTextBox(LogBox2, $"\r\nWait new message!\r\n");
                    }
                }

                serverListener.Close();
            }
        }
        Thread nThread; bool flagRead;
        private async void OpenSocetServer_ClickAsync(object sender, EventArgs e) {
            flagRead = true;
            nThread = new(Open_thread);
            nThread.Start();

            if(CheckBox.Checked) {
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
                                for(int i = kissHeader.Length; i < mBuffer.Length; i++) {
                                    buffer[i] = mBuffer[i];
                                }
                            }
                            await serverListener.SendAsync(buffer, SocketFlags.None);
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
            if(nThread.IsAlive) {
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
        private void SendTemperature_Click(object sender, EventArgs e) {
            it.it = DT.temperatureTransmission.TShipAddres.addres;

            fl.fl = (float)Convert.ToDouble(LabTemp.Text);
            GeneralFunctional.SendMessageInSocket(serverListener, fl, it,
                hardBufWrite, LogBox);
        }

        private async void SendMagnetometer1_Click(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingMag.Text, 16);
            int addresValue = Convert.ToInt16(AddresMag1.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveMag.Text, 16);
            int iCount = 0;
            int fCount = 3;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabMagX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabMagY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabMagZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }
        private async void SendMagnetometer2_Click(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingMag.Text, 16);
            int addresValue = Convert.ToInt16(AddresMag2.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveMag.Text, 16);
            int iCount = 0;
            int fCount = 3;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabMagX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabMagY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabMagZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }

        private async void SendAcselerometer_Click(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingAcs.Text, 16);
            int addresValue = Convert.ToInt16(AddresAcs.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveAcs.Text, 16);
            int iCount = 0;
            int fCount = 4;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabRotX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabRotY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabRotZ.Text);
            arFValue[3] = (float)Convert.ToDouble(LabRotW.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }

        private async void SendRegulation_Click(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingReg.Text, 16);
            int addresValue = Convert.ToInt16(AddresReg.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveReg.Text, 16);
            int iCount = 0;
            int fCount = 3;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabPosX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabPosY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabPosZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }

        private async void SendRatesensor_Click(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingRat.Text, 16);
            int addresValue = Convert.ToInt16(AddresRat.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveRat.Text, 16);
            int iCount = 0;
            int fCount = 3;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabRatesX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabRatesY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabRatesZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }

        private async void SendAccelsensor_ClickAsync(object? sender, EventArgs? e) {
            int idShipping = Convert.ToInt16(IdShippingAcc.Text, 16);
            int addresValue = Convert.ToInt16(AddresAcc.Text, 16);
            int addresReceive = Convert.ToInt16(AddresReceiveAcc.Text, 16);
            int iCount = 0;
            int fCount = 3;
            int[] arIValue = Array.Empty<int>();
            float[] arFValue = new float[fCount];
            arFValue[0] = (float)Convert.ToDouble(LabAccelX.Text);
            arFValue[1] = (float)Convert.ToDouble(LabAccelY.Text);
            arFValue[2] = (float)Convert.ToDouble(LabAccelZ.Text);
            GeneralFunctional.SendMessageInSocket(serverListener,
                idShipping, addresValue, addresReceive,
                iCount, fCount, arIValue, arFValue,
                LogBox, CheckKISS.Checked);

            if(CheckBox.Checked)
                await client.SendAsync(buffer, SocketFlags.None);
        }
        // Send data

        private void IPTextBox_TextChanged(object sender, EventArgs e) {
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            Thread.Sleep(1000);
            Read();
            Thread.Sleep(1000);
        }
        private void Read() {
            if(flagRead) {
                LogBox.Invoke(new Action(() => {
                    LogBox.Text += "\r\n " + serialPort.BytesToRead + ": ";
                }));
                int byteWrite = 0, offsetByte = serialPort.BytesToRead;
                do {
                    int copyByte = byteWrite + offsetByte > serialPort.BytesToRead ?
                        serialPort.BytesToRead - byteWrite : offsetByte;
                    byte[] data = new byte[copyByte];
                    serialPort.Read(data, 0, copyByte);
                    for(int i = 0; i < data.Length; i++) {
                        LogBox.Invoke(new Action(() => {
                            LogBox.Text += $" {data[i]:X}";
                        }));
                    }
                    if(data.Length > 1)
                        GeneralFunctional.CanToApp11(data);
                    byteWrite += copyByte;
                } while(byteWrite < serialPort.BytesToRead);
            }
        }
        private void CANTestWrite_Click(object sender, EventArgs e) {
            byte[] sendToCan = GeneralFunctional.AppToCan11(testInternetBuf);
            LogBox.Text += "\r\n";
            int byteWrite = 0, offsetByte = 14;
            // Send from 8 bytes
            while(byteWrite < sendToCan.Length) {
                int copyByte = byteWrite + offsetByte >= sendToCan.Length ?
                    sendToCan.Length - byteWrite : offsetByte;
                byte[] data = new byte[copyByte];
                Array.Copy(sendToCan, byteWrite, data, 0, copyByte);
                serialPort?.Write(data, 0, copyByte);
                for(int i = 0; i < data.Length; i++) {
                    LogBox.Text += " " + $"{data[i]:X}";
                }
                byteWrite += offsetByte;
                LogBox.Text += "\r\n";
            }
            LogBox.Text += "\r\n";
        }
        private void CANTestRead_Click(object sender, EventArgs e) {
            Read();
        }

        private SerialPort serialPort;
        private void OpenRKSCAN_Click(object sender, EventArgs e) {
            flagRead = true;
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

                    LogBox.Text += "\r\nPort open";
                }
                catch(Exception ex) {
                    LogBox.Text = ex.Message;
                }
            } else {
                LogBox.Text = "serialPort is open";
            }
        }
        private void CloseRKSCAN_Click(object sender, EventArgs e) {
            flagRead = false;
            if(serialPort.IsOpen) {
                try {
                    // Закрыть
                    serialPort.Write("C\r");
                    Thread.Sleep(1000);
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