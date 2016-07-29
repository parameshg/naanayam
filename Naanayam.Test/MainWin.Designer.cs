namespace Naanayam.Test
{
    partial class MainWin
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
            this.Uninstall = new System.Windows.Forms.Button();
            this.Install = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Uninstall
            // 
            this.Uninstall.Location = new System.Drawing.Point(105, 38);
            this.Uninstall.Name = "Uninstall";
            this.Uninstall.Size = new System.Drawing.Size(75, 23);
            this.Uninstall.TabIndex = 0;
            this.Uninstall.Text = "Uninstall";
            this.Uninstall.UseVisualStyleBackColor = true;
            this.Uninstall.Click += new System.EventHandler(this.Uninstall_Click);
            // 
            // Install
            // 
            this.Install.Location = new System.Drawing.Point(105, 119);
            this.Install.Name = "Install";
            this.Install.Size = new System.Drawing.Size(75, 23);
            this.Install.TabIndex = 1;
            this.Install.Text = "Install";
            this.Install.UseVisualStyleBackColor = true;
            this.Install.Click += new System.EventHandler(this.Install_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(82, 185);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.Install);
            this.Controls.Add(this.Uninstall);
            this.Name = "MainWin";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Uninstall;
        private System.Windows.Forms.Button Install;
        private System.Windows.Forms.Button btnTest;
    }
}

