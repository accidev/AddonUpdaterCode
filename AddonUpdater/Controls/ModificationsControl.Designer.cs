namespace AddonUpdater.Controls
{
    partial class ModificationsControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            panel2 = new System.Windows.Forms.Panel();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            btnBackupWTF = new System.Windows.Forms.Button();
            ComboBoxWTF = new System.Windows.Forms.ComboBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            btnRestoreWTF = new System.Windows.Forms.Button();
            panel3 = new System.Windows.Forms.Panel();
            lblPatchStatus = new System.Windows.Forms.Label();
            btnInstallPatch = new System.Windows.Forms.Button();
            pictureBoxPatch = new System.Windows.Forms.PictureBox();
            label2 = new System.Windows.Forms.Label();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPatch).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.FromArgb(37, 35, 47);
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(btnBackupWTF);
            panel2.Controls.Add(ComboBoxWTF);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(btnRestoreWTF);
            panel2.Location = new System.Drawing.Point(30, 12);
            panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(295, 488);
            panel2.TabIndex = 1;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = Properties.Resources.explorer;
            pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBox2.Location = new System.Drawing.Point(248, 352);
            pictureBox2.Margin = new System.Windows.Forms.Padding(0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(47, 44);
            pictureBox2.TabIndex = 38;
            pictureBox2.TabStop = false;
            pictureBox2.Click += PictureBox2_Click;
            // 
            // btnBackupWTF
            // 
            btnBackupWTF.BackColor = System.Drawing.Color.FromArgb(44, 42, 63);
            btnBackupWTF.FlatAppearance.BorderSize = 0;
            btnBackupWTF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBackupWTF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnBackupWTF.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            btnBackupWTF.Location = new System.Drawing.Point(45, 350);
            btnBackupWTF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnBackupWTF.Name = "btnBackupWTF";
            btnBackupWTF.Size = new System.Drawing.Size(200, 50);
            btnBackupWTF.TabIndex = 37;
            btnBackupWTF.TabStop = false;
            btnBackupWTF.Text = "\tСохранить WTF";
            btnBackupWTF.UseVisualStyleBackColor = false;
            btnBackupWTF.Click += BtnBackupWTF_Click;
            // 
            // ComboBoxWTF
            // 
            ComboBoxWTF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ComboBoxWTF.FormattingEnabled = true;
            ComboBoxWTF.Items.AddRange(new object[] { "При запуске программы", "Раз в день", "Раз в два дня", "Раз в три дня", "Раз в неделю", "Раз в месяц", "Никогда" });
            ComboBoxWTF.Location = new System.Drawing.Point(45, 301);
            ComboBoxWTF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            ComboBoxWTF.Name = "ComboBoxWTF";
            ComboBoxWTF.Size = new System.Drawing.Size(200, 28);
            ComboBoxWTF.TabIndex = 36;
            ComboBoxWTF.TextChanged += ComboBoxWTF_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.PatchX;
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBox1.Location = new System.Drawing.Point(45, 12);
            pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(200, 212);
            pictureBox1.TabIndex = 33;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(20, 225);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(260, 88);
            label1.TabIndex = 29;
            label1.Text = "Автосохранение настроек персонажей";
            // 
            // btnRestoreWTF
            // 
            btnRestoreWTF.BackColor = System.Drawing.Color.FromArgb(44, 42, 63);
            btnRestoreWTF.FlatAppearance.BorderSize = 0;
            btnRestoreWTF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRestoreWTF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnRestoreWTF.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            btnRestoreWTF.Location = new System.Drawing.Point(45, 418);
            btnRestoreWTF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnRestoreWTF.Name = "btnRestoreWTF";
            btnRestoreWTF.Size = new System.Drawing.Size(200, 50);
            btnRestoreWTF.TabIndex = 4;
            btnRestoreWTF.TabStop = false;
            btnRestoreWTF.Text = "\tВосстановить WTF";
            btnRestoreWTF.UseVisualStyleBackColor = false;
            btnRestoreWTF.Click += BtnRestoreWTF_Click;
            // 
            // panel3
            // 
            panel3.BackColor = System.Drawing.Color.FromArgb(37, 35, 47);
            panel3.Controls.Add(lblPatchStatus);
            panel3.Controls.Add(btnInstallPatch);
            panel3.Controls.Add(pictureBoxPatch);
            panel3.Controls.Add(label2);
            panel3.Location = new System.Drawing.Point(350, 12);
            panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(295, 488);
            panel3.TabIndex = 2;
            // 
            // lblPatchStatus
            // 
            lblPatchStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblPatchStatus.ForeColor = System.Drawing.Color.White;
            lblPatchStatus.Location = new System.Drawing.Point(20, 320);
            lblPatchStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPatchStatus.Name = "lblPatchStatus";
            lblPatchStatus.Size = new System.Drawing.Size(260, 50);
            lblPatchStatus.TabIndex = 39;
            lblPatchStatus.Text = "Статус: Не установлен";
            // 
            // btnInstallPatch
            // 
            btnInstallPatch.BackColor = System.Drawing.Color.FromArgb(44, 42, 63);
            btnInstallPatch.FlatAppearance.BorderSize = 0;
            btnInstallPatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnInstallPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnInstallPatch.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            btnInstallPatch.Location = new System.Drawing.Point(45, 390);
            btnInstallPatch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnInstallPatch.Name = "btnInstallPatch";
            btnInstallPatch.Size = new System.Drawing.Size(200, 50);
            btnInstallPatch.TabIndex = 37;
            btnInstallPatch.TabStop = false;
            btnInstallPatch.Text = "Установить патч";
            btnInstallPatch.UseVisualStyleBackColor = false;
            btnInstallPatch.Click += BtnInstallPatch_Click;
            // 
            // pictureBoxPatch
            // 
            pictureBoxPatch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBoxPatch.Location = new System.Drawing.Point(45, 12);
            pictureBoxPatch.Margin = new System.Windows.Forms.Padding(0);
            pictureBoxPatch.Name = "pictureBoxPatch";
            pictureBoxPatch.Size = new System.Drawing.Size(200, 212);
            pictureBoxPatch.TabIndex = 33;
            pictureBoxPatch.TabStop = false;
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(20, 225);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(260, 88);
            label2.TabIndex = 29;
            label2.Text = "Патч на FPS";
            // 
            // ModificationsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(44, 42, 63);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "ModificationsControl";
            Size = new System.Drawing.Size(1070, 800);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxPatch).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRestoreWTF;
        private System.Windows.Forms.ComboBox ComboBoxWTF;
        private System.Windows.Forms.Button btnBackupWTF;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblPatchStatus;
        private System.Windows.Forms.Button btnInstallPatch;
        private System.Windows.Forms.PictureBox pictureBoxPatch;
        private System.Windows.Forms.Label label2;
    }
}
