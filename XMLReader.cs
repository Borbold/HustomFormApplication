using System.Diagnostics;
using System.IO.Ports;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace HustonRTEMS {
    internal class XMLReader {
        protected XmlTextReader? xmlDoc;
        protected Panel _panelNames, _panelInfo, _panelButton;
        protected Button _toServer;
        protected SerialPort _serialPort;
        protected TextBox _baseSatationAd, _deviceAd, _logBox;
        protected string _pathXML;

        public XMLReader(string pathXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd,
                TextBox logBox) {
            _panelNames = panelNames;
            _panelInfo = panelInfo;
            _pathXML = pathXML;
            _toServer = toServer;
            _panelButton = panelButton;
            _serialPort = serialPort;
            _baseSatationAd = baseSatationAd;
            _deviceAd = deviceAd;
            _logBox = logBox;
        }

        public void MoldButtonName() {
            xmlDoc = new(_pathXML);
            Point location = new();
            while(xmlDoc.Read()) {
                if(xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "PacName") {
                    CreateButton(xmlDoc.ReadElementContentAsString(), location, _panelNames.Width - 20, _panelNames);
                    location.Offset(0, 20);
                }
            }
            xmlDoc.Close();
        }

        protected void RemoveAll(Panel panel) {
            for(int i = 0; i < panel.Controls.Count;) {
                if(panel.Controls[i].Tag != null && panel.Controls[i].Tag.ToString() != "NotInvisible")
                    panel.Controls.RemoveAt(i);
                else
                    i++;
            }
        }
        protected void CreateLabel(string text, Point location, int width, Panel page) {
            Label newLab = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newLab);
        }
        protected void CreateLabel(string text, Point location, int width, int height, Panel page) {
            Label newLab = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
                Height = height,
            };
            page.Controls.Add(newLab);
        }
        protected TextBox CreateTextBox(string name, string text, Point location, int width, Panel page) {
            TextBox newTextBox = new() {
                Name = name,
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newTextBox);
            return newTextBox;
        }
        protected CheckBox CreateCheckBox(string name, Point location, int width, Panel page) {
            CheckBox newCheckBox = new() {
                Name = name,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            page.Controls.Add(newCheckBox);
            return newCheckBox;
        }
        protected void CreateButton(string text, Point location, int width, Panel page) {
            Button newButton = new() {
                Text = text,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                Location = location,
                Width = width,
            };
            XMLCreator creator = new(_pathXML, _panelNames, _panelInfo, _panelButton,
                _toServer, _serialPort, _baseSatationAd, _deviceAd, text, _logBox);
            newButton.Click += new EventHandler(creator.MoldInteractPanel);
            page.Controls.Add(newButton);
        }
        protected void CreateButtonToServer(Button toButton, Panel page, EventHandler toEvent) {
            Button newButton = new() {
                Location = toButton.Location,
                Text = toButton.Text,
                Font = toButton.Font,
                Visible = true,
            };
            newButton.Click += new EventHandler(toEvent);
            page.Controls.Add(newButton);
        }
    }

    internal partial class XMLCreator: XMLReader {
        protected readonly CanToUnican CTU = new();
        private readonly string _pacName;
        private readonly int _nameWidth, _infoWidth, _descWidth;
        private readonly List<string> listTypeValue = new();
        private readonly List<object> listInfoBox = new();
        private readonly List<ushort> listLen = new(), listOff = new();
        private ushort _pacId = 0;

        public XMLCreator(string pathXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd, string pacName,
                TextBox logBox) :
                base(pathXML, panelNames, panelInfo, panelButton, toServer, serialPort, baseSatationAd, deviceAd,
                    logBox) {
            _nameWidth = _panelNames.Width - 55;
            _infoWidth = _nameWidth - 100;
            _descWidth = _nameWidth + 100;
            _pacName = pacName;
        }

        public void MoldInteractPanel(object? sender, EventArgs e) {
            RemoveAll(_panelButton);
            xmlDoc = new(_pathXML);
            string comType = "";
            int offsetY = 25;
            Point locName = new(), locTextBox = new(_nameWidth, 0), locDesc = new(_nameWidth + _infoWidth, 0);
            bool checkPacName = false;
            RemoveAll(_panelInfo);
            while(xmlDoc.Read()) {
                if(xmlDoc.NodeType == XmlNodeType.Element) {
                    if(!checkPacName && xmlDoc.Name == "PacName") {
                        if(xmlDoc.ReadInnerXml() == _pacName) checkPacName = true;
                    }
                    if(checkPacName) {
                        if(xmlDoc.Name == "PacType") {
                            comType = xmlDoc.ReadInnerXml();
                        }
                        if(xmlDoc.Name == "PacId") {
                            _pacId = Convert.ToUInt16(xmlDoc.ReadInnerXml(), 16);
                            switch(comType) {
                                case "command":
                                    _deviceAd.Text = "0x1";
                                    // TODO: modify to sample from the text in xml
                                    _baseSatationAd.Text = "0x9";
                                    break;
                                case "datain":
                                    _baseSatationAd.Text = "0x1";
                                    // TODO: modify to sample from the text in xml
                                    _deviceAd.Text = "0x9";
                                    break;
                            }
                        }
                        if(xmlDoc.Name == "FldName") {
                            CreateLabel(xmlDoc.ReadInnerXml(), locName,
                                _nameWidth, _panelInfo);
                            locName.Offset(0, offsetY);
                        }
                        if(xmlDoc.Name == "FldType") {
                            listTypeValue.Add(xmlDoc.ReadInnerXml());
                            if(listTypeValue.Last() != "bit") {
                                listInfoBox.Add(CreateTextBox(string.Format("Value{0}", listTypeValue.Count), "0", locTextBox,
                                    _infoWidth, _panelInfo));
                            } else {
                                listInfoBox.Add(CreateCheckBox(string.Format("Value{0}", listTypeValue.Count), locTextBox,
                                    _infoWidth, _panelInfo));
                            }
                            locTextBox.Offset(0, offsetY);
                        }
                        if(xmlDoc.Name == "FldDesc") {
                            CreateLabel(xmlDoc.ReadInnerXml(), locDesc,
                                _descWidth, _panelInfo);
                            locDesc.Offset(0, offsetY);
                        }
                        if(xmlDoc.Name == "FldLen")
                            listLen.Add(Convert.ToUInt16(xmlDoc.ReadInnerXml()));
                        if(xmlDoc.Name == "FldOffset")
                            listOff.Add(Convert.ToUInt16(xmlDoc.ReadInnerXml()));
                        if(xmlDoc.Name == "PacDesc") {
                            string desc = xmlDoc.ReadInnerXml(), lDesc = "";
                            for(int i = 0, j = 0; i < desc.Length; i += 20, j++) {
                                lDesc += desc.Substring(i, Math.Abs(20 - (desc.Length * j)));
                                lDesc += "\n";
                            }
                            CreateLabel(lDesc != "" ? lDesc : desc, locName,
                                _descWidth, 200, _panelButton);
                        }
                    }
                    if(xmlDoc.Name == "PacName") {
                        if(checkPacName && xmlDoc.ReadInnerXml() != _pacName) break;
                    }
                }
            }
            xmlDoc.Close();
            CreateButtonToServer(_toServer, _panelButton, Test);
        }

        private void Test(object? sender, EventArgs e) {
            ushort unicanBitsLenght = 0;
            List<byte> dataBytes = new();
            for(int i = 0; i < listTypeValue.Count; i++) {
                string? type = listTypeValue[i];
                if(listInfoBox[i] is TextBox) {
                    TextBox? l = listInfoBox[i] as TextBox;
                    Debug.WriteLine(string.Format("Type: {0}; Value: {1}; Len: {2}; Dev: {3};", type, l.Text, listLen[i], _deviceAd.Text));
                    unicanBitsLenght += listLen[i];
                    byte[] getB = GetBytes(l.Text, (int)Math.Ceiling(listLen[i] / 8.0), type);
                    for(int j = 0; j < Math.Ceiling(listLen[i] / 8.0); j++) {
                        dataBytes.Add(getB[j]);
                    }
                }
            }
            for(int i = 0; i < listTypeValue.Count; i++) {
                string? type = listTypeValue[i];
                if(listInfoBox[i] is CheckBox) {
                    CheckBox? c = listInfoBox[i] as CheckBox;
                    Debug.WriteLine(string.Format("Type: {0}; Value: {1};", type, c.Checked));
                    unicanBitsLenght += listLen[i];
                    for(int j = i; j < listTypeValue.Count; j++) {
                        if(listInfoBox[j] is TextBox) {
                            int k = listOff[j] / 8;
                            dataBytes[k] = (byte)(dataBytes[k] << 1);
                            dataBytes[k] |= (byte)(c.Checked ? 1 : 0);
                            break;
                        }
                    }
                }
            }
            // Bits in bytes
            ushort unicanLenght = (ushort)(unicanBitsLenght / 8);
            UnicanMessage test = new() {
                unicanMSGId = _pacId,
                unicanAddressTo = Convert.ToUInt16(_baseSatationAd.Text, 16),
                unicanAddressFrom = Convert.ToUInt16(_deviceAd.Text, 16),
                unicanLength = unicanLenght,
                data = new byte[unicanLenght]
            };
            for(int i = 0; i < dataBytes.Count; i++) {
                byte b = dataBytes[i];
                test.data[i] = b;
            }
            CTU.SendWithCAN(test, _serialPort, _logBox);
        }

        private byte[] GetBytes(string text, int lenght, string type) {
            int locL = 0;
            byte[] bytes = new byte[lenght];
            switch(type) {
                case "float":
                    float getFloat = Convert.ToSingle(text);
                    FlUn flUn = new() {
                        fl = getFloat
                    };
                    bytes[0] = flUn.byte1;
                    bytes[1] = flUn.byte2;
                    bytes[2] = flUn.byte3;
                    bytes[3] = flUn.byte4;
                    break;
                case "int":
                    int getInt = Convert.ToInt32(text);
                    bytes[locL] = (byte)getInt;         locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getInt >> 8);  locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getInt >> 16); locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getInt >> 24);
                    break;
                case "uint":
                    uint getUInt = Convert.ToUInt32(text);
                    bytes[locL] = (byte)getUInt;        locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getUInt >> 8); locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getUInt >> 16);locL++;
                    if(locL >= lenght) break;
                    bytes[locL] = (byte)(getUInt >> 24);
                    break;
            }
            return bytes;
        }
    }
}
