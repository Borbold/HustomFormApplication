using HustonUI;
using System.Diagnostics;

namespace HustonRTEMS {
    internal class LogReader {
        private string _pathLog;
        private Panel _butPanel, _infoPanel;

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
                        string name = $"{str[0]}; {str[4]}; {str[3]}; {str[5]:X};";
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
            UICreator.RemoveAll(_infoPanel);
            Point location = new();
            string[] splitString = logSave[numberLog].Split(';');
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
            }
        }
    }
}
