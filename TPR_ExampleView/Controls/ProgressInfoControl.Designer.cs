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
            this.lProgress = new System.Windows.Forms.Label();
            this.lTimeStart = new System.Windows.Forms.Label();
            this.lDuratuin = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bAbort = new System.Windows.Forms.Button();
            this.bPause = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.lName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lProgress, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lTimeStart, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lDuratuin, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bPause, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.bCancel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.bAbort, 2, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(219, 113);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lName
            // 
            this.lName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lName, 2);
            this.lName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lName.Location = new System.Drawing.Point(3, 0);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(140, 13);
            this.lName.TabIndex = 1;
            this.lName.Text = "MethodName";
            // 
            // lProgress
            // 
            this.lProgress.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lProgress, 2);
            this.lProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lProgress.Location = new System.Drawing.Point(3, 48);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(140, 13);
            this.lProgress.TabIndex = 2;
            this.lProgress.Text = "Progress (0/100)";
            // 
            // lTimeStart
            // 
            this.lTimeStart.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lTimeStart, 2);
            this.lTimeStart.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lTimeStart.Location = new System.Drawing.Point(3, 16);
            this.lTimeStart.Name = "lTimeStart";
            this.lTimeStart.Size = new System.Drawing.Size(140, 13);
            this.lTimeStart.TabIndex = 3;
            this.lTimeStart.Text = "TimeStart 12:00:00";
            // 
            // lDuratuin
            // 
            this.lDuratuin.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lDuratuin, 2);
            this.lDuratuin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lDuratuin.Location = new System.Drawing.Point(3, 32);
            this.lDuratuin.Name = "lDuratuin";
            this.lDuratuin.Size = new System.Drawing.Size(140, 13);
            this.lDuratuin.TabIndex = 3;
            this.lDuratuin.Text = "Duration 5s";
            // 
            // bCancel
            // 
            this.bCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bCancel.ForeColor = System.Drawing.Color.Red;
            this.bCancel.Location = new System.Drawing.Point(97, 90);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(24, 20);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "X";
            this.toolTip1.SetToolTip(this.bCancel, "Отменить задачу");
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.Button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // bAbort
            // 
            this.bAbort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bAbort.ForeColor = System.Drawing.Color.Red;
            this.bAbort.Location = new System.Drawing.Point(170, 90);
            this.bAbort.Name = "bAbort";
            this.bAbort.Size = new System.Drawing.Size(24, 20);
            this.bAbort.TabIndex = 4;
            this.bAbort.Text = "!!";
            this.toolTip1.SetToolTip(this.bAbort, "Прервать задачу принудительно");
            this.bAbort.UseVisualStyleBackColor = true;
            this.bAbort.Click += new System.EventHandler(this.BAbort_Click);
            // 
            // bPause
            // 
            this.bPause.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bPause.ForeColor = System.Drawing.Color.Red;
            this.bPause.Location = new System.Drawing.Point(24, 90);
            this.bPause.Name = "bPause";
            this.bPause.Size = new System.Drawing.Size(24, 20);
            this.bPause.TabIndex = 4;
            this.bPause.Text = "||";
            this.toolTip1.SetToolTip(this.bPause, "Пауза");
            this.bPause.UseVisualStyleBackColor = true;
            this.bPause.Click += new System.EventHandler(this.BPause_Click);
            // 
            // ProgressInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ProgressInfoControl";
            this.Size = new System.Drawing.Size(219, 113);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lName;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Label lTimeStart;
        private System.Windows.Forms.Label lDuratuin;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button bPause;
        private System.Windows.Forms.Button bAbort;
    }
}
