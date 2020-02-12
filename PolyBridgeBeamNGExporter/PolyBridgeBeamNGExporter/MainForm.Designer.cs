namespace PolyBridgeBeamNGExporter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.browseDialog = new System.Windows.Forms.OpenFileDialog();
            this.bridgePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.convertButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.bridgeNameTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bridgeWidthTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.unpackedRadioButton = new System.Windows.Forms.RadioButton();
            this.packedRadioButton = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.exportSaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.browseDialog.Filter = "Poly Bridge Campaign Save|*.*|Poly Bridge Sandbox Save|*.pbl";
            this.browseDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // bridgePathTextBox
            // 
            this.bridgePathTextBox.Location = new System.Drawing.Point(128, 12);
            this.bridgePathTextBox.Name = "bridgePathTextBox";
            this.bridgePathTextBox.ReadOnly = true;
            this.bridgePathTextBox.Size = new System.Drawing.Size(301, 20);
            this.bridgePathTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Poly Bridge Save File:";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(435, 10);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // convertButton
            // 
            this.convertButton.Enabled = false;
            this.convertButton.Location = new System.Drawing.Point(365, 142);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(145, 28);
            this.convertButton.TabIndex = 3;
            this.convertButton.Text = "Convert to BeamNG";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.ConvertButton_Click);
            this.convertButton.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConvertButton_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Bridge Name";
            // 
            // bridgeNameTextbox
            // 
            this.bridgeNameTextbox.Location = new System.Drawing.Point(128, 38);
            this.bridgeNameTextbox.Name = "bridgeNameTextbox";
            this.bridgeNameTextbox.Size = new System.Drawing.Size(301, 20);
            this.bridgeNameTextbox.TabIndex = 5;
            this.bridgeNameTextbox.Text = "My Poly Bridge Bridge";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bridge Width (m)";
            // 
            // bridgeWidthTextbox
            // 
            this.bridgeWidthTextbox.Location = new System.Drawing.Point(128, 64);
            this.bridgeWidthTextbox.Name = "bridgeWidthTextbox";
            this.bridgeWidthTextbox.Size = new System.Drawing.Size(301, 20);
            this.bridgeWidthTextbox.TabIndex = 7;
            this.bridgeWidthTextbox.Text = "5.0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Export type";
            // 
            // unpackedRadioButton
            // 
            this.unpackedRadioButton.AutoSize = true;
            this.unpackedRadioButton.Location = new System.Drawing.Point(128, 97);
            this.unpackedRadioButton.Name = "unpackedRadioButton";
            this.unpackedRadioButton.Size = new System.Drawing.Size(75, 17);
            this.unpackedRadioButton.TabIndex = 9;
            this.unpackedRadioButton.Text = "Unpacked";
            this.unpackedRadioButton.UseVisualStyleBackColor = true;
            // 
            // packedRadioButton
            // 
            this.packedRadioButton.AutoSize = true;
            this.packedRadioButton.Checked = true;
            this.packedRadioButton.Location = new System.Drawing.Point(209, 99);
            this.packedRadioButton.Name = "packedRadioButton";
            this.packedRadioButton.Size = new System.Drawing.Size(62, 17);
            this.packedRadioButton.TabIndex = 10;
            this.packedRadioButton.TabStop = true;
            this.packedRadioButton.Text = "Packed";
            this.packedRadioButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(12, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(498, 1);
            this.label5.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Created by Dummiesman";
            // 
            // exportSaveButton
            // 
            this.exportSaveButton.Location = new System.Drawing.Point(228, 142);
            this.exportSaveButton.Name = "exportSaveButton";
            this.exportSaveButton.Size = new System.Drawing.Size(131, 28);
            this.exportSaveButton.TabIndex = 13;
            this.exportSaveButton.Text = "Export Save";
            this.exportSaveButton.UseVisualStyleBackColor = true;
            this.exportSaveButton.Visible = false;
            this.exportSaveButton.Click += new System.EventHandler(this.ExportSaveButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 182);
            this.Controls.Add(this.exportSaveButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.packedRadioButton);
            this.Controls.Add(this.unpackedRadioButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bridgeWidthTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bridgeNameTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bridgePathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Poly Bridge -> BeamNG";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog browseDialog;
        private System.Windows.Forms.TextBox bridgePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox bridgeNameTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox bridgeWidthTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton unpackedRadioButton;
        private System.Windows.Forms.RadioButton packedRadioButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button exportSaveButton;
    }
}

