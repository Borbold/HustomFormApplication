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
                        string[] str = str_buf.Split(';');
                        str[0] = str[0].Replace('_', '-');
                        string name = $"{str[0]}; {str[4]}; {str[3]}; {str[5]:X};";
                        TextBox newButton = UICreator.CreateTextBox("none", name, location,
                            _butPanel.Width - 20, _butPanel);
                        newButton.Invoke(new Action(() => {
                            newButton.ReadOnly = true;
                            newButton.Cursor = Cursors.Default;
                            newButton.MouseDown += new MouseEventHandler(TestDown);
                        }));
                        location.Offset(0, 20);
                        str_buf = "";
                    }
                }
                binaryReader.Close();
                fileStream.Close();
            }, logToken);

            /*char[] second = new char[10000];
            binaryReader.Read(second);
            CancellationTokenSource logSource2 = new();
            CancellationToken logToken2 = logSource.Token;
            TaskFactory logFac2 = new(logToken);
            string str_buf2 = "";
            _ = logFac2.StartNew(() => {
                for(int i = 0; i < second.Length; i++) {
                    char buf = second[i];
                    str_buf2 += buf;
                    if(buf == '\n') {
                        string[] str = str_buf2.Split(';');
                        str[0] = str[0].Replace('_', '-');
                        string name = $"{str[0]}; {str[4]}; {str[3]}; {str[5]:X};";
                        TextBox newButton = UICreator.CreateTextBox("none", name, location,
                            _butPanel.Width - 20, _butPanel);
                        newButton.Invoke(new Action(() => {
                            newButton.ReadOnly = true;
                            newButton.Cursor = Cursors.Default;
                            newButton.MouseDown += new MouseEventHandler(TestDown);
                        }));
                        location.Offset(0, 20);
                        str_buf2 = "";
                    }
                }
                while(!firstOver) { }
                binaryReader.Close();
                fileStream.Close();
            }, logToken2);*/
        }

        private TextBox previousBut = new();
        private void TestDown(object sender, EventArgs e) {
            previousBut.BackColor = Color.WhiteSmoke;
            ((TextBox)sender).BackColor = Color.CadetBlue;
            previousBut = (TextBox)sender;
            _butPanel.Select();
        }
    }
}
