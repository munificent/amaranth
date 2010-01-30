namespace Amaranth.Terminals
{
    partial class TerminalForm
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
            this.mTerminalControl = new Amaranth.Terminals.TerminalControl();
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m6x10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m7x10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m8x12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m10x12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTerminalControl
            // 
            this.mTerminalControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTerminalControl.Location = new System.Drawing.Point(0, 24);
            this.mTerminalControl.Name = "mTerminalControl";
            this.mTerminalControl.Size = new System.Drawing.Size(284, 240);
            this.mTerminalControl.TabIndex = 0;
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.fontToolStripMenuItem});
            this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMenuStrip.Name = "mMenuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size(284, 24);
            this.mMenuStrip.TabIndex = 1;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mExitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mExitToolStripMenuItem
            // 
            this.mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
            this.mExitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.mExitToolStripMenuItem.Text = "Exit";
            this.mExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m6x10ToolStripMenuItem,
            this.m7x10ToolStripMenuItem,
            this.m8x12ToolStripMenuItem,
            this.m10x12ToolStripMenuItem});
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.DropDownOpening += new System.EventHandler(this.FontToolStripMenuItem_DropDownOpening);
            // 
            // m6x10ToolStripMenuItem
            // 
            this.m6x10ToolStripMenuItem.Name = "m6x10ToolStripMenuItem";
            this.m6x10ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.m6x10ToolStripMenuItem.Text = "6 x 10";
            this.m6x10ToolStripMenuItem.Click += new System.EventHandler(this.Font6x10ToolStripMenuItem_Click);
            // 
            // m7x10ToolStripMenuItem
            // 
            this.m7x10ToolStripMenuItem.Name = "m7x10ToolStripMenuItem";
            this.m7x10ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.m7x10ToolStripMenuItem.Text = "7 x 10";
            this.m7x10ToolStripMenuItem.Click += new System.EventHandler(this.Font7x10ToolStripMenuItem1_Click);
            // 
            // m8x12ToolStripMenuItem
            // 
            this.m8x12ToolStripMenuItem.Name = "m8x12ToolStripMenuItem";
            this.m8x12ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.m8x12ToolStripMenuItem.Text = "8 x 12";
            this.m8x12ToolStripMenuItem.Click += new System.EventHandler(this.Font8x12ToolStripMenuItem_Click);
            // 
            // m10x12ToolStripMenuItem
            // 
            this.m10x12ToolStripMenuItem.Name = "m10x12ToolStripMenuItem";
            this.m10x12ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.m10x12ToolStripMenuItem.Text = "10 x 12";
            this.m10x12ToolStripMenuItem.Click += new System.EventHandler(this.Font10x12ToolStripMenuItem_Click);
            // 
            // TerminalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.mTerminalControl);
            this.Controls.Add(this.mMenuStrip);
            this.MainMenuStrip = this.mMenuStrip;
            this.Name = "TerminalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Terminal";
            this.mMenuStrip.ResumeLayout(false);
            this.mMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Amaranth.Terminals.TerminalControl mTerminalControl;
        private System.Windows.Forms.MenuStrip mMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m6x10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m7x10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m8x12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m10x12ToolStripMenuItem;
    }
}