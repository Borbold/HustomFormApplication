using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustonRTEMS {
    internal class HandlerLittleD {
        public readonly string[] variableNameLD = {
            "Time", "Plate id", "Sense id", "Value", "Second"
        };
        private readonly GeneralFunctional GF = new();

        public void GetDBFileInfo(TextBox nameDBFile) {
            OpenFileDialog ofd = new() {
                Title = "Select file",
                Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt*",
                FilterIndex = 1,
            };
            if(ofd.ShowDialog() == DialogResult.OK) {
                nameDBFile.Text = ofd.FileName;
            }
        }

        public void ReadDBFile(TextBox nameDBFile, RichTextBox DBAllText, TextBox logBox) {
            try {
                FileStream fileStream = File.OpenRead(nameDBFile.Text);
                BinaryReader binaryReader = new(fileStream);

                byte[] result = binaryReader.ReadBytes((int)fileStream.Length);
                int index = 0;
                for(int i = 0; i < result.Length; i++) {
                    if(result[i] == '_' && result[i + 1] == '_') {
                        index = i + 13;
                    }
                }

                uint intT = 0;
                FlUn floatV = new();
                for(int i = index, j = 0; i < result.Length; j = 0, i += 5) {
                    if(j == 0) {
                        intT |= result[i];
                        intT |= (uint)result[i + 1] << 8;
                        intT |= (uint)result[i + 2] << 16;
                        intT |= (uint)result[i + 3] << 24;
                        DateTime dt = new();
                        dt = dt.AddSeconds(intT);
                        dt = dt.AddYears(1969);
                        DBAllText.Text += $"{variableNameLD[0]}: ";
                        DBAllText.Text += dt;
                        DBAllText.Text += ";\t";
                        i += 4;
                        j++;
                    }
                    if(j == 1) {
                        GF.WriteDBInformation(result, i, DBAllText, variableNameLD, 1);
                        i += 4;
                        j++;
                    }
                    if(j == 2) {
                        GF.WriteDBInformation(result, i, DBAllText, variableNameLD, 2);
                        i += 4;
                        j++;
                    }
                    if(j == 3) {
                        floatV.byte1 = result[i];
                        floatV.byte2 = result[i + 1];
                        floatV.byte3 = result[i + 2];
                        floatV.byte4 = result[i + 3];
                        DBAllText.Text += $"{variableNameLD[3]}: ";
                        DBAllText.Text += floatV.fl;
                        DBAllText.Text += ";\t";
                        i += 4;
                        j++;
                    }
                    if(j == 4) {
                        DBAllText.Text += $"{variableNameLD[4]}: ";
                        DBAllText.Text += intT;
                        DBAllText.Text += ";\n";
                    }
                }
            }
            catch(Exception ex) {
                GF.ClearInvokeTextBox(logBox);
                GF.InvokeTextBox(logBox, ex.Message);
            }
        }
    }
}
