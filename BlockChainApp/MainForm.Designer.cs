namespace BlockChainApp
{
    partial class MainForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBox_Log = new System.Windows.Forms.TextBox();
            this.ListBox_Users = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // TextBox_Log
            // 
            this.TextBox_Log.Location = new System.Drawing.Point(12, 45);
            this.TextBox_Log.Multiline = true;
            this.TextBox_Log.Name = "TextBox_Log";
            this.TextBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox_Log.Size = new System.Drawing.Size(643, 530);
            this.TextBox_Log.TabIndex = 1;
            // 
            // ListBox_Users
            // 
            this.ListBox_Users.FormattingEnabled = true;
            this.ListBox_Users.Location = new System.Drawing.Point(661, 12);
            this.ListBox_Users.Name = "ListBox_Users";
            this.ListBox_Users.Size = new System.Drawing.Size(338, 563);
            this.ListBox_Users.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 594);
            this.Controls.Add(this.ListBox_Users);
            this.Controls.Add(this.TextBox_Log);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TextBox_Log;
        private System.Windows.Forms.ListBox ListBox_Users;
    }
}

