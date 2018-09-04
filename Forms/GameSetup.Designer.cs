namespace CGoban
{
    partial class GameSetup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rad19x19 = new System.Windows.Forms.RadioButton();
            this.rad9x9 = new System.Windows.Forms.RadioButton();
            this.rad13x13 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cboRuleSet = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblHandicaps = new System.Windows.Forms.Label();
            this.numHandicaps = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHandicaps)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rad19x19);
            this.groupBox1.Controls.Add(this.rad9x9);
            this.groupBox1.Controls.Add(this.rad13x13);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(79, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game Size";
            // 
            // rad19x19
            // 
            this.rad19x19.AutoSize = true;
            this.rad19x19.Checked = true;
            this.rad19x19.Location = new System.Drawing.Point(6, 65);
            this.rad19x19.Name = "rad19x19";
            this.rad19x19.Size = new System.Drawing.Size(60, 17);
            this.rad19x19.TabIndex = 3;
            this.rad19x19.TabStop = true;
            this.rad19x19.Text = "19 x 19";
            this.rad19x19.UseVisualStyleBackColor = true;
            this.rad19x19.CheckedChanged += new System.EventHandler(this.rad19x19_CheckedChanged);
            // 
            // rad9x9
            // 
            this.rad9x9.AutoSize = true;
            this.rad9x9.Location = new System.Drawing.Point(6, 19);
            this.rad9x9.Name = "rad9x9";
            this.rad9x9.Size = new System.Drawing.Size(48, 17);
            this.rad9x9.TabIndex = 1;
            this.rad9x9.Text = "9 x 9";
            this.rad9x9.UseVisualStyleBackColor = true;
            this.rad9x9.CheckedChanged += new System.EventHandler(this.rad9x9_CheckedChanged);
            // 
            // rad13x13
            // 
            this.rad13x13.AutoSize = true;
            this.rad13x13.Location = new System.Drawing.Point(6, 42);
            this.rad13x13.Name = "rad13x13";
            this.rad13x13.Size = new System.Drawing.Size(60, 17);
            this.rad13x13.TabIndex = 2;
            this.rad13x13.Text = "13 x 13";
            this.rad13x13.UseVisualStyleBackColor = true;
            this.rad13x13.CheckedChanged += new System.EventHandler(this.rad13x13_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Rule Set:";
            // 
            // cboRuleSet
            // 
            this.cboRuleSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRuleSet.FormattingEnabled = true;
            this.cboRuleSet.Items.AddRange(new object[] {
            "Japanese",
            "Chinese"});
            this.cboRuleSet.Location = new System.Drawing.Point(97, 31);
            this.cboRuleSet.Name = "cboRuleSet";
            this.cboRuleSet.Size = new System.Drawing.Size(78, 21);
            this.cboRuleSet.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(181, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(100, 71);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblHandicaps
            // 
            this.lblHandicaps.AutoSize = true;
            this.lblHandicaps.Location = new System.Drawing.Point(178, 12);
            this.lblHandicaps.Name = "lblHandicaps";
            this.lblHandicaps.Size = new System.Drawing.Size(61, 13);
            this.lblHandicaps.TabIndex = 5;
            this.lblHandicaps.Text = "Handicaps:";
            // 
            // numHandicaps
            // 
            this.numHandicaps.Location = new System.Drawing.Point(181, 32);
            this.numHandicaps.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numHandicaps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHandicaps.Name = "numHandicaps";
            this.numHandicaps.Size = new System.Drawing.Size(38, 20);
            this.numHandicaps.TabIndex = 6;
            this.numHandicaps.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // GameSetup
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(267, 106);
            this.Controls.Add(this.numHandicaps);
            this.Controls.Add(this.lblHandicaps);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cboRuleSet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "GameSetup";
            this.Text = "Game Setup";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHandicaps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rad19x19;
        private System.Windows.Forms.RadioButton rad9x9;
        private System.Windows.Forms.RadioButton rad13x13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboRuleSet;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblHandicaps;
        private System.Windows.Forms.NumericUpDown numHandicaps;
    }
}