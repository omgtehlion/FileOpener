namespace FileOpener.Opener
{
    partial class MyControl
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
            if (disposing && (components != null)) {
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
            this.textBoxQuery = new System.Windows.Forms.TextBox();
            this.labelTerms = new System.Windows.Forms.Label();
            this.contextMenuStripOpen = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.reindexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelPath = new System.Windows.Forms.Label();
            this.labelResults = new System.Windows.Forms.Label();
            this.splitButtonOpen = new wyDay.Controls.SplitButton();
            this.contextMenuStripOpen.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxQuery
            // 
            this.textBoxQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxQuery.Location = new System.Drawing.Point(7, 19);
            this.textBoxQuery.Name = "textBoxQuery";
            this.textBoxQuery.Size = new System.Drawing.Size(537, 20);
            this.textBoxQuery.TabIndex = 1;
            this.textBoxQuery.TextChanged += new System.EventHandler(this.textBoxQuery_TextChanged);
            this.textBoxQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxQuery_KeyDown);
            // 
            // labelTerms
            // 
            this.labelTerms.AutoSize = true;
            this.labelTerms.Location = new System.Drawing.Point(7, 3);
            this.labelTerms.Name = "labelTerms";
            this.labelTerms.Size = new System.Drawing.Size(317, 13);
            this.labelTerms.TabIndex = 2;
            this.labelTerms.Text = "Search terms (? = matches any character, * = matches any string):";
            // 
            // contextMenuStripOpen
            // 
            this.contextMenuStripOpen.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openWithToolStripMenuItem,
            this.toolStripMenuItem1,
            this.reindexToolStripMenuItem});
            this.contextMenuStripOpen.Name = "contextMenuStrip1";
            this.contextMenuStripOpen.Size = new System.Drawing.Size(219, 98);
            this.contextMenuStripOpen.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripOpen_Opening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openWithToolStripMenuItem
            // 
            this.openWithToolStripMenuItem.Name = "openWithToolStripMenuItem";
            this.openWithToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.openWithToolStripMenuItem.Text = "Ope&n With... [Shift+Return]";
            this.openWithToolStripMenuItem.Click += new System.EventHandler(this.openWithToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(215, 6);
            // 
            // reindexToolStripMenuItem
            // 
            this.reindexToolStripMenuItem.Name = "reindexToolStripMenuItem";
            this.reindexToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.reindexToolStripMenuItem.Text = "Reindex";
            this.reindexToolStripMenuItem.Click += new System.EventHandler(this.reindexToolStripMenuItem_Click);
            // 
            // listViewResults
            // 
            this.listViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewResults.ContextMenuStrip = this.contextMenuStripOpen;
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewResults.HideSelection = false;
            this.listViewResults.LabelWrap = false;
            this.listViewResults.Location = new System.Drawing.Point(7, 62);
            this.listViewResults.MultiSelect = false;
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(536, 160);
            this.listViewResults.TabIndex = 2;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = System.Windows.Forms.View.Details;
            this.listViewResults.VirtualMode = true;
            this.listViewResults.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewResults_RetrieveVirtualItem);
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            this.listViewResults.DoubleClick += new System.EventHandler(this.listViewResults_DoubleClick);
            this.listViewResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewResults_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 253;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Project";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Path";
            this.columnHeader3.Width = 500;
            // 
            // labelPath
            // 
            this.labelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPath.BackColor = System.Drawing.SystemColors.Control;
            this.labelPath.Location = new System.Drawing.Point(7, 233);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(452, 18);
            this.labelPath.TabIndex = 4;
            this.labelPath.Text = "C:\\file.name";
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Location = new System.Drawing.Point(7, 46);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(45, 13);
            this.labelResults.TabIndex = 8;
            this.labelResults.Text = "Results:";
            // 
            // splitButtonOpen
            // 
            this.splitButtonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.splitButtonOpen.AutoSize = true;
            this.splitButtonOpen.ContextMenuStrip = this.contextMenuStripOpen;
            this.splitButtonOpen.Location = new System.Drawing.Point(465, 228);
            this.splitButtonOpen.Name = "splitButtonOpen";
            this.splitButtonOpen.Size = new System.Drawing.Size(78, 23);
            this.splitButtonOpen.SplitMenuStrip = this.contextMenuStripOpen;
            this.splitButtonOpen.TabIndex = 6;
            this.splitButtonOpen.Text = "&Open";
            this.splitButtonOpen.UseVisualStyleBackColor = true;
            this.splitButtonOpen.Click += new System.EventHandler(this.splitButtonOpen_Click);
            // 
            // MyControl
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.labelTerms);
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.listViewResults);
            this.Controls.Add(this.textBoxQuery);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.splitButtonOpen);
            this.MinimumSize = new System.Drawing.Size(523, 194);
            this.Name = "MyControl";
            this.Size = new System.Drawing.Size(551, 259);
            this.contextMenuStripOpen.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;

        private System.Windows.Forms.ContextMenuStrip contextMenuStripOpen;
        private System.Windows.Forms.Label labelTerms;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWithToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxQuery;
        private wyDay.Controls.SplitButton splitButtonOpen;
        private System.Windows.Forms.Label labelResults;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reindexToolStripMenuItem;
    }
}

