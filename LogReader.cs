using HustonUI;
using System.Diagnostics;

namespace HustonRTEMS {
    internal class LogReader {
        private string _pathLog;
        private Panel _butPanel;

        public LogReader(string pathLog, Panel butPanel) {
            _pathLog = pathLog;
            _butPanel = butPanel;
        }

        public void ReadLog() {
            FileStream fileStream = File.OpenRead(_pathLog);
            BinaryReader binaryReader = new(fileStream);
            Point location = new();

            int count_str = 500;
            char[] buffer = new char[120];
            binaryReader.Read(buffer);
            buffer = new char[1];
            string str_buf = "";

            CancellationTokenSource logSource = new();
            CancellationToken logToken = logSource.Token;
            TaskFactory logFac = new(logToken);
            _ = logFac.StartNew(() => {
                do {
                    binaryReader.Read(buffer);
                    if(buffer[0].GetType().Name != "Char")
                        break;
                    str_buf += buffer[0];
                    if(buffer[0] == '\n') {
                        string[] str = str_buf.Split(';');
                        str[0] = str[0].Replace('_', '-');
                        string name = $"{str[0]}; {str[4]}; {str[3]}; {str[5]:X};";
                        TextBox newButton = UICreator.CreateTextBox("none", name, location,
                            _butPanel.Width - 20, _butPanel);
                        newButton.ReadOnly = true;
                        newButton.Cursor = Cursors.Default;
                        newButton.MouseDown += new MouseEventHandler(TestDown);
                        location.Offset(0, 20);
                        count_str--;
                        str_buf = "";
                    }
                } while(count_str > 0);
                binaryReader.Close();
                fileStream.Close();
            }, logToken);
        }

        private TextBox previousBut = new();
        private void TestDown(object sender, EventArgs e) {
            previousBut.BackColor = Color.White;
            ((TextBox)sender).BackColor = Color.CadetBlue;
            previousBut = (TextBox)sender;
            _butPanel.Select();
        }
    }
}
