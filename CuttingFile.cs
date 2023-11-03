using System.Diagnostics;
using System.IO;
using HustonUI;

namespace HustonRTEMS {
    internal class CuttingFile {
        private Panel _butPanel;
        private TextBox _displayCRC;
        private short _countFile;

        private string _fileName = "Test_";

        public CuttingFile(Panel butPanel, TextBox displayCRC) {
            _butPanel = butPanel;
            _displayCRC = displayCRC;
        }

        public void ReadFileForCut(string pathFile, string cutFolder, int amountBytes) {
            UICreator.RemoveAll(_butPanel);

            FileStream fileStream = File.OpenRead(pathFile);
            BinaryReader binaryReader = new(fileStream);

            byte[] result = binaryReader.ReadBytes((int)fileStream.Length);

            DirectoryInfo directoryInfo = new(cutFolder);
            foreach(FileInfo file in directoryInfo.GetFiles())
                file.Delete();

            _countFile = (short)(result.Length / amountBytes);
            short numberFile = 1;
            while(_countFile >= numberFile) {
                FileStream fileWrite = File.OpenWrite($"{cutFolder}\\{_fileName}{numberFile}");
                BinaryWriter binaryWriter = new(fileWrite);
                int start = (numberFile - 1) * (result.Length / _countFile);
                int end = result.Length / (_countFile - (numberFile - 1));
                for(int i = start; i < end; i++)
                    binaryWriter.Write(result[i]);
                binaryWriter.Close();
                fileWrite.Close();
                numberFile++;
            }

            _displayCRC.Text = Crc16.ComputeCrc(result).ToString();

            binaryReader.Close();
            fileStream.Close();
        }

        public void InteractiveTextBox() {
            CancellationTokenSource logSource = new();
            CancellationToken logToken = logSource.Token;
            TaskFactory logFac = new(logToken);

            _ = logFac.StartNew(() => {
                for(int i = 0; i < _countFile / 2; i++) {
                    string name = $"{_fileName}{i}";
                    TextBox newButton = UICreator.CreateTextBox("none", name, new Point(0, 20 * i),
                        _butPanel.Width - 20, _butPanel);
                    newButton.Invoke(new Action(() => {
                        newButton.ReadOnly = true;
                        newButton.Cursor = Cursors.Default;
                        newButton.MouseDown += new MouseEventHandler(TestDown);
                    }));
                }
            }, logToken);

            _ = logFac.StartNew(() => {
                for(int i = _countFile / 2; i < _countFile; i++) {
                    string name = $"{_fileName}{i}";
                    TextBox newButton = UICreator.CreateTextBox("none", name, new Point(0, 20 * i),
                        _butPanel.Width - 20, _butPanel);
                    newButton.Invoke(new Action(() => {
                        newButton.ReadOnly = true;
                        newButton.Cursor = Cursors.Default;
                        newButton.MouseDown += new MouseEventHandler(TestDown);
                    }));
                }
            }, logToken);
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