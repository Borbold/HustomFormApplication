namespace HustonRTEMS {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            LogBox = new TextBox();
            ListenPort = new Button();
            TemperatureLable = new Label();
            SubTabControl = new TabControl();
            TabTemperature = new TabPage();
            SendTemperature = new Button();
            LabTemp = new Label();
            TrackBarTemp = new TrackBar();
            label3 = new Label();
            TabMagnetometer = new TabPage();
            panel1 = new Panel();
            TrackMagX = new TrackBar();
            label2 = new Label();
            LabMagY_2 = new Label();
            label13 = new Label();
            LabMagZ_2 = new Label();
            label12 = new Label();
            LabMagX_2 = new Label();
            label11 = new Label();
            TrackMagZ_2 = new TrackBar();
            TrackMagY = new TrackBar();
            TrackMagY_2 = new TrackBar();
            TrackMagZ = new TrackBar();
            label17 = new Label();
            LabMagX = new Label();
            label18 = new Label();
            LabMagZ = new Label();
            TrackMagX_2 = new TrackBar();
            LabMagY = new Label();
            label19 = new Label();
            label20 = new Label();
            SendMagnetometer = new Button();
            label1 = new Label();
            TabAcselerometr = new TabPage();
            SendAcselerometer = new Button();
            LabRotY = new Label();
            LabRotZ = new Label();
            LabRotX = new Label();
            TrackBarRotZ = new TrackBar();
            TrackBarRotY = new TrackBar();
            label7 = new Label();
            label6 = new Label();
            TrackBarRotX = new TrackBar();
            label4 = new Label();
            label5 = new Label();
            MainTabControll = new TabControl();
            Settings = new TabPage();
            TabSettings = new TabControl();
            Izernet = new TabPage();
            CheckBox = new CheckBox();
            OpenSocetServer = new Button();
            UseInternet = new RadioButton();
            PortTextBox = new TextBox();
            IPTextBox = new TextBox();
            label9 = new Label();
            label8 = new Label();
            CANPort = new TabPage();
            UseCan = new RadioButton();
            comboBox1 = new ComboBox();
            label14 = new Label();
            SpeedTextBox = new TextBox();
            label10 = new Label();
            SensorReadings = new TabPage();
            SubTabControl.SuspendLayout();
            TabTemperature.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBarTemp).BeginInit();
            TabMagnetometer.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackMagX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagZ_2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagY_2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagX_2).BeginInit();
            TabAcselerometr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotX).BeginInit();
            MainTabControll.SuspendLayout();
            Settings.SuspendLayout();
            TabSettings.SuspendLayout();
            Izernet.SuspendLayout();
            CANPort.SuspendLayout();
            SensorReadings.SuspendLayout();
            SuspendLayout();
            // 
            // LogBox
            // 
            LogBox.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LogBox.Location = new Point(187, 491);
            LogBox.Multiline = true;
            LogBox.Name = "LogBox";
            LogBox.ReadOnly = true;
            LogBox.ScrollBars = ScrollBars.Vertical;
            LogBox.Size = new Size(726, 151);
            LogBox.TabIndex = 1;
            // 
            // ListenPort
            // 
            ListenPort.Font = new Font("Times New Roman", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            ListenPort.Location = new Point(12, 590);
            ListenPort.Name = "ListenPort";
            ListenPort.Size = new Size(169, 52);
            ListenPort.TabIndex = 3;
            ListenPort.Text = "Очистить лог";
            ListenPort.UseVisualStyleBackColor = true;
            ListenPort.Click += ClearLog_Click;
            // 
            // TemperatureLable
            // 
            TemperatureLable.AutoSize = true;
            TemperatureLable.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            TemperatureLable.Location = new Point(102, 33);
            TemperatureLable.Name = "TemperatureLable";
            TemperatureLable.Size = new Size(222, 30);
            TemperatureLable.TabIndex = 4;
            TemperatureLable.Text = "Температура первого";
            // 
            // SubTabControl
            // 
            SubTabControl.Controls.Add(TabTemperature);
            SubTabControl.Controls.Add(TabMagnetometer);
            SubTabControl.Controls.Add(TabAcselerometr);
            SubTabControl.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            SubTabControl.Location = new Point(6, 6);
            SubTabControl.Name = "SubTabControl";
            SubTabControl.SelectedIndex = 0;
            SubTabControl.Size = new Size(884, 436);
            SubTabControl.TabIndex = 6;
            // 
            // TabTemperature
            // 
            TabTemperature.Controls.Add(SendTemperature);
            TabTemperature.Controls.Add(LabTemp);
            TabTemperature.Controls.Add(TrackBarTemp);
            TabTemperature.Controls.Add(label3);
            TabTemperature.Controls.Add(TemperatureLable);
            TabTemperature.Location = new Point(4, 26);
            TabTemperature.Name = "TabTemperature";
            TabTemperature.Padding = new Padding(3);
            TabTemperature.Size = new Size(876, 406);
            TabTemperature.TabIndex = 0;
            TabTemperature.Text = "Temperature";
            TabTemperature.UseVisualStyleBackColor = true;
            // 
            // SendTemperature
            // 
            SendTemperature.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SendTemperature.Location = new Point(6, 343);
            SendTemperature.Name = "SendTemperature";
            SendTemperature.Size = new Size(156, 57);
            SendTemperature.TabIndex = 10;
            SendTemperature.Text = "Отправить данные температуры";
            SendTemperature.UseVisualStyleBackColor = true;
            SendTemperature.Click += SendTemperature_Click;
            // 
            // LabTemp
            // 
            LabTemp.AutoSize = true;
            LabTemp.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabTemp.Location = new Point(636, 33);
            LabTemp.Name = "LabTemp";
            LabTemp.Size = new Size(35, 30);
            LabTemp.TabIndex = 9;
            LabTemp.Text = "20";
            // 
            // TrackBarTemp
            // 
            TrackBarTemp.LargeChange = 1;
            TrackBarTemp.Location = new Point(330, 18);
            TrackBarTemp.Maximum = 500;
            TrackBarTemp.Minimum = 100;
            TrackBarTemp.Name = "TrackBarTemp";
            TrackBarTemp.Size = new Size(300, 45);
            TrackBarTemp.TabIndex = 8;
            TrackBarTemp.Value = 100;
            TrackBarTemp.Scroll += TrackBarTemp_Scroll;
            TrackBarTemp.ValueChanged += TrackBarTemp_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(3, 3);
            label3.Name = "label3";
            label3.Size = new Size(227, 30);
            label3.TabIndex = 6;
            label3.Text = "Датчики температуры";
            // 
            // TabMagnetometer
            // 
            TabMagnetometer.Controls.Add(panel1);
            TabMagnetometer.Controls.Add(SendMagnetometer);
            TabMagnetometer.Controls.Add(label1);
            TabMagnetometer.Location = new Point(4, 26);
            TabMagnetometer.Name = "TabMagnetometer";
            TabMagnetometer.Padding = new Padding(3);
            TabMagnetometer.Size = new Size(876, 406);
            TabMagnetometer.TabIndex = 1;
            TabMagnetometer.Text = "Magnetometer";
            TabMagnetometer.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(TrackMagX);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(LabMagY_2);
            panel1.Controls.Add(label13);
            panel1.Controls.Add(LabMagZ_2);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(LabMagX_2);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(TrackMagZ_2);
            panel1.Controls.Add(TrackMagY);
            panel1.Controls.Add(TrackMagY_2);
            panel1.Controls.Add(TrackMagZ);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(LabMagX);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(LabMagZ);
            panel1.Controls.Add(TrackMagX_2);
            panel1.Controls.Add(LabMagY);
            panel1.Controls.Add(label19);
            panel1.Controls.Add(label20);
            panel1.Location = new Point(275, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(595, 394);
            panel1.TabIndex = 39;
            // 
            // TrackMagX
            // 
            TrackMagX.LargeChange = 1;
            TrackMagX.Location = new Point(245, 36);
            TrackMagX.Maximum = 500;
            TrackMagX.Minimum = 100;
            TrackMagX.Name = "TrackMagX";
            TrackMagX.Size = new Size(300, 45);
            TrackMagX.TabIndex = 20;
            TrackMagX.Value = 100;
            TrackMagX.Scroll += TrackMagX_Scroll;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(20, 51);
            label2.Name = "label2";
            label2.Size = new Size(188, 30);
            label2.TabIndex = 9;
            label2.Text = "Магнитное поле 1";
            // 
            // LabMagY_2
            // 
            LabMagY_2.AutoSize = true;
            LabMagY_2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagY_2.Location = new Point(551, 263);
            LabMagY_2.Name = "LabMagY_2";
            LabMagY_2.Size = new Size(35, 30);
            LabMagY_2.TabIndex = 37;
            LabMagY_2.Text = "20";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label13.Location = new Point(214, 51);
            label13.Name = "label13";
            label13.Size = new Size(25, 30);
            label13.TabIndex = 19;
            label13.Text = "X";
            // 
            // LabMagZ_2
            // 
            LabMagZ_2.AutoSize = true;
            LabMagZ_2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagZ_2.Location = new Point(551, 314);
            LabMagZ_2.Name = "LabMagZ_2";
            LabMagZ_2.Size = new Size(35, 30);
            LabMagZ_2.TabIndex = 36;
            LabMagZ_2.Text = "20";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label12.Location = new Point(214, 102);
            label12.Name = "label12";
            label12.Size = new Size(25, 30);
            label12.TabIndex = 21;
            label12.Text = "Y";
            // 
            // LabMagX_2
            // 
            LabMagX_2.AutoSize = true;
            LabMagX_2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagX_2.Location = new Point(551, 212);
            LabMagX_2.Name = "LabMagX_2";
            LabMagX_2.Size = new Size(35, 30);
            LabMagX_2.TabIndex = 35;
            LabMagX_2.Text = "20";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(214, 153);
            label11.Name = "label11";
            label11.Size = new Size(25, 30);
            label11.TabIndex = 22;
            label11.Text = "Z";
            // 
            // TrackMagZ_2
            // 
            TrackMagZ_2.LargeChange = 1;
            TrackMagZ_2.Location = new Point(245, 299);
            TrackMagZ_2.Maximum = 500;
            TrackMagZ_2.Minimum = 100;
            TrackMagZ_2.Name = "TrackMagZ_2";
            TrackMagZ_2.Size = new Size(300, 45);
            TrackMagZ_2.TabIndex = 34;
            TrackMagZ_2.Value = 100;
            TrackMagZ_2.Scroll += TrackMagZ_2_Scroll;
            // 
            // TrackMagY
            // 
            TrackMagY.LargeChange = 1;
            TrackMagY.Location = new Point(245, 87);
            TrackMagY.Maximum = 500;
            TrackMagY.Minimum = 100;
            TrackMagY.Name = "TrackMagY";
            TrackMagY.Size = new Size(300, 45);
            TrackMagY.TabIndex = 23;
            TrackMagY.Value = 100;
            TrackMagY.Scroll += TrackMagY_Scroll;
            // 
            // TrackMagY_2
            // 
            TrackMagY_2.LargeChange = 1;
            TrackMagY_2.Location = new Point(245, 248);
            TrackMagY_2.Maximum = 500;
            TrackMagY_2.Minimum = 100;
            TrackMagY_2.Name = "TrackMagY_2";
            TrackMagY_2.Size = new Size(300, 45);
            TrackMagY_2.TabIndex = 33;
            TrackMagY_2.Value = 100;
            TrackMagY_2.Scroll += TrackMagY_2_Scroll;
            // 
            // TrackMagZ
            // 
            TrackMagZ.LargeChange = 1;
            TrackMagZ.Location = new Point(245, 138);
            TrackMagZ.Maximum = 500;
            TrackMagZ.Minimum = 100;
            TrackMagZ.Name = "TrackMagZ";
            TrackMagZ.Size = new Size(300, 45);
            TrackMagZ.TabIndex = 24;
            TrackMagZ.Value = 100;
            TrackMagZ.Scroll += TrackMagZ_Scroll;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label17.Location = new Point(214, 314);
            label17.Name = "label17";
            label17.Size = new Size(25, 30);
            label17.TabIndex = 32;
            label17.Text = "Z";
            // 
            // LabMagX
            // 
            LabMagX.AutoSize = true;
            LabMagX.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagX.Location = new Point(551, 51);
            LabMagX.Name = "LabMagX";
            LabMagX.Size = new Size(35, 30);
            LabMagX.TabIndex = 25;
            LabMagX.Text = "20";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label18.Location = new Point(214, 263);
            label18.Name = "label18";
            label18.Size = new Size(25, 30);
            label18.TabIndex = 31;
            label18.Text = "Y";
            // 
            // LabMagZ
            // 
            LabMagZ.AutoSize = true;
            LabMagZ.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagZ.Location = new Point(551, 153);
            LabMagZ.Name = "LabMagZ";
            LabMagZ.Size = new Size(35, 30);
            LabMagZ.TabIndex = 26;
            LabMagZ.Text = "20";
            // 
            // TrackMagX_2
            // 
            TrackMagX_2.LargeChange = 1;
            TrackMagX_2.Location = new Point(245, 197);
            TrackMagX_2.Maximum = 500;
            TrackMagX_2.Minimum = 100;
            TrackMagX_2.Name = "TrackMagX_2";
            TrackMagX_2.Size = new Size(300, 45);
            TrackMagX_2.TabIndex = 30;
            TrackMagX_2.Value = 100;
            TrackMagX_2.Scroll += TrackMagX_2_Scroll;
            // 
            // LabMagY
            // 
            LabMagY.AutoSize = true;
            LabMagY.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabMagY.Location = new Point(551, 102);
            LabMagY.Name = "LabMagY";
            LabMagY.Size = new Size(35, 30);
            LabMagY.TabIndex = 27;
            LabMagY.Text = "20";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label19.Location = new Point(214, 212);
            label19.Name = "label19";
            label19.Size = new Size(25, 30);
            label19.TabIndex = 29;
            label19.Text = "X";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label20.Location = new Point(23, 212);
            label20.Name = "label20";
            label20.Size = new Size(188, 30);
            label20.TabIndex = 28;
            label20.Text = "Магнитное поле 2";
            // 
            // SendMagnetometer
            // 
            SendMagnetometer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SendMagnetometer.Location = new Point(6, 343);
            SendMagnetometer.Name = "SendMagnetometer";
            SendMagnetometer.Size = new Size(156, 57);
            SendMagnetometer.TabIndex = 38;
            SendMagnetometer.Text = "Отправить данные магнитометра";
            SendMagnetometer.UseVisualStyleBackColor = true;
            SendMagnetometer.Click += SendMagnetometer_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(6, 3);
            label1.Name = "label1";
            label1.Size = new Size(263, 30);
            label1.TabIndex = 10;
            label1.Text = "Датчики магнитного поля";
            // 
            // TabAcselerometr
            // 
            TabAcselerometr.Controls.Add(SendAcselerometer);
            TabAcselerometr.Controls.Add(LabRotY);
            TabAcselerometr.Controls.Add(LabRotZ);
            TabAcselerometr.Controls.Add(LabRotX);
            TabAcselerometr.Controls.Add(TrackBarRotZ);
            TabAcselerometr.Controls.Add(TrackBarRotY);
            TabAcselerometr.Controls.Add(label7);
            TabAcselerometr.Controls.Add(label6);
            TabAcselerometr.Controls.Add(TrackBarRotX);
            TabAcselerometr.Controls.Add(label4);
            TabAcselerometr.Controls.Add(label5);
            TabAcselerometr.Location = new Point(4, 26);
            TabAcselerometr.Name = "TabAcselerometr";
            TabAcselerometr.Padding = new Padding(3);
            TabAcselerometr.Size = new Size(876, 406);
            TabAcselerometr.TabIndex = 2;
            TabAcselerometr.Text = "Acselerometer";
            TabAcselerometr.UseVisualStyleBackColor = true;
            // 
            // SendAcselerometer
            // 
            SendAcselerometer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SendAcselerometer.Location = new Point(6, 343);
            SendAcselerometer.Name = "SendAcselerometer";
            SendAcselerometer.Size = new Size(156, 57);
            SendAcselerometer.TabIndex = 19;
            SendAcselerometer.Text = "Отправить данные акселерометра";
            SendAcselerometer.UseVisualStyleBackColor = true;
            SendAcselerometer.Click += SendAcselerometer_Click;
            // 
            // LabRotY
            // 
            LabRotY.AutoSize = true;
            LabRotY.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabRotY.Location = new Point(506, 84);
            LabRotY.Name = "LabRotY";
            LabRotY.Size = new Size(35, 30);
            LabRotY.TabIndex = 18;
            LabRotY.Text = "20";
            // 
            // LabRotZ
            // 
            LabRotZ.AutoSize = true;
            LabRotZ.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabRotZ.Location = new Point(506, 135);
            LabRotZ.Name = "LabRotZ";
            LabRotZ.Size = new Size(35, 30);
            LabRotZ.TabIndex = 17;
            LabRotZ.Text = "20";
            // 
            // LabRotX
            // 
            LabRotX.AutoSize = true;
            LabRotX.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            LabRotX.Location = new Point(506, 33);
            LabRotX.Name = "LabRotX";
            LabRotX.Size = new Size(35, 30);
            LabRotX.TabIndex = 16;
            LabRotX.Text = "20";
            // 
            // TrackBarRotZ
            // 
            TrackBarRotZ.LargeChange = 1;
            TrackBarRotZ.Location = new Point(200, 120);
            TrackBarRotZ.Maximum = 500;
            TrackBarRotZ.Minimum = 100;
            TrackBarRotZ.Name = "TrackBarRotZ";
            TrackBarRotZ.Size = new Size(300, 45);
            TrackBarRotZ.TabIndex = 15;
            TrackBarRotZ.Value = 100;
            TrackBarRotZ.Scroll += TrackBarRotZ_Scroll;
            // 
            // TrackBarRotY
            // 
            TrackBarRotY.LargeChange = 1;
            TrackBarRotY.Location = new Point(200, 69);
            TrackBarRotY.Maximum = 500;
            TrackBarRotY.Minimum = 100;
            TrackBarRotY.Name = "TrackBarRotY";
            TrackBarRotY.Size = new Size(300, 45);
            TrackBarRotY.TabIndex = 14;
            TrackBarRotY.Value = 100;
            TrackBarRotY.Scroll += TrackBarRotY_Scroll;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(169, 135);
            label7.Name = "label7";
            label7.Size = new Size(25, 30);
            label7.TabIndex = 13;
            label7.Text = "Z";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(169, 84);
            label6.Name = "label6";
            label6.Size = new Size(25, 30);
            label6.TabIndex = 12;
            label6.Text = "Y";
            // 
            // TrackBarRotX
            // 
            TrackBarRotX.LargeChange = 1;
            TrackBarRotX.Location = new Point(200, 18);
            TrackBarRotX.Maximum = 500;
            TrackBarRotX.Minimum = 100;
            TrackBarRotX.Name = "TrackBarRotX";
            TrackBarRotX.Size = new Size(300, 45);
            TrackBarRotX.TabIndex = 11;
            TrackBarRotX.Value = 100;
            TrackBarRotX.Scroll += TrackBarRotX_Scroll;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(3, 3);
            label4.Name = "label4";
            label4.Size = new Size(191, 30);
            label4.TabIndex = 10;
            label4.Text = "Датчики поворота";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(169, 33);
            label5.Name = "label5";
            label5.Size = new Size(25, 30);
            label5.TabIndex = 9;
            label5.Text = "X";
            // 
            // MainTabControll
            // 
            MainTabControll.Controls.Add(Settings);
            MainTabControll.Controls.Add(SensorReadings);
            MainTabControll.Location = new Point(12, 12);
            MainTabControll.Name = "MainTabControll";
            MainTabControll.SelectedIndex = 0;
            MainTabControll.Size = new Size(901, 473);
            MainTabControll.TabIndex = 13;
            // 
            // Settings
            // 
            Settings.Controls.Add(TabSettings);
            Settings.Location = new Point(4, 24);
            Settings.Name = "Settings";
            Settings.Padding = new Padding(3);
            Settings.Size = new Size(893, 445);
            Settings.TabIndex = 1;
            Settings.Text = "Настройки";
            Settings.UseVisualStyleBackColor = true;
            // 
            // TabSettings
            // 
            TabSettings.Controls.Add(Izernet);
            TabSettings.Controls.Add(CANPort);
            TabSettings.Location = new Point(6, 6);
            TabSettings.Name = "TabSettings";
            TabSettings.SelectedIndex = 0;
            TabSettings.Size = new Size(881, 433);
            TabSettings.TabIndex = 1;
            // 
            // Izernet
            // 
            Izernet.Controls.Add(CheckBox);
            Izernet.Controls.Add(OpenSocetServer);
            Izernet.Controls.Add(UseInternet);
            Izernet.Controls.Add(PortTextBox);
            Izernet.Controls.Add(IPTextBox);
            Izernet.Controls.Add(label9);
            Izernet.Controls.Add(label8);
            Izernet.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Izernet.Location = new Point(4, 24);
            Izernet.Name = "Izernet";
            Izernet.Padding = new Padding(3);
            Izernet.Size = new Size(873, 405);
            Izernet.TabIndex = 0;
            Izernet.Text = "Интернет";
            Izernet.UseVisualStyleBackColor = true;
            // 
            // CheckBox
            // 
            CheckBox.AutoSize = true;
            CheckBox.Checked = true;
            CheckBox.CheckState = CheckState.Checked;
            CheckBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            CheckBox.Location = new Point(3, 323);
            CheckBox.Name = "CheckBox";
            CheckBox.Size = new Size(77, 25);
            CheckBox.TabIndex = 14;
            CheckBox.Text = "RTEMS";
            CheckBox.UseVisualStyleBackColor = true;
            // 
            // OpenSocetServer
            // 
            OpenSocetServer.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            OpenSocetServer.Location = new Point(3, 354);
            OpenSocetServer.Name = "OpenSocetServer";
            OpenSocetServer.Size = new Size(346, 45);
            OpenSocetServer.TabIndex = 9;
            OpenSocetServer.Text = "Открыть сокет на прослушку";
            OpenSocetServer.UseVisualStyleBackColor = true;
            OpenSocetServer.Click += OpenSocetServer_Click;
            // 
            // UseInternet
            // 
            UseInternet.AutoSize = true;
            UseInternet.CheckAlign = ContentAlignment.MiddleRight;
            UseInternet.Checked = true;
            UseInternet.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            UseInternet.Location = new Point(750, 3);
            UseInternet.Name = "UseInternet";
            UseInternet.Size = new Size(120, 36);
            UseInternet.TabIndex = 8;
            UseInternet.TabStop = true;
            UseInternet.Text = "Use that";
            UseInternet.UseVisualStyleBackColor = true;
            UseInternet.CheckedChanged += UseInternet_CheckedChanged;
            // 
            // PortTextBox
            // 
            PortTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            PortTextBox.Location = new Point(53, 40);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.Size = new Size(100, 29);
            PortTextBox.TabIndex = 3;
            PortTextBox.Text = "5555";
            // 
            // IPTextBox
            // 
            IPTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            IPTextBox.Location = new Point(53, 5);
            IPTextBox.Name = "IPTextBox";
            IPTextBox.Size = new Size(100, 29);
            IPTextBox.TabIndex = 2;
            IPTextBox.Text = "127.0.0.1";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(3, 43);
            label9.Name = "label9";
            label9.Size = new Size(38, 21);
            label9.TabIndex = 1;
            label9.Text = "Port";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label8.Location = new Point(6, 8);
            label8.Name = "label8";
            label8.Size = new Size(23, 21);
            label8.TabIndex = 0;
            label8.Text = "IP";
            // 
            // CANPort
            // 
            CANPort.Controls.Add(UseCan);
            CANPort.Controls.Add(comboBox1);
            CANPort.Controls.Add(label14);
            CANPort.Controls.Add(SpeedTextBox);
            CANPort.Controls.Add(label10);
            CANPort.Location = new Point(4, 24);
            CANPort.Name = "CANPort";
            CANPort.Padding = new Padding(3);
            CANPort.Size = new Size(873, 405);
            CANPort.TabIndex = 1;
            CANPort.Text = "CAN";
            CANPort.UseVisualStyleBackColor = true;
            // 
            // UseCan
            // 
            UseCan.AutoSize = true;
            UseCan.CheckAlign = ContentAlignment.MiddleRight;
            UseCan.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            UseCan.Location = new Point(750, 3);
            UseCan.Name = "UseCan";
            UseCan.Size = new Size(120, 36);
            UseCan.TabIndex = 7;
            UseCan.TabStop = true;
            UseCan.Text = "Use that";
            UseCan.UseVisualStyleBackColor = true;
            UseCan.CheckedChanged += UseCan_CheckedChanged;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Com1", "Com2", "Com3" });
            comboBox1.Location = new Point(65, 34);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 5;
            comboBox1.Text = "Com1";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label14.Location = new Point(18, 36);
            label14.Name = "label14";
            label14.Size = new Size(38, 21);
            label14.TabIndex = 4;
            label14.Text = "Port";
            // 
            // SpeedTextBox
            // 
            SpeedTextBox.Location = new Point(65, 5);
            SpeedTextBox.Name = "SpeedTextBox";
            SpeedTextBox.Size = new Size(100, 23);
            SpeedTextBox.TabIndex = 1;
            SpeedTextBox.Text = "100";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new Point(6, 3);
            label10.Name = "label10";
            label10.Size = new Size(53, 21);
            label10.TabIndex = 0;
            label10.Text = "Speed";
            // 
            // SensorReadings
            // 
            SensorReadings.Controls.Add(SubTabControl);
            SensorReadings.Location = new Point(4, 24);
            SensorReadings.Name = "SensorReadings";
            SensorReadings.Padding = new Padding(3);
            SensorReadings.Size = new Size(893, 445);
            SensorReadings.TabIndex = 0;
            SensorReadings.Text = "Показания датчиков";
            SensorReadings.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(925, 654);
            Controls.Add(MainTabControll);
            Controls.Add(ListenPort);
            Controls.Add(LogBox);
            Name = "Form1";
            Text = "TCP API";
            SubTabControl.ResumeLayout(false);
            TabTemperature.ResumeLayout(false);
            TabTemperature.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBarTemp).EndInit();
            TabMagnetometer.ResumeLayout(false);
            TabMagnetometer.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackMagX).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagZ_2).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagY).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagY_2).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackMagX_2).EndInit();
            TabAcselerometr.ResumeLayout(false);
            TabAcselerometr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotY).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarRotX).EndInit();
            MainTabControll.ResumeLayout(false);
            Settings.ResumeLayout(false);
            TabSettings.ResumeLayout(false);
            Izernet.ResumeLayout(false);
            Izernet.PerformLayout();
            CANPort.ResumeLayout(false);
            CANPort.PerformLayout();
            SensorReadings.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox LogBox;
        private Button ListenPort;
        private Label TemperatureLable;
        private TabControl SubTabControl;
        private TabPage TabTemperature;
        private TabPage TabMagnetometer;
        private TabPage TabAcselerometr;
        private Label label3;
        private TrackBar TrackBarTemp;
        private Label label1;
        private Label label2;
        private TrackBar TrackBarRotZ;
        private TrackBar TrackBarRotY;
        private Label label7;
        private Label label6;
        private TrackBar TrackBarRotX;
        private Label label4;
        private Label label5;
        private Label LabTemp;
        private Label LabRotY;
        private Label LabRotZ;
        private Label LabRotX;
        private TabControl MainTabControll;
        private TabPage SensorReadings;
        private TabPage Settings;
        private Label LabMagY;
        private Label LabMagZ;
        private Label LabMagX;
        private TrackBar TrackMagZ;
        private TrackBar TrackMagY;
        private Label label11;
        private Label label12;
        private TrackBar TrackMagX;
        private Label label13;
        private TabControl TabSettings;
        private TabPage Izernet;
        private TabPage CANPort;
        private TextBox PortTextBox;
        private TextBox IPTextBox;
        private Label label9;
        private Label label8;
        private TextBox SpeedTextBox;
        private Label label10;
        private Label LabMagY_2;
        private Label LabMagZ_2;
        private Label LabMagX_2;
        private TrackBar TrackMagZ_2;
        private TrackBar TrackMagY_2;
        private Label label17;
        private Label label18;
        private TrackBar TrackMagX_2;
        private Label label19;
        private Label label20;
        private CheckBox CheckBox;
        private ComboBox comboBox1;
        private Label label14;
        private RadioButton UseCan;
        private RadioButton UseInternet;
        private Button OpenSocetServer;
        private Button SendTemperature;
        private Button SendMagnetometer;
        private Button SendAcselerometer;
        private Panel panel1;
    }
}