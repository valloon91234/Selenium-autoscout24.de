namespace AutoScout24
{
    partial class Form_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_StartPublishing = new System.Windows.Forms.Button();
            this.textBox_RootDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_DataFilename = new System.Windows.Forms.TextBox();
            this.groupBox_Proxy = new System.Windows.Forms.GroupBox();
            this.comboBox_ProxyList = new System.Windows.Forms.ComboBox();
            this.textBox_ProxyPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_ProxyUsername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_ProxyAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_StartChecking = new System.Windows.Forms.Button();
            this.groupBox_Proxy.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_StartPublishing
            // 
            this.button_StartPublishing.Location = new System.Drawing.Point(320, 133);
            this.button_StartPublishing.Margin = new System.Windows.Forms.Padding(4);
            this.button_StartPublishing.Name = "button_StartPublishing";
            this.button_StartPublishing.Size = new System.Drawing.Size(189, 39);
            this.button_StartPublishing.TabIndex = 0;
            this.button_StartPublishing.Text = "Start Publishing";
            this.button_StartPublishing.UseVisualStyleBackColor = true;
            this.button_StartPublishing.Click += new System.EventHandler(this.button_StartPublshing_Click);
            // 
            // textBox_RootDirectory
            // 
            this.textBox_RootDirectory.Location = new System.Drawing.Point(109, 12);
            this.textBox_RootDirectory.Name = "textBox_RootDirectory";
            this.textBox_RootDirectory.Size = new System.Drawing.Size(400, 27);
            this.textBox_RootDirectory.TabIndex = 1;
            this.textBox_RootDirectory.Text = "D:\\_temp\\Cars";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(10, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(109, 90);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(200, 27);
            this.textBox_Password.TabIndex = 5;
            this.textBox_Password.Text = "qweQWE123!@#";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(10, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "Root Folder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(10, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "Data File";
            // 
            // textBox_DataFilename
            // 
            this.textBox_DataFilename.Location = new System.Drawing.Point(109, 51);
            this.textBox_DataFilename.Name = "textBox_DataFilename";
            this.textBox_DataFilename.Size = new System.Drawing.Size(400, 27);
            this.textBox_DataFilename.TabIndex = 7;
            this.textBox_DataFilename.Text = "D:\\_temp\\Cars\\data.txt";
            // 
            // groupBox1
            // 
            this.groupBox_Proxy.Controls.Add(this.comboBox_ProxyList);
            this.groupBox_Proxy.Controls.Add(this.textBox_ProxyPassword);
            this.groupBox_Proxy.Controls.Add(this.label7);
            this.groupBox_Proxy.Controls.Add(this.textBox_ProxyUsername);
            this.groupBox_Proxy.Controls.Add(this.label6);
            this.groupBox_Proxy.Controls.Add(this.textBox_ProxyAddress);
            this.groupBox_Proxy.Controls.Add(this.label5);
            this.groupBox_Proxy.ForeColor = System.Drawing.Color.White;
            this.groupBox_Proxy.Location = new System.Drawing.Point(12, 123);
            this.groupBox_Proxy.Name = "groupBox1";
            this.groupBox_Proxy.Size = new System.Drawing.Size(297, 187);
            this.groupBox_Proxy.TabIndex = 9;
            this.groupBox_Proxy.TabStop = false;
            this.groupBox_Proxy.Text = "Proxy";
            // 
            // comboBox_ProxyList
            // 
            this.comboBox_ProxyList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ProxyList.FormattingEnabled = true;
            this.comboBox_ProxyList.Location = new System.Drawing.Point(100, 26);
            this.comboBox_ProxyList.Name = "comboBox_ProxyList";
            this.comboBox_ProxyList.Size = new System.Drawing.Size(111, 27);
            this.comboBox_ProxyList.TabIndex = 14;
            this.comboBox_ProxyList.SelectedIndexChanged += new System.EventHandler(this.comboBox_ProxyList_SelectedIndexChanged);
            // 
            // textBox_ProxyPassword
            // 
            this.textBox_ProxyPassword.Enabled = false;
            this.textBox_ProxyPassword.Location = new System.Drawing.Point(100, 145);
            this.textBox_ProxyPassword.Name = "textBox_ProxyPassword";
            this.textBox_ProxyPassword.Size = new System.Drawing.Size(183, 27);
            this.textBox_ProxyPassword.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label7.Location = new System.Drawing.Point(13, 149);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 19);
            this.label7.TabIndex = 12;
            this.label7.Text = "Password";
            // 
            // textBox_ProxyUsername
            // 
            this.textBox_ProxyUsername.Enabled = false;
            this.textBox_ProxyUsername.Location = new System.Drawing.Point(100, 106);
            this.textBox_ProxyUsername.Name = "textBox_ProxyUsername";
            this.textBox_ProxyUsername.Size = new System.Drawing.Size(183, 27);
            this.textBox_ProxyUsername.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(13, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 19);
            this.label6.TabIndex = 10;
            this.label6.Text = "Username";
            // 
            // textBox_ProxyAddress
            // 
            this.textBox_ProxyAddress.Enabled = false;
            this.textBox_ProxyAddress.Location = new System.Drawing.Point(100, 67);
            this.textBox_ProxyAddress.Name = "textBox_ProxyAddress";
            this.textBox_ProxyAddress.Size = new System.Drawing.Size(183, 27);
            this.textBox_ProxyAddress.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(13, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 19);
            this.label5.TabIndex = 8;
            this.label5.Text = "Address";
            // 
            // button_StartChecking
            // 
            this.button_StartChecking.Location = new System.Drawing.Point(320, 183);
            this.button_StartChecking.Margin = new System.Windows.Forms.Padding(4);
            this.button_StartChecking.Name = "button_StartChecking";
            this.button_StartChecking.Size = new System.Drawing.Size(189, 39);
            this.button_StartChecking.TabIndex = 10;
            this.button_StartChecking.Text = "Start Checking";
            this.button_StartChecking.UseVisualStyleBackColor = true;
            this.button_StartChecking.Click += new System.EventHandler(this.button_StartChecking_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(521, 323);
            this.Controls.Add(this.button_StartChecking);
            this.Controls.Add(this.groupBox_Proxy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_DataFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_RootDirectory);
            this.Controls.Add(this.button_StartPublishing);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoScout24";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Main_FormClosed);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.groupBox_Proxy.ResumeLayout(false);
            this.groupBox_Proxy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_StartPublishing;
        private System.Windows.Forms.TextBox textBox_RootDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_DataFilename;
        private System.Windows.Forms.GroupBox groupBox_Proxy;
        private System.Windows.Forms.TextBox textBox_ProxyPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_ProxyUsername;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ProxyAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_ProxyList;
        private System.Windows.Forms.Button button_StartChecking;
    }
}

