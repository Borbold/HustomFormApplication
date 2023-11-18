using System.Diagnostics;
using System.IO.Ports;
using System.Xml;
using HustonUI;

namespace HustonRTEMS {
    internal class XMLReader {
        private XmlTextReader? _xmlDoc;

        protected Panel _panelNames, _panelInfo, _panelButton;
        protected Button _toServer;
        protected SerialPort _serialPort;
        protected TextBox _baseSatationAd, _deviceAd, _logBox;
        protected string[] _namesXML;

        public XMLReader(string[] namesXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd,
                TextBox logBox) {
            _panelNames = panelNames;
            _panelInfo = panelInfo;
            _namesXML = namesXML;
            _toServer = toServer;
            _panelButton = panelButton;
            _serialPort = serialPort;
            _baseSatationAd = baseSatationAd;
            _deviceAd = deviceAd;
            _logBox = logBox;
        }

        public void MoldComboBoxName() {
            Point location = new();
            int numberBut = 0;
            foreach (string name in _namesXML) {
                string[] splitName = name.Split('\\');
                Button newButton = UICreator.CreateButton(splitName[^1], location, _panelNames.Width - 20, _panelNames);
                newButton.Tag = numberBut;
                newButton.Click += new EventHandler(MoldButtonName);
                location.Offset(0, 20);
                numberBut++;
            }
        }

        private void MoldButtonName(object? sender, EventArgs e) {
            UICreator.RemoveAll(_panelNames);
            Point location = new();

            Button backButton = UICreator.CreateButton("Back", location, _panelNames.Width - 20, _panelNames);
            backButton.Click += new EventHandler((object ? sender, EventArgs e) => {
                UICreator.RemoveAll(_panelNames);
                MoldComboBoxName();
            });
            location.Offset(0, 20);

            int numberBut = 0;
            _xmlDoc = new(_namesXML[(int)(sender as Control).Tag]);
            while(_xmlDoc.Read()) {
                if(_xmlDoc.NodeType == XmlNodeType.Element && _xmlDoc.Name == "PacName") {
                    string text = _xmlDoc.ReadElementContentAsString();
                    XMLCreator creator = new(_namesXML, _panelNames, _panelInfo, _panelButton,
                        _toServer, _serialPort, _baseSatationAd, _deviceAd, text, _logBox);
                    Button newButton = UICreator.CreateButton(text, location, _panelNames.Width - 20, _panelNames);
                    newButton.Tag = (sender as Control).Tag;
                    newButton.Click += new EventHandler(creator.MoldInteractPanel);
                    location.Offset(0, 20);
                }
            }
            _xmlDoc.Close();
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
        private XmlTextReader? _xmlDoc;

        protected readonly CanToUnican CTU = new();
        private readonly string _pacName;
        private readonly int _nameWidth, _infoWidth, _descWidth;
        private readonly List<string> listTypeValue = new();
        private readonly List<object> listInfoBox = new();
        private readonly List<ushort> listLen = new(), listOff = new();
        private ushort _pacId = 0;

        public XMLCreator(string[] pathXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd, string pacName,
                TextBox logBox) :
                base(pathXML, panelNames, panelInfo, panelButton, toServer, serialPort, baseSatationAd, deviceAd,
                    logBox) {
            _nameWidth = _panelNames.Width - 75;
            _infoWidth = _nameWidth - 100;
            _descWidth = _nameWidth + 100;
            _pacName = pacName;
        }

        public void MoldInteractPanel(object? sender, EventArgs e) {
            UICreator.RemoveAll(_panelButton);
            _xmlDoc = new(_namesXML[(int)(sender as Control).Tag]);
            string comType = "";
            int offsetY = 25;
            Point locName = new(), locTextBox = new(_nameWidth, 0), locDesc = new(_nameWidth + _infoWidth, 0);
            bool checkPacName = false;
            UICreator.RemoveAll(_panelInfo);
            while(_xmlDoc.Read()) {
                if(_xmlDoc.NodeType == XmlNodeType.Element) {
                    if(!checkPacName && _xmlDoc.Name == "PacName") {
                        if(_xmlDoc.ReadInnerXml() == _pacName) checkPacName = true;
                    }
                    if(checkPacName) {
                        if(_xmlDoc.Name == "PacType") {
                            comType = _xmlDoc.ReadInnerXml();
                        }
                        if(_xmlDoc.Name == "PacId") {
                            _pacId = (ushort)Convert.ToUInt32(_xmlDoc.ReadInnerXml(), 16);
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
                        if(_xmlDoc.Name == "FldName") {
                            UICreator.CreateLabel(_xmlDoc.ReadInnerXml(), locName,
                                _nameWidth, _panelInfo);
                            locName.Offset(0, offsetY);
                        }
                        if(_xmlDoc.Name == "FldType") {
                            listTypeValue.Add(_xmlDoc.ReadInnerXml());
                            if(listTypeValue.Last() != "bit") {
                                listInfoBox.Add(UICreator.CreateTextBox(string.Format("Value{0}", listTypeValue.Count), "0", locTextBox,
                                    _infoWidth, _panelInfo));
                            } else {
                                listInfoBox.Add(UICreator.CreateCheckBox(string.Format("Value{0}", listTypeValue.Count), locTextBox,
                                    _infoWidth, _panelInfo));
                            }
                            locTextBox.Offset(0, offsetY);
                        }
                        if(_xmlDoc.Name == "FldDesc") {
                            UICreator.CreateLabel(_xmlDoc.ReadInnerXml(), locDesc,
                                _descWidth, _panelInfo);
                            locDesc.Offset(0, offsetY);
                        }
                        if(_xmlDoc.Name == "FldLen")
                            listLen.Add(Convert.ToUInt16(_xmlDoc.ReadInnerXml()));
                        if(_xmlDoc.Name == "FldOffset")
                            listOff.Add(Convert.ToUInt16(_xmlDoc.ReadInnerXml()));
                        if(_xmlDoc.Name == "PacDesc") {
                            string desc = _xmlDoc.ReadInnerXml(), lDesc = "";
                            for(int i = 0, j = 20; desc.Length >= 20 && i < desc.Length; i += 20) {
                                int k = j + i > desc.Length ? desc.Length - i : j;
                                lDesc += desc.Substring(i, k);
                                lDesc += "\n";
                            }
                            UICreator.CreateLabel(lDesc != "" ? lDesc : desc, new Point(0, 0),
                                _descWidth, 200, _panelButton);
                        }
                    }
                    if(_xmlDoc.Name == "PacName") {
                        if(checkPacName && _xmlDoc.ReadInnerXml() != _pacName) break;
                    }
                }
            }
            _xmlDoc.Close();
            CreateButtonToServer(_toServer, _panelButton, SendTo);
        }

        private void SendTo(object? sender, EventArgs e) {
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
