
namespace Pickaxe.Studio
{
    partial class EditControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.progressText = new System.Windows.Forms.ToolStripStatusLabel();
            this.resultsTabs = new System.Windows.Forms.TabControl();
            this.resultsTabPage = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.messagesTextBox = new System.Windows.Forms.TextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.resultsListView = new HighlightListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.resultsTabs.SuspendLayout();
            this.resultsTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textEditorControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.resultsTabs);
            this.splitContainer1.Size = new System.Drawing.Size(622, 418);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // textEditorControl1
            // 
            this.textEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControl1.IsReadOnly = false;
            this.textEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.textEditorControl1.Margin = new System.Windows.Forms.Padding(2);
            this.textEditorControl1.Name = "textEditorControl1";
            this.textEditorControl1.ShowVRuler = false;
            this.textEditorControl1.Size = new System.Drawing.Size(622, 240);
            this.textEditorControl1.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.progressBar,
            this.progressText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 151);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(622, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 17);
            this.statusLabel.Text = "Ready.";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            this.progressBar.Visible = false;
            // 
            // progressText
            // 
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(24, 17);
            this.progressText.Text = "0/0";
            this.progressText.Visible = false;
            // 
            // resultsTabs
            // 
            this.resultsTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsTabs.Controls.Add(this.resultsTabPage);
            this.resultsTabs.Controls.Add(this.tabPage2);
            this.resultsTabs.ImageList = this.imageList1;
            this.resultsTabs.Location = new System.Drawing.Point(0, 3);
            this.resultsTabs.Name = "resultsTabs";
            this.resultsTabs.SelectedIndex = 0;
            this.resultsTabs.Size = new System.Drawing.Size(619, 142);
            this.resultsTabs.TabIndex = 3;
            // 
            // resultsTabPage
            // 
            this.resultsTabPage.Controls.Add(this.resultsListView);
            this.resultsTabPage.ImageIndex = 1;
            this.resultsTabPage.Location = new System.Drawing.Point(4, 23);
            this.resultsTabPage.Name = "resultsTabPage";
            this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.resultsTabPage.Size = new System.Drawing.Size(611, 115);
            this.resultsTabPage.TabIndex = 0;
            this.resultsTabPage.Text = "Results";
            this.resultsTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.messagesTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(611, 115);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Messages";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // messagesTextBox
            // 
            this.messagesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesTextBox.Location = new System.Drawing.Point(3, 3);
            this.messagesTextBox.Multiline = true;
            this.messagesTextBox.Name = "messagesTextBox";
            this.messagesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messagesTextBox.Size = new System.Drawing.Size(605, 109);
            this.messagesTextBox.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "StatusAnnotations_Critical_16xLG_color.png");
            this.imageList1.Images.SetKeyName(1, "TableHS.png");
            // 
            // resultsListView
            // 
            this.resultsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsListView.Location = new System.Drawing.Point(3, 3);
            this.resultsListView.Name = "resultsListView";
            this.resultsListView.Size = new System.Drawing.Size(605, 109);
            this.resultsListView.TabIndex = 0;
            this.resultsListView.UseCompatibleStateImageBehavior = false;
            this.resultsListView.View = System.Windows.Forms.View.Details;
            // 
            // EditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EditControl";
            this.Size = new System.Drawing.Size(622, 418);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.resultsTabs.ResumeLayout(false);
            this.resultsTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ICSharpCode.TextEditor.TextEditorControl textEditorControl1;
        private System.Windows.Forms.TabControl resultsTabs;
        private System.Windows.Forms.TabPage resultsTabPage;
        private HighlightListView resultsListView;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox messagesTextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel progressText;
        private System.Windows.Forms.ImageList imageList1;
    }
}
