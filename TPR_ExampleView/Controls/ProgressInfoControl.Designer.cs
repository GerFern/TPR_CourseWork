namespace TPR_ExampleView
{
    partial class ProgressInfoControl
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lName = new System.Windows.Forms.Label();
            this.lTimeStart = new System.Windows.Forms.Label();
            this.lDuratuin = new System.Windows.Forms.Label();
            this.bPause = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lProgress = new System.Windows.Forms.Label();
            this.bAbort = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.закрытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ContextMenuStrip = this.contextMenuStrip1;
            this.tableLayoutPanel1.Controls.Add(this.lName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lTimeStart, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lDuratuin, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 111);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lName
            // 
            this.lName.AutoSize = true;
            this.lName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lName.Location = new System.Drawing.Point(3, 0);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(194, 13);
            this.lName.TabIndex = 1;
            this.lName.Text = "MethodName";
            // 
            // lTimeStart
            // 
            this.lTimeStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lTimeStart.AutoSize = true;
            this.lTimeStart.Location = new System.Drawing.Point(3, 16);
            this.lTimeStart.Name = "lTimeStart";
            this.lTimeStart.Size = new System.Drawing.Size(97, 13);
            this.lTimeStart.TabIndex = 3;
            this.lTimeStart.Text = "TimeStart 12:00:00";
            // 
            // lDuratuin
            // 
            this.lDuratuin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lDuratuin.AutoSize = true;
            this.lDuratuin.Location = new System.Drawing.Point(3, 32);
            this.lDuratuin.Name = "lDuratuin";
            this.lDuratuin.Size = new System.Drawing.Size(61, 13);
            this.lDuratuin.TabIndex = 3;
            this.lDuratuin.Text = "Duration 5s";
            // 
            // bPause
            // 
            this.bPause.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bPause.BackgroundImage = global::TPR_ExampleView.Properties.Resources.воспроизведение_32;
            this.bPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bPause.Location = new System.Drawing.Point(21, 0);
            this.bPause.Margin = new System.Windows.Forms.Padding(0);
            this.bPause.Name = "bPause";
            this.bPause.Size = new System.Drawing.Size(24, 24);
            this.bPause.TabIndex = 4;
            this.bPause.TabStop = false;
            this.toolTip1.SetToolTip(this.bPause, "Пауза");
            this.bPause.UseVisualStyleBackColor = false;
            this.bPause.Click += new System.EventHandler(this.BPause_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bCancel.BackgroundImage = global::TPR_ExampleView.Properties.Resources.стоп_32;
            this.bCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bCancel.Location = new System.Drawing.Point(87, 0);
            this.bCancel.Margin = new System.Windows.Forms.Padding(0);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(24, 24);
            this.bCancel.TabIndex = 4;
            this.bCancel.TabStop = false;
            this.toolTip1.SetToolTip(this.bCancel, "Отменить задачу");
            this.bCancel.UseVisualStyleBackColor = false;
            this.bCancel.Click += new System.EventHandler(this.Button1_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lProgress, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 45);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 16);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // lProgress
            // 
            this.lProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(3, 3);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(86, 13);
            this.lProgress.TabIndex = 2;
            this.lProgress.Text = "Progress (0/100)";
            // 
            // bAbort
            // 
            this.bAbort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bAbort.BackgroundImage = global::TPR_ExampleView.Properties.Resources.закрыть_окно_32;
            this.bAbort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bAbort.Location = new System.Drawing.Point(154, 0);
            this.bAbort.Margin = new System.Windows.Forms.Padding(0);
            this.bAbort.Name = "bAbort";
            this.bAbort.Size = new System.Drawing.Size(24, 24);
            this.bAbort.TabIndex = 4;
            this.bAbort.TabStop = false;
            this.toolTip1.SetToolTip(this.bAbort, "Прервать задачу принудительно");
            this.bAbort.UseVisualStyleBackColor = false;
            this.bAbort.Click += new System.EventHandler(this.BAbort_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.bPause, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.bCancel, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.bAbort, 2, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 87);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 24);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.закрытьToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 26);
            // 
            // закрытьToolStripMenuItem
            // 
            this.закрытьToolStripMenuItem.Name = "закрытьToolStripMenuItem";
            this.закрытьToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.закрытьToolStripMenuItem.Text = "Закрыть";
            this.закрытьToolStripMenuItem.Click += new System.EventHandler(this.ЗакрытьToolStripMenuItem_Click);
            // 
            // ProgressInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ProgressInfoControl";
            this.Size = new System.Drawing.Size(200, 111);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lName;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Label lTimeStart;
        private System.Windows.Forms.Label lDuratuin;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button bPause;
        private System.Windows.Forms.Button bAbort;
        internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem закрытьToolStripMenuItem;
    }
}
