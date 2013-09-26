namespace RealAnggaran
{
    partial class fMRek
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
            this.lvTampil = new System.Windows.Forms.ListView();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtKd = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ckdKPA = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUraian = new System.Windows.Forms.TextBox();
            this.lNama = new System.Windows.Forms.Label();
            this.bKeluar = new System.Windows.Forms.Button();
            this.bSimpan = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lvTampil);
            this.groupBox1.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.groupBox1.Location = new System.Drawing.Point(12, 204);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1012, 380);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DAFTAR REKENING";
            // 
            // lvTampil
            // 
            this.lvTampil.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTampil.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.lvTampil.Location = new System.Drawing.Point(9, 21);
            this.lvTampil.Name = "lvTampil";
            this.lvTampil.Size = new System.Drawing.Size(991, 348);
            this.lvTampil.TabIndex = 0;
            this.lvTampil.UseCompatibleStateImageBehavior = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(140, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 30);
            this.label6.TabIndex = 9;
            this.label6.Text = ":";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(140, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 30);
            this.label5.TabIndex = 8;
            this.label5.Text = ":";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtKd);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.ckdKPA);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtUraian);
            this.groupBox2.Controls.Add(this.lNama);
            this.groupBox2.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(782, 187);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ENTRY REKENING";
            // 
            // txtKd
            // 
            this.txtKd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKd.Location = new System.Drawing.Point(182, 19);
            this.txtKd.Mask = "0000,00,000,000,00,000  ";
            this.txtKd.Name = "txtKd";
            this.txtKd.ResetOnSpace = false;
            this.txtKd.Size = new System.Drawing.Size(208, 26);
            this.txtKd.SkipLiterals = false;
            this.txtKd.TabIndex = 0;
            this.txtKd.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.txtKd_MaskInputRejected);
            this.txtKd.Leave += new System.EventHandler(this.txtKd_Leave);
            this.txtKd.TextChanged += new System.EventHandler(this.txtKd_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(140, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 30);
            this.label7.TabIndex = 13;
            this.label7.Text = ":";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(16, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 18);
            this.label8.TabIndex = 12;
            this.label8.Text = "NAMA";
            // 
            // ckdKPA
            // 
            this.ckdKPA.FormattingEnabled = true;
            this.ckdKPA.Location = new System.Drawing.Point(182, 130);
            this.ckdKPA.Name = "ckdKPA";
            this.ckdKPA.Size = new System.Drawing.Size(104, 26);
            this.ckdKPA.TabIndex = 2;
            this.ckdKPA.SelectedIndexChanged += new System.EventHandler(this.ckdKPA_SelectedIndexChanged);
            this.ckdKPA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ckdKPA_KeyPress);
            this.ckdKPA.TextChanged += new System.EventHandler(this.ckdKPA_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(140, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 30);
            this.label4.TabIndex = 7;
            this.label4.Text = ":";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(16, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "KODE KPA";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(16, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "URAIAN";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(16, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "KODE REK";
            // 
            // txtUraian
            // 
            this.txtUraian.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUraian.Font = new System.Drawing.Font("Arial Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUraian.ForeColor = System.Drawing.Color.Black;
            this.txtUraian.Location = new System.Drawing.Point(182, 51);
            this.txtUraian.MaxLength = 3546;
            this.txtUraian.Multiline = true;
            this.txtUraian.Name = "txtUraian";
            this.txtUraian.Size = new System.Drawing.Size(345, 73);
            this.txtUraian.TabIndex = 1;
            this.txtUraian.Text = "TEST";
            // 
            // lNama
            // 
            this.lNama.AutoSize = true;
            this.lNama.Font = new System.Drawing.Font("Arial Black", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lNama.Location = new System.Drawing.Point(176, 152);
            this.lNama.Name = "lNama";
            this.lNama.Size = new System.Drawing.Size(153, 33);
            this.lNama.TabIndex = 37;
            this.lNama.Text = "KODE OPP";
            // 
            // bKeluar
            // 
            this.bKeluar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bKeluar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bKeluar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bKeluar.Location = new System.Drawing.Point(840, 100);
            this.bKeluar.Name = "bKeluar";
            this.bKeluar.Size = new System.Drawing.Size(155, 37);
            this.bKeluar.TabIndex = 4;
            this.bKeluar.Text = "K&ELUAR";
            this.bKeluar.UseVisualStyleBackColor = true;
            this.bKeluar.Click += new System.EventHandler(this.bKeluar_Click);
            // 
            // bSimpan
            // 
            this.bSimpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSimpan.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSimpan.Image = global::RealAnggaran.Properties.Resources.Folder___Factory_Bank;
            this.bSimpan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSimpan.Location = new System.Drawing.Point(840, 43);
            this.bSimpan.Name = "bSimpan";
            this.bSimpan.Size = new System.Drawing.Size(155, 36);
            this.bSimpan.TabIndex = 3;
            this.bSimpan.Text = "&SIMPAN";
            this.bSimpan.UseVisualStyleBackColor = true;
            this.bSimpan.Click += new System.EventHandler(this.bSimpan_Click);
            // 
            // fMRek
            // 
            this.AcceptButton = this.bSimpan;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bKeluar;
            this.ClientSize = new System.Drawing.Size(1036, 591);
            this.Controls.Add(this.bSimpan);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.bKeluar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "fMRek";
            this.Text = "fMRek";
            this.Load += new System.EventHandler(this.fMRek_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bSimpan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvTampil;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUraian;
        private System.Windows.Forms.Button bKeluar;
        private System.Windows.Forms.ComboBox ckdKPA;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MaskedTextBox txtKd;
        private System.Windows.Forms.Label lNama;
    }
}