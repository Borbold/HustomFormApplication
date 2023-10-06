namespace HustonRTEMS {
    internal class HandlerLittleD {
        private enum VAR_NAME {
            Time, Plate_id, Sense_id, Value
        }
        private readonly string[] variableNameLD = {
            "Time", "Plate id", "Sense id", "Value", "Second"
        };
        private readonly GeneralFunctional GF = new();
        private string? allText;

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

        public void SelectedIndexChanged(RichTextBox DBAllText, ComboBox FilterComboBox,
                ComboBox HowFilter, TextBox FilterTextBox, TextBox logBox) {
            if(DBAllText.Text.Length > 0 || (allText != null && allText.Length > 0)) {
                Dictionary<int, EnVal> allDB;
                Dictionary<int, EnVal> sortDB;
                int k;

                allText ??= DBAllText.Text;
                string[] splitAllText = allText.Split(new char[] { '\n', '\t' });
                string[] lineBreak = allText.Split('\n');
                switch(FilterComboBox.SelectedIndex) {
                    case 0:
                        k = 0;
                        allDB = new();
                        foreach(string s in splitAllText) {
                            string[] splitTimeText = s.Split(':');
                            if(string.Compare(splitTimeText[0], variableNameLD[(int)VAR_NAME.Time]) == 0) {
                                List<string> time = new();
                                time.AddRange(splitTimeText[1].Split(new char[] { ' ', '.' }));
                                time.Add(splitTimeText[2]);
                                time.AddRange(splitTimeText[3].Split(';'));
                                DateTime dt = new(Convert.ToInt32(time[3]),
                                    Convert.ToInt32(time[2]), Convert.ToInt32(time[1]),
                                    Convert.ToInt32(time[4]), Convert.ToInt32(time[5]),
                                    Convert.ToInt32(time[6]));
                                allDB.Add(k, (EnVal)dt.Ticks);
                                k++;
                            }
                        }

                        sortDB = allDB.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        try {
                            GF.CreateFilterDB(ref sortDB, allDB.Count, HowFilter,
                                (EnVal)Convert.ToDecimal(FilterTextBox.Text));
                        }
                        catch(Exception ex) {
                            logBox.Text = ex.Message;
                        }

                        DBAllText.Text = "";
                        foreach(KeyValuePair<int, EnVal> sort in sortDB) {
                            DBAllText.Text += lineBreak[sort.Key] + '\n';
                        }
                        break;
                    case 1:
                        k = 0;
                        allDB = new();
                        foreach(string s in splitAllText) {
                            string[] splitTimeText = s.Split(':');
                            if(string.Compare(splitTimeText[0], variableNameLD[(int)VAR_NAME.Plate_id]) == 0) {
                                allDB.Add(k, (EnVal)Convert.ToInt32(splitTimeText[1].Split(';')[0]));
                                k++;
                            }
                        }

                        sortDB = allDB.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        try {
                            GF.CreateFilterDB(ref sortDB, allDB.Count, HowFilter,
                                (EnVal)Convert.ToDecimal(FilterTextBox.Text));
                        }
                        catch(Exception ex) {
                            logBox.Text = ex.Message;
                        }

                        DBAllText.Text = "";
                        foreach(KeyValuePair<int, EnVal> sort in sortDB) {
                            DBAllText.Text += lineBreak[sort.Key] + '\n';
                        }
                        break;
                    case 2:
                        k = 0;
                        allDB = new();
                        foreach(string s in splitAllText) {
                            string[] splitTimeText = s.Split(':');
                            if(string.Compare(splitTimeText[0], variableNameLD[(int)VAR_NAME.Sense_id]) == 0) {
                                allDB.Add(k, (EnVal)Convert.ToInt32(splitTimeText[1].Split(';')[0]));
                                k++;
                            }
                        }

                        sortDB = allDB.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        try {
                            GF.CreateFilterDB(ref sortDB, allDB.Count, HowFilter,
                                (EnVal)Convert.ToDecimal(FilterTextBox.Text));
                        }
                        catch(Exception ex) {
                            logBox.Text = ex.Message;
                        }

                        DBAllText.Text = "";
                        foreach(KeyValuePair<int, EnVal> sort in sortDB) {
                            DBAllText.Text += lineBreak[sort.Key] + '\n';
                        }
                        break;
                    case 3:
                        k = 0;
                        allDB = new();
                        foreach(string s in splitAllText) {
                            string[] splitTimeText = s.Split(':');
                            if(string.Compare(splitTimeText[0], variableNameLD[(int)VAR_NAME.Value]) == 0) {
                                allDB.Add(k, (EnVal)Convert.ToInt32(splitTimeText[1].Split(';')[0]));
                                k++;
                            }
                        }

                        sortDB = allDB.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        try {
                            GF.CreateFilterDB(ref sortDB, allDB.Count, HowFilter,
                                (EnVal)Convert.ToDecimal(FilterTextBox.Text));
                        }
                        catch(Exception ex) {
                            logBox.Text = ex.Message;
                        }

                        DBAllText.Text = "";
                        foreach(KeyValuePair<int, EnVal> sort in sortDB) {
                            DBAllText.Text += lineBreak[sort.Key] + '\n';
                        }
                        break;
                }
            } else {
                logBox.Text = "Text box clean. Fill it in with the data.";
            }
        }
    }
}
