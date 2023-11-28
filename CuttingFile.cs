﻿using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Security.Policy;
using System.Xml.Linq;
using HustonUI;

namespace HustonRTEMS {
    internal class CuttingFile {
        private readonly CanToUnican CTU = new();
        private Panel _butPanel;
        private TextBox _displayCRC;
        private short _countFile;
        private string _cutFolder;
        private int _lengthFile;

        private string _fileName = "Test_";

        public CuttingFile(Panel butPanel, TextBox displayCRC, string cutFolder, int lengthFile) {
            _butPanel = butPanel;
            _displayCRC = displayCRC;
            _cutFolder = cutFolder;
            _lengthFile = lengthFile;
        }

        public void ReadFileForCut(string pathFile) {
            UICreator.RemoveAll(_butPanel);

            FileStream fileStream = File.OpenRead(pathFile);
            BinaryReader binaryReader = new(fileStream);
            byte[] result = binaryReader.ReadBytes((int)fileStream.Length);

            DirectoryInfo directoryInfo = new(_cutFolder);
            foreach(FileInfo file in directoryInfo.GetFiles())
                file.Delete();

            _countFile = (short)(result.Length / _lengthFile);
            short numberFile = 1;
            while(_countFile >= numberFile) {
                FileStream fileWrite = File.OpenWrite($"{_cutFolder}\\{_fileName}{numberFile}");
                BinaryWriter binaryWriter = new(fileWrite);
                int start = _lengthFile * (numberFile - 1);
                int end = (_lengthFile * numberFile) > result.Length ? result.Length : _lengthFile * numberFile;
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
                    TextBox newButton = UICreator.CreateTextBox($"{_cutFolder}\\{name}", name, new Point(0, 20 * i),
                        _butPanel.Width - 20, _butPanel);
                    newButton.Invoke(new Action(() => {
                        newButton.ReadOnly = true;
                        newButton.Cursor = Cursors.Default;
                        newButton.MouseDown += new MouseEventHandler(TextSelection);
                        newButton.Tag = i;
                    }));
                }
            }, logToken);

            _ = logFac.StartNew(() => {
                for(int i = _countFile / 2; i < _countFile; i++) {
                    string name = $"{_fileName}{i}";
                    TextBox newButton = UICreator.CreateTextBox($"{_cutFolder}\\{name}", name, new Point(0, 20 * i),
                        _butPanel.Width - 20, _butPanel);
                    newButton.Invoke(new Action(() => {
                        newButton.ReadOnly = true;
                        newButton.Cursor = Cursors.Default;
                        newButton.MouseDown += new MouseEventHandler(TextSelection);
                        newButton.Tag = i;
                    }));
                }
            }, logToken);
        }
        private TextBox previousBut = new();
        private void TextSelection(object sender, EventArgs e) {
            previousBut.BackColor = Color.WhiteSmoke;
            ((TextBox)sender).BackColor = Color.CadetBlue;
            previousBut = (TextBox)sender;
            _butPanel.Select();
        }

        public void FlipCutFile(Panel sendPanel, Panel resemblPanel) {
            if(previousBut.Text != "") {
                sendPanel.AutoScroll = false;
                resemblPanel.Controls.Remove(previousBut);
                sendPanel.AutoScroll = true;
                TextBox newButton = UICreator.CreateTextBox(previousBut.Name, previousBut.Text,
                    new Point(0, 20 * (int)previousBut.Tag), sendPanel.Width - 20, sendPanel);
                newButton.ReadOnly = true;
                newButton.Cursor = Cursors.Default;
                newButton.MouseDown += new MouseEventHandler(TextSelection);
                newButton.Tag = previousBut.Tag;
            }
        }

        public void FlipCutFileAll(Panel sendPanel, Panel resemblPanel) {
            Control[] listC = new Control[resemblPanel.Controls.Count];
            for(int i = 0; i < resemblPanel.Controls.Count; i++)
                listC[i] = resemblPanel.Controls[i];
            for(int i = 0; i < listC.Length; i++) {
                Control locC = listC[i];
                sendPanel.AutoScroll = false;
                resemblPanel.Controls.Remove(locC);
                sendPanel.AutoScroll = true;
                TextBox newButton = UICreator.CreateTextBox(locC.Name, locC.Text,
                    new Point(0, 20 * (int)locC.Tag), sendPanel.Width - 20, sendPanel);
                newButton.ReadOnly = true;
                newButton.Cursor = Cursors.Default;
                newButton.MouseDown += new MouseEventHandler(TextSelection);
                newButton.Tag = locC.Tag;
            }
        }

        public void SendCutFile(Panel sendPanel,
                ushort id, ushort addressTo, ushort addressFrom,
                SerialPort serialPort, TextBox logBox) {
            Control.ControlCollection sendControl = sendPanel.Controls;
            for(int i = 0; i < sendControl.Count; i++) {
                string path = sendControl[i].Name.ToString();

                FileStream fileStream = File.OpenRead(path);
                BinaryReader binaryReader = new(fileStream);
                byte[] result = binaryReader.ReadBytes((int)fileStream.Length);

                ushort unicanLenght = (ushort)(result.Length * sizeof(byte));
                UnicanMessage test = new() {
                    unicanMSGId = id,
                    unicanAddressTo = addressTo,
                    unicanAddressFrom = addressFrom,
                    unicanLength = unicanLenght,
                    data = new byte[unicanLenght]
                };
                ItUn[] byteV = new ItUn[unicanLenght];
                for(int k = 0; k < unicanLenght; k++) {
                    byteV[k].it = Convert.ToInt16(result[k]);
                }
                for(int k = 0; k < unicanLenght; k++) {
                    test.data[k] = byteV[k].byte1;
                    test.data[++k] = byteV[k].byte2;
                }
                binaryReader.Close();
                fileStream.Close();
                Thread.Sleep(500);
                CTU.SendWithCAN(test, serialPort, logBox);
            }
        }
    }
}