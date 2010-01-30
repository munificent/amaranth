namespace Amaranth.Tools.Dungeons
{
    partial class DungeonForm
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
            this.mMakeButton = new System.Windows.Forms.Button();
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mDungeonView = new Amaranth.Tools.Dungeons.DungeonView();
            this.mLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.mDepthLabel = new System.Windows.Forms.Label();
            this.mSeedTextBox = new System.Windows.Forms.TextBox();
            this.mReseedCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mPosLabel = new System.Windows.Forms.Label();
            this.mSplitContainer.Panel1.SuspendLayout();
            this.mSplitContainer.Panel2.SuspendLayout();
            this.mSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mLevelTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // mMakeButton
            // 
            this.mMakeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mMakeButton.Location = new System.Drawing.Point(659, 561);
            this.mMakeButton.Name = "mMakeButton";
            this.mMakeButton.Size = new System.Drawing.Size(75, 23);
            this.mMakeButton.TabIndex = 1;
            this.mMakeButton.Text = "Make";
            this.mMakeButton.UseVisualStyleBackColor = true;
            this.mMakeButton.Click += new System.EventHandler(this.MakeButton_Click);
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.Size = new System.Drawing.Size(245, 543);
            this.mPropertyGrid.TabIndex = 2;
            // 
            // mSplitContainer
            // 
            this.mSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mSplitContainer.Location = new System.Drawing.Point(12, 12);
            this.mSplitContainer.Name = "mSplitContainer";
            // 
            // mSplitContainer.Panel1
            // 
            this.mSplitContainer.Panel1.Controls.Add(this.mDungeonView);
            // 
            // mSplitContainer.Panel2
            // 
            this.mSplitContainer.Panel2.Controls.Add(this.mPropertyGrid);
            this.mSplitContainer.Size = new System.Drawing.Size(722, 543);
            this.mSplitContainer.SplitterDistance = 473;
            this.mSplitContainer.TabIndex = 3;
            // 
            // mDungeonView
            // 
            this.mDungeonView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mDungeonView.Location = new System.Drawing.Point(0, 0);
            this.mDungeonView.Name = "mDungeonView";
            this.mDungeonView.Size = new System.Drawing.Size(473, 543);
            this.mDungeonView.TabIndex = 0;
            this.mDungeonView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mDungeonView_MouseMove);
            // 
            // mLevelTrackBar
            // 
            this.mLevelTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mLevelTrackBar.AutoSize = false;
            this.mLevelTrackBar.LargeChange = 10;
            this.mLevelTrackBar.Location = new System.Drawing.Point(408, 561);
            this.mLevelTrackBar.Maximum = 100;
            this.mLevelTrackBar.Minimum = 1;
            this.mLevelTrackBar.Name = "mLevelTrackBar";
            this.mLevelTrackBar.Size = new System.Drawing.Size(179, 23);
            this.mLevelTrackBar.TabIndex = 10;
            this.mLevelTrackBar.TickFrequency = 10;
            this.mLevelTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mLevelTrackBar.Value = 1;
            this.mLevelTrackBar.ValueChanged += new System.EventHandler(this.mLevelTrackBar_ValueChanged);
            // 
            // mDepthLabel
            // 
            this.mDepthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mDepthLabel.AutoSize = true;
            this.mDepthLabel.Location = new System.Drawing.Point(593, 566);
            this.mDepthLabel.Name = "mDepthLabel";
            this.mDepthLabel.Size = new System.Drawing.Size(60, 13);
            this.mDepthLabel.TabIndex = 11;
            this.mDepthLabel.Text = "Depth: 100";
            // 
            // mSeedTextBox
            // 
            this.mSeedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mSeedTextBox.Location = new System.Drawing.Point(295, 563);
            this.mSeedTextBox.Name = "mSeedTextBox";
            this.mSeedTextBox.Size = new System.Drawing.Size(88, 20);
            this.mSeedTextBox.TabIndex = 12;
            // 
            // mReseedCheckBox
            // 
            this.mReseedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mReseedCheckBox.AutoSize = true;
            this.mReseedCheckBox.Checked = true;
            this.mReseedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mReseedCheckBox.Location = new System.Drawing.Point(389, 565);
            this.mReseedCheckBox.Name = "mReseedCheckBox";
            this.mReseedCheckBox.Size = new System.Drawing.Size(15, 14);
            this.mReseedCheckBox.TabIndex = 13;
            this.mReseedCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(254, 566);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Seed:";
            // 
            // mPosLabel
            // 
            this.mPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mPosLabel.AutoSize = true;
            this.mPosLabel.Location = new System.Drawing.Point(9, 566);
            this.mPosLabel.Name = "mPosLabel";
            this.mPosLabel.Size = new System.Drawing.Size(42, 13);
            this.mPosLabel.TabIndex = 15;
            this.mPosLabel.Text = "Mouse:";
            // 
            // DungeonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 596);
            this.Controls.Add(this.mPosLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mReseedCheckBox);
            this.Controls.Add(this.mSeedTextBox);
            this.Controls.Add(this.mDepthLabel);
            this.Controls.Add(this.mLevelTrackBar);
            this.Controls.Add(this.mSplitContainer);
            this.Controls.Add(this.mMakeButton);
            this.Name = "DungeonForm";
            this.Text = "Dungeon Generator";
            this.mSplitContainer.Panel1.ResumeLayout(false);
            this.mSplitContainer.Panel2.ResumeLayout(false);
            this.mSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mLevelTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DungeonView mDungeonView;
        private System.Windows.Forms.Button mMakeButton;
        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.SplitContainer mSplitContainer;
        private System.Windows.Forms.TrackBar mLevelTrackBar;
        private System.Windows.Forms.Label mDepthLabel;
        private System.Windows.Forms.TextBox mSeedTextBox;
        private System.Windows.Forms.CheckBox mReseedCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mPosLabel;
    }
}

