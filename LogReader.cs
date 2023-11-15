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

            char[] buffer = new char[120];
            binaryReader.Read(buffer);
            buffer = new char[1];

            char[] first = new char[10000];
            binaryReader.Read(first);

            CancellationTokenSource logSource = new();
            CancellationToken logToken = logSource.Token;
            TaskFactory logFac = new(logToken);
            string str_buf = "";
            _ = logFac.StartNew(() => {
                for(int i = 0; i < first.Length; i++) {
                    char buf = first[i];
                    str_buf += buf;
                    if(buf == '\n') {
                        str_buf = str_buf.Remove(str_buf.Length - 2);
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
                        }));
                        location.Offset(0, 20);
                        str_buf = "";
                    }
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
            OutputInformationLog(previousBut.Location.Y / 20);
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
                UICreator.CreateLabel($"Val{i}: {value[i]:X}", location, _infoPanel.Width, _infoPanel);
                location.Offset(0, 20);
            }
        }
    }
}
