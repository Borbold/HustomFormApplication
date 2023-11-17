using HustonUI;
using MPAPI;
using System.Diagnostics;
using System.Xml;

namespace HustonRTEMS {
    internal class LogReader {
        private string _pathLog;
        private Panel _butPanel, _infoPanel;

        private XmlTextReader? _xmlDoc;

        public LogReader(string pathLog, Panel infoPanel, Panel butPanel) {
            _pathLog = pathLog;
            _infoPanel = infoPanel;
            _butPanel = butPanel;
        }

        private List<string> logSave = new();
        public void ReadLog() {
            FileStream fileStream = File.OpenRead(_pathLog);
            BinaryReader binaryReader = new(fileStream);
            Point location = new();

            char[] header = new char[120];
            binaryReader.Read(header);

            char[] logRes = new char[fileStream.Length - 120];
            binaryReader.Read(logRes);

            CancellationTokenSource logSource = new();
            CancellationToken logToken = logSource.Token;
            TaskFactory logFac = new(logToken);
            string str_buf = "";
            _ = logFac.StartNew(() => {
                int i = 0;
                if(logRes.Length > 20000) {
                    i = logRes.Length - 20000;
                    while(logRes[i] != '\n') i++;
                    i++;
                }

                int iTag = 0;
                for(; i < logRes.Length; i++) {
                    if(logRes[i] == '\r' && logRes[i + 1] == '\n') {
                        logSave.Add(str_buf);
                        string[] str = str_buf.Split(';');
                        str[0] = str[0].Replace('_', '-');
                        string name = $"{str[0]}; {str[3]}; {str[4]}; {str[5]:X};";
                        TextBox newButton = UICreator.CreateTextBox("none", name, location,
                            _butPanel.Width - 20, _butPanel);
                        newButton.Invoke(new Action(() => {
                            newButton.ReadOnly = true;
                            newButton.Cursor = Cursors.Default;
                            newButton.MouseDown += new MouseEventHandler(TextSelection);
                            newButton.Tag = iTag;
                        }));
                        location.Offset(0, 20);
                        str_buf = "";
                        iTag++;
                        i = (i + 2) > logRes.Length ? i + 2 : i;
                    }
                    str_buf += logRes[i];
                }
                binaryReader.Close();
                fileStream.Close();
            }, logToken);
        }

        private TextBox previousBut = new();
        private void TextSelection(object sender, EventArgs e) {
            previousBut.BackColor = Color.WhiteSmoke;
            ((TextBox)sender).BackColor = Color.CadetBlue;
            previousBut = (TextBox)sender;
            _butPanel.Select();
            OutputInformationLog((int)previousBut.Tag);
        }

        private void OutputInformationLog(int numberLog) {
            string[] splitString = logSave[numberLog].Split(';');
            splitString[5] = splitString[5][0] == ' ' ? splitString[5].Substring(1) : splitString[5];

            _xmlDoc = new("C:\\Users\\Ivar\\Documents\\SX-Houston-app_v214\\resources\\maps\\map.xml");
            string devModel = "";
            while(_xmlDoc.Read()) {
                if(_xmlDoc.NodeType == XmlNodeType.Element) {
                    if(_xmlDoc.Name == "DevModel") {
                        devModel = _xmlDoc.ReadInnerXml();
                    }
                    if(_xmlDoc.Name == "DevAddress") {
                        string devAdd = _xmlDoc.ReadInnerXml();
                        if(devAdd == ("0x" + splitString[3])) {
                            break;
                        } else {
                            devModel = "";
                        }
                    }
                }
            }
            _xmlDoc.Close();

            UICreator.RemoveAll(_infoPanel);
            _xmlDoc = new($"C:\\Users\\Ivar\\Documents\\SX-Houston-app_v214\\resources\\devices\\{devModel}.xml");
            int nameW = _infoPanel.Width - 300, infoW = nameW;
            int offsetY = 25, fieldOffset = 0;
            Point locName = new(), locInfo = new(nameW, 0);
            while(_xmlDoc.Read()) {
                if(_xmlDoc.NodeType == XmlNodeType.Element) {
                    if(_xmlDoc.Name == "PacId") {
                        string pacId = _xmlDoc.ReadInnerXml();
                        if(Convert.ToInt16(pacId, 16) == Convert.ToInt16(splitString[5], 16)) {
                            while(_xmlDoc.Read()) {
                                if(_xmlDoc.Name == "Packet") {
                                    break;
                                } else {
                                    if(_xmlDoc.Name == "FldName") {
                                        UICreator.CreateLabel(_xmlDoc.ReadInnerXml(), locName,
                                            nameW, _infoPanel);
                                        locName.Offset(0, offsetY);
                                    }

                                    if(_xmlDoc.Name == "FldOffset")
                                        fieldOffset = Convert.ToInt16(_xmlDoc.ReadInnerXml()) / 4;
                                    if(_xmlDoc.NodeType != XmlNodeType.EndElement && _xmlDoc.Name == "FldLen") {
                                        int fieldLen = Convert.ToInt16(_xmlDoc.ReadInnerXml()) / 4;
                                        long val = Convert.ToInt64(splitString[8].Substring(fieldOffset, fieldLen), 16);
                                        UICreator.CreateTextBox("none", val.ToString(), locInfo,
                                            infoW, _infoPanel);
                                        locInfo.Offset(0, offsetY);
                                        fieldOffset = -1;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            _xmlDoc.Close();

            /*Point location = new();
            int countValue = Convert.ToInt16(splitString[6]);
            List<string> value = new();
            string lv = "";
            for(int i = 0; i < splitString[8].Length; i++) {
                lv += splitString[8][i];
                if((i + 1) % 2 == 0) {
                    value.Add(lv);
                    lv = "";
                }
            }
            for(int i = 0; i < countValue; i++) {
                UICreator.CreateLabel($"Val{i + 1}: {Convert.ToInt16(value[i], 16)}", location, _infoPanel.Width, _infoPanel);
                location.Offset(0, 20);
            }*/
        }
    }
}
