namespace TPR_ExampleView
{
    partial class ImageInfo
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.открытьФайлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.переименоватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.показатьФормуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выбратьТекущимИзображениемToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьИзображениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.закрытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.убратьЗаписьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.закрытьФормуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.закрытьИУбратьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lName = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lType = new System.Windows.Forms.Label();
            this.lStat = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ContextMenuStrip = this.contextMenuStrip1;
            this.tableLayoutPanel1.Controls.Add(this.lName, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lType, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lStat, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(111, 48);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Click += new System.EventHandler(this.PictureBox1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьФайлToolStripMenuItem,
            this.переименоватьToolStripMenuItem,
            this.показатьФормуToolStripMenuItem,
            this.выбратьТекущимИзображениемToolStripMenuItem,
            this.сохранитьИзображениеToolStripMenuItem,
            this.закрытьToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(261, 158);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // открытьФайлToolStripMenuItem
            // 
            this.открытьФайлToolStripMenuItem.Name = "открытьФайлToolStripMenuItem";
            this.открытьФайлToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.открытьФайлToolStripMenuItem.Text = "Открыть файл";
            this.открытьФайлToolStripMenuItem.Click += new System.EventHandler(this.ОткрытьФайлToolStripMenuItem_Click);
            // 
            // переименоватьToolStripMenuItem
            // 
            this.переименоватьToolStripMenuItem.Name = "переименоватьToolStripMenuItem";
            this.переименоватьToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.переименоватьToolStripMenuItem.Text = "Переименовать";
            this.переименоватьToolStripMenuItem.Click += new System.EventHandler(this.ПереименоватьToolStripMenuItem_Click);
            // 
            // показатьФормуToolStripMenuItem
            // 
            this.показатьФормуToolStripMenuItem.Name = "показатьФормуToolStripMenuItem";
            this.показатьФормуToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.показатьФормуToolStripMenuItem.Text = "Показать форму";
            this.показатьФормуToolStripMenuItem.Click += new System.EventHandler(this.ПоказатьФормуToolStripMenuItem_Click);
            // 
            // выбратьТекущимИзображениемToolStripMenuItem
            // 
            this.выбратьТекущимИзображениемToolStripMenuItem.Name = "выбратьТекущимИзображениемToolStripMenuItem";
            this.выбратьТекущимИзображениемToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.выбратьТекущимИзображениемToolStripMenuItem.Text = "Выбрать текущим изображением";
            this.выбратьТекущимИзображениемToolStripMenuItem.Click += new System.EventHandler(this.ВыбратьТекущимИзображениемToolStripMenuItem_Click);
            // 
            // сохранитьИзображениеToolStripMenuItem
            // 
            this.сохранитьИзображениеToolStripMenuItem.Name = "сохранитьИзображениеToolStripMenuItem";
            this.сохранитьИзображениеToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.сохранитьИзображениеToolStripMenuItem.Text = "Сохранить изображение как...";
            this.сохранитьИзображениеToolStripMenuItem.Click += new System.EventHandler(this.СохранитьИзображениеToolStripMenuItem_Click);
            // 
            // закрытьToolStripMenuItem
            // 
            this.закрытьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.убратьЗаписьToolStripMenuItem,
            this.закрытьФормуToolStripMenuItem,
            this.закрытьИУбратьToolStripMenuItem});
            this.закрытьToolStripMenuItem.Name = "закрытьToolStripMenuItem";
            this.закрытьToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.закрытьToolStripMenuItem.Text = "Закрыть";
            // 
            // убратьЗаписьToolStripMenuItem
            // 
            this.убратьЗаписьToolStripMenuItem.Name = "убратьЗаписьToolStripMenuItem";
            this.убратьЗаписьToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.убратьЗаписьToolStripMenuItem.Text = "Убрать запись";
            this.убратьЗаписьToolStripMenuItem.Click += new System.EventHandler(this.УбратьЗаписьToolStripMenuItem_Click);
            // 
            // закрытьФормуToolStripMenuItem
            // 
            this.закрытьФормуToolStripMenuItem.Name = "закрытьФормуToolStripMenuItem";
            this.закрытьФормуToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.закрытьФормуToolStripMenuItem.Text = "Закрыть форму";
            this.закрытьФормуToolStripMenuItem.Click += new System.EventHandler(this.ЗакрытьФормуToolStripMenuItem_Click);
            // 
            // закрытьИУбратьToolStripMenuItem
            // 
            this.закрытьИУбратьToolStripMenuItem.Name = "закрытьИУбратьToolStripMenuItem";
            this.закрытьИУбратьToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.закрытьИУбратьToolStripMenuItem.Text = "Закрыть и убрать";
            this.закрытьИУбратьToolStripMenuItem.Click += new System.EventHandler(this.ЗакрытьИУбратьToolStripMenuItem_Click);
            // 
            // lName
            // 
            this.lName.AutoEllipsis = true;
            this.lName.AutoSize = true;
            this.lName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lName.Location = new System.Drawing.Point(71, 3);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(37, 13);
            this.lName.TabIndex = 1;
            this.lName.Text = "Name";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox1.Name = "checkBox1";
            this.tableLayoutPanel1.SetRowSpan(this.checkBox1, 3);
            this.checkBox1.Size = new System.Drawing.Size(20, 48);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lType.Location = new System.Drawing.Point(71, 19);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(37, 13);
            this.lType.TabIndex = 3;
            this.lType.Text = "Type";
            // 
            // lStat
            // 
            this.lStat.AutoSize = true;
            this.lStat.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lStat.Location = new System.Drawing.Point(71, 35);
            this.lStat.Name = "lStat";
            this.lStat.Size = new System.Drawing.Size(37, 13);
            this.lStat.TabIndex = 4;
            this.lStat.Text = "Status";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImage = global::TPR_ExampleView.Properties.Resources.закрыть_окно_32;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(20, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 3);
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.PictureBox1_Click);
            // 
            // ImageInfo
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ImageInfo";
            this.Size = new System.Drawing.Size(111, 48);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lName;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.Label lStat;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem открытьФайлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выбратьТекущимИзображениемToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem закрытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem убратьЗаписьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem закрытьФормуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem закрытьИУбратьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem показатьФормуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem переименоватьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьИзображениеToolStripMenuItem;
    }
}
