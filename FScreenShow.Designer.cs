namespace ScreenCapturer
{
    partial class FScreenShow
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
            this.SuspendLayout();
            // 
            // FScreenShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 375);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FScreenShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "FScreen";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FScreen_FormClosed);
            this.Load += new System.EventHandler(this.FScreen_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FScreen_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FScreen_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}