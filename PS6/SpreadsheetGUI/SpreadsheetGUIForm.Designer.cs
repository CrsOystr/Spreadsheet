namespace SpreadsheetGUI
{
    partial class SpreadsheetGUIForm
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
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.selectedCellInfoPanel = new System.Windows.Forms.Panel();
            this.valueLabel = new System.Windows.Forms.Label();
            this.labelSeparator2 = new System.Windows.Forms.Label();
            this.contentTextBox = new System.Windows.Forms.TextBox();
            this.labelSeparator1 = new System.Windows.Forms.Label();
            this.CellName = new System.Windows.Forms.Label();
            this.menuStrip.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.selectedCellInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 44);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(629, 260);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.helpToolStripMenuItem1});
            this.menuStrip.Location = new System.Drawing.Point(0, 6);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(107, 28);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip2";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem2});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
            this.saveToolStripMenuItem.Text = "Save            (Ctrl+S)";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem2
            // 
            this.closeToolStripMenuItem2.Name = "closeToolStripMenuItem2";
            this.closeToolStripMenuItem2.Size = new System.Drawing.Size(208, 24);
            this.closeToolStripMenuItem2.Text = "Close          (Alt+F4)";
            this.closeToolStripMenuItem2.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.selectedCellInfoPanel);
            this.controlPanel.Controls.Add(this.menuStrip);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Margin = new System.Windows.Forms.Padding(4);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(629, 44);
            this.controlPanel.TabIndex = 5;
            // 
            // selectedCellInfoPanel
            // 
            this.selectedCellInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.selectedCellInfoPanel.Controls.Add(this.valueLabel);
            this.selectedCellInfoPanel.Controls.Add(this.labelSeparator2);
            this.selectedCellInfoPanel.Controls.Add(this.contentTextBox);
            this.selectedCellInfoPanel.Controls.Add(this.labelSeparator1);
            this.selectedCellInfoPanel.Controls.Add(this.CellName);
            this.selectedCellInfoPanel.Location = new System.Drawing.Point(123, 0);
            this.selectedCellInfoPanel.Margin = new System.Windows.Forms.Padding(4);
            this.selectedCellInfoPanel.Name = "selectedCellInfoPanel";
            this.selectedCellInfoPanel.Size = new System.Drawing.Size(900, 43);
            this.selectedCellInfoPanel.TabIndex = 5;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueLabel.Location = new System.Drawing.Point(237, 9);
            this.valueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(91, 22);
            this.valueLabel.TabIndex = 4;
            this.valueLabel.Text = "                    ";
            // 
            // labelSeparator2
            // 
            this.labelSeparator2.AutoSize = true;
            this.labelSeparator2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSeparator2.Location = new System.Drawing.Point(199, 9);
            this.labelSeparator2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSeparator2.Name = "labelSeparator2";
            this.labelSeparator2.Size = new System.Drawing.Size(29, 20);
            this.labelSeparator2.TabIndex = 3;
            this.labelSeparator2.Text = "==";
            // 
            // contentTextBox
            // 
            this.contentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentTextBox.Location = new System.Drawing.Point(57, 6);
            this.contentTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.contentTextBox.Name = "contentTextBox";
            this.contentTextBox.Size = new System.Drawing.Size(133, 27);
            this.contentTextBox.TabIndex = 2;
            this.contentTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.contentTextBox_KeyDown);
            // 
            // labelSeparator1
            // 
            this.labelSeparator1.AutoSize = true;
            this.labelSeparator1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSeparator1.Location = new System.Drawing.Point(36, 9);
            this.labelSeparator1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSeparator1.Name = "labelSeparator1";
            this.labelSeparator1.Size = new System.Drawing.Size(12, 20);
            this.labelSeparator1.TabIndex = 1;
            this.labelSeparator1.Text = ":";
            // 
            // CellName
            // 
            this.CellName.AutoSize = true;
            this.CellName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CellName.Location = new System.Drawing.Point(7, 10);
            this.CellName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CellName.Name = "CellName";
            this.CellName.Size = new System.Drawing.Size(27, 20);
            this.CellName.TabIndex = 0;
            this.CellName.Text = "A1";
            // 
            // SpreadsheetGUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(629, 304);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.controlPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SpreadsheetGUIForm";
            this.Text = "- Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetGUIForm_FormClosing);
            this.Load += new System.EventHandler(this.SpreadsheetGUIForm_Load);
            this.Shown += new System.EventHandler(this.SpreadsheetGUIForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.selectedCellInfoPanel.ResumeLayout(false);
            this.selectedCellInfoPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Panel selectedCellInfoPanel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label labelSeparator2;
        private System.Windows.Forms.TextBox contentTextBox;
        private System.Windows.Forms.Label labelSeparator1;
        private System.Windows.Forms.Label CellName;
    }
}

