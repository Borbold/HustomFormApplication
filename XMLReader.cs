using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace HustonRTEMS {
    internal class XMLReader {
        protected XmlTextReader? xmlDoc;
        protected Panel _panelNames, _panelInfo, _panelButton;
        protected Button _toServer;
        protected SerialPort _serialPort;
        protected TextBox _baseSatationAd, _deviceAd;
        protected string _pathXML;

        public XMLReader(string pathXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd) {
            _panelNames = panelNames;
            _panelInfo = panelInfo;
            _pathXML = pathXML;
            _toServer = toServer;
            _panelButton = panelButton;
            _serialPort = serialPort;
            _baseSatationAd = baseSatationAd;
            _deviceAd = deviceAd;
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
            for(int i = 0; i < panel.Controls.Count;)
                if(panel.Controls[i].Name != "BaseStationAd" && panel.Controls[i].Name != "DeviceAd")
                    panel.Controls.RemoveAt(i);
                else
                    i++;
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
                _toServer, _serialPort, _baseSatationAd, _deviceAd, text);
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

    partial class XMLCreator: XMLReader {
        protected readonly CanToUnican CTU = new();
        private string _pacName;
        private int _nameWidth, _infoWidth, _descWidth;
        private List<string> listTypeValue = new();
        private List<object> listInfoBox = new();
        private List<int> listLen = new();

        public XMLCreator(string pathXML, Panel panelNames, Panel panelInfo, Panel panelButton,
                Button toServer, SerialPort serialPort, TextBox baseSatationAd, TextBox deviceAd, string pacName) :
                base(pathXML, panelNames, panelInfo, panelButton, toServer, serialPort, baseSatationAd, deviceAd) {
            _nameWidth = _panelNames.Width - 55;
            _infoWidth = _nameWidth - 100;
            _descWidth = _nameWidth + 100;
            _pacName = pacName;
        }

        public  void MoldInteractPanel(Object? sender, EventArgs e) {
            RemoveAll(_panelButton);
            xmlDoc = new(_pathXML);
            int offsetY = 25;
            Point locName = new(), locTextBox = new(_nameWidth, 0), locDesc = new(_nameWidth + _infoWidth, 0);
            bool checkPacName = false;
            RemoveAll(_panelInfo);
            while(xmlDoc.Read()) {
                if(!checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "PacName") {
                    if(xmlDoc.ReadInnerXml() == _pacName) checkPacName = true;
                }
                if(checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "FldName") {
                    CreateLabel(xmlDoc.ReadInnerXml(), locName,
                        _nameWidth, _panelInfo);
                    locName.Offset(0, offsetY);
                }
                if(checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "FldType") {
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
                if(checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "FldDesc") {
                    CreateLabel(xmlDoc.ReadInnerXml(), locDesc,
                        _descWidth, _panelInfo);
                    locDesc.Offset(0, offsetY);
                }
                if(checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "FldLen")
                    listLen.Add(Convert.ToInt32(xmlDoc.ReadInnerXml()));
                if(checkPacName && xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "PacDesc") {
                    string desc = xmlDoc.ReadInnerXml(), lDesc = "";
                    for(int i = 0, j = 0; i < desc.Length; i += 20, j++) {
                        lDesc += desc.Substring(i, Math.Abs(20 - desc.Length * j));
                        lDesc += "\n";
                    }
                    CreateLabel(lDesc != "" ? lDesc : desc, locName,
                        _descWidth, 200, _panelButton);
                }
                if(xmlDoc.NodeType == XmlNodeType.Element && xmlDoc.Name == "PacName") {
                    if(checkPacName && xmlDoc.ReadInnerXml() != _pacName) break;
                }
            }
            xmlDoc.Close();
            CreateButtonToServer(_toServer, _panelButton, Test);
        }

        private void Test(Object? sender, EventArgs e) {
            for(int i = 0; i < listTypeValue.Count; i++) {
                string? type = listTypeValue[i];
                if(listInfoBox[i] is TextBox) {
                    var l = listInfoBox[i] as TextBox;
                    Debug.WriteLine(string.Format("Type: {0}; Value: {1}; Len: {2}; Dev: {3};", type, l?.Text, listLen[i], _deviceAd.Text));
                } else if(listInfoBox[i] is CheckBox) {
                    var c = listInfoBox[i] as CheckBox;
                    Debug.WriteLine(string.Format("Type: {0}; Value: {1};", type, c?.Checked));
                }
            }
            //CTU.SendWithCAN(test, _serialPort, null);
        }
    }
}
