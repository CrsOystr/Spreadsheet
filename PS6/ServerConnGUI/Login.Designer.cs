﻿namespace ServerConnGUI
{
    partial class Login
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
            this.IP_textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PW_textbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ServerButton = new System.Windows.Forms.Button();
            this.ssListBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.newSpreadsheet_textBox = new System.Windows.Forms.TextBox();
            this.groupBox_connected = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox_connected.SuspendLayout();
            this.SuspendLayout();
            // 
            // IP_textbox
            // 
            this.IP_textbox.Location = new System.Drawing.Point(105, 32);
            this.IP_textbox.Name = "IP_textbox";
            this.IP_textbox.Size = new System.Drawing.Size(135, 20);
            this.IP_textbox.TabIndex = 0;
            this.IP_textbox.Text = "127.0.0.1";
            this.IP_textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_keyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please enter in a server IP and password to connect";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP Address";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Server Password";
            // 
            // PW_textbox
            // 
            this.PW_textbox.Location = new System.Drawing.Point(105, 59);
            this.PW_textbox.Name = "PW_textbox";
            this.PW_textbox.Size = new System.Drawing.Size(135, 20);
            this.PW_textbox.TabIndex = 4;
            this.PW_textbox.Text = "james";
            this.PW_textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_keyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Status:";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(105, 115);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(79, 13);
            this.StatusLabel.TabIndex = 6;
            this.StatusLabel.Text = "Not Connected";
            // 
            // ServerButton
            // 
            this.ServerButton.Location = new System.Drawing.Point(105, 86);
            this.ServerButton.Name = "ServerButton";
            this.ServerButton.Size = new System.Drawing.Size(135, 23);
            this.ServerButton.TabIndex = 7;
            this.ServerButton.Text = "Connect";
            this.ServerButton.UseVisualStyleBackColor = true;
            this.ServerButton.Click += new System.EventHandler(this.ServerButton_Click);
            // 
            // ssListBox
            // 
            this.ssListBox.FormattingEnabled = true;
            this.ssListBox.Items.AddRange(new object[] {
            " "});
            this.ssListBox.Location = new System.Drawing.Point(9, 18);
            this.ssListBox.Name = "ssListBox";
            this.ssListBox.Size = new System.Drawing.Size(227, 212);
            this.ssListBox.TabIndex = 8;
            this.ssListBox.DoubleClick += new System.EventHandler(this.ssListBox_DoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(178, 235);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 19);
            this.button1.TabIndex = 9;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // newSpreadsheet_textBox
            // 
            this.newSpreadsheet_textBox.Location = new System.Drawing.Point(9, 235);
            this.newSpreadsheet_textBox.Margin = new System.Windows.Forms.Padding(2);
            this.newSpreadsheet_textBox.Name = "newSpreadsheet_textBox";
            this.newSpreadsheet_textBox.Size = new System.Drawing.Size(166, 20);
            this.newSpreadsheet_textBox.TabIndex = 10;
            this.newSpreadsheet_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newSpreadsheet_textBox_KeyDown);
            // 
            // groupBox_connected
            // 
            this.groupBox_connected.Controls.Add(this.ssListBox);
            this.groupBox_connected.Controls.Add(this.newSpreadsheet_textBox);
            this.groupBox_connected.Controls.Add(this.button1);
            this.groupBox_connected.Enabled = false;
            this.groupBox_connected.Location = new System.Drawing.Point(5, 140);
            this.groupBox_connected.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_connected.Name = "groupBox_connected";
            this.groupBox_connected.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_connected.Size = new System.Drawing.Size(245, 264);
            this.groupBox_connected.TabIndex = 11;
            this.groupBox_connected.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Debug";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 406);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox_connected);
            this.Controls.Add(this.ServerButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PW_textbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.IP_textbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Login";
            this.Text = "Login to server";
            this.Load += new System.EventHandler(this.Login_Load);
            this.groupBox_connected.ResumeLayout(false);
            this.groupBox_connected.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IP_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PW_textbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button ServerButton;
        private System.Windows.Forms.ListBox ssListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox newSpreadsheet_textBox;
        private System.Windows.Forms.GroupBox groupBox_connected;
        private System.Windows.Forms.Button button2;
    }
}

