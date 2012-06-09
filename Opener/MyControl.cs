using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FileOpener.Opener
{
    public partial class MyControl : UserControl
    {
        private readonly ToolWindowPane pane;

        private SolutionIndex Index;

        private List<ItemInfo> found;
        private List<ItemInfo> Found
        {
            get { return found; }
            set
            {
                found = value;
                listViewResults.VirtualListSize = found.Count;
                if (listViewResults.VirtualListSize > 0) {
                    listViewResults.FocusedItem = listViewResults.Items[0];
                    listViewResults.SelectedIndices.Add(0);
                    listViewResults.EnsureVisible(0);
                }
                listViewResults.Invalidate();
                listViewResults_SelectedIndexChanged(null, null);
            }
        }

        public MyControl(ToolWindowPane pane)
        {
            this.pane = pane;

            InitializeComponent();

            labelPath.Text = "";
            RefreshResultsLabel();
            ValidateButtonState();
        }

        #region UI support code

        public void HideToolWindow()
        {
            ((IVsWindowFrame)pane.Frame).Hide();
        }

        public void InvokeCommand()
        {
            textBoxQuery.Focus();
            textBoxQuery.SelectAll();
            ValidateIndex(false);
            textBoxQuery_TextChanged(this, EventArgs.Empty);
        }


        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedIndices.Count > 0) {
                var idx = listViewResults.SelectedIndices[0];
                labelPath.Text = Found[idx].Path.Trim();
            } else {
                labelPath.Text = "";
            }
            ValidateButtonState();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void SendKeystroke(IntPtr hWnd, int keyCode)
        {
            //TODO: use Control.ReflectMessage
            var kc = (IntPtr)keyCode;
            SendMessage(hWnd, 0x0100, kc, (IntPtr)1);
            SendMessage(hWnd, 0x0101, kc, (IntPtr)(-0x3FFFFFFF));
        }

        private void textBoxQuery_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Down:
                case Keys.Up:
                case Keys.PageUp:
                case Keys.PageDown:
                    e.SuppressKeyPress = true;
                    SendKeystroke(listViewResults.Handle, (int)e.KeyCode);
                    break;
                case Keys.Return:
                    e.SuppressKeyPress = true;
                    OpenSolutionResource(e.Shift);
                    break;
            }
        }

        private void ValidateButtonState()
        {
            splitButtonOpen.Enabled = listViewResults.SelectedIndices.Count > 0;
        }

        private void reindexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Index = null;
            ValidateIndex(true);
            textBoxQuery_TextChanged(this, EventArgs.Empty);
        }

        private void contextMenuStripOpen_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var enabled = listViewResults.SelectedIndices.Count > 0;
            openToolStripMenuItem.Enabled = openWithToolStripMenuItem.Enabled = enabled;
        }

        #endregion

        #region open and open with

        private void splitButtonOpen_Click(object sender, EventArgs e)
        {
            OpenSolutionResource(false);
        }

        private void listViewResults_DoubleClick(object sender, EventArgs e)
        {
            OpenSolutionResource(false);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSolutionResource(false);
        }

        private void listViewResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) {
                OpenSolutionResource(e.Shift);
            }
        }

        private void openWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSolutionResource(true);
        }

        private void OpenSolutionResource(bool openWith)
        {
            if (listViewResults.SelectedIndices.Count != 1) {
                return;
            }
            var sr = Found[listViewResults.SelectedIndices[0]];

            if (!string.IsNullOrEmpty(sr.Path)) {
                //var dte = GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                //dte.ItemOperations.OpenFile(sr.Path);

                var service = (IVsCommandWindow)GetService(typeof(SVsCommandWindow));
                if (service != null) {
                    service.ExecuteCommand("File.OpenFile \"" + sr.Path + "\"" + (openWith ? " /editor" : ""));

                    HideToolWindow();
                }
            }
        }

        #endregion

        #region search and display

        private void ReindexWorker(IVsSolution sln)
        {
            var watch = Stopwatch.StartNew();
            Index.Reindex(sln, count => {
                if (count % 10 == 0 && watch.ElapsedMilliseconds > 60) {
                    watch.Restart();
                    while (watch.ElapsedMilliseconds < 40)
                        Application.DoEvents();
                    labelPath.Text = "Indexing: " + count + " items indexed...";
                    watch.Restart();
                }
            });
            labelPath.Text = Index.Count + " items indexed.";
        }

        private void ValidateIndex(bool force)
        {
            //Index = null;
            var sln = GetService(typeof(SVsSolution)) as IVsSolution;
            var slnName = SolutionIndex.GetSlnFileName(sln);

            if (slnName == null) {
                Index = null;
                return;
            }

            if (Index == null || !Index.SolutionName.Equals(slnName, StringComparison.OrdinalIgnoreCase)) {
                Index = new SolutionIndex(slnName);
                textBoxQuery.Text = "";
                Found = new List<ItemInfo>();

                var cacheLoaded = !force && Index.LoadFromCache();
                if (!cacheLoaded) {
                    ReindexWorker(sln);
                } else {
                    // Lazy reindex
                    var t = new System.Timers.Timer(3000) { AutoReset = false };
                    t.Elapsed += (s, e) => BeginInvoke(new Action(() => ReindexWorker(sln)));
                    t.Start();
                }
            }
            listViewResults.SmallImageList = Index.IconCache.ImageList;
        }

        private void textBoxQuery_TextChanged(object sender, EventArgs e)
        {
            var query = textBoxQuery.Text.Trim();
            if (query.Length > 0) {
                //var watch = Stopwatch.StartNew();
                if (Index == null || !Index.Ready) {
                    Found = new List<ItemInfo>();
                } else {
                    Found = Index.Query(query);
                }
                //pane.Caption = watch.ElapsedMilliseconds.ToString();
            }
            RefreshResultsLabel();
            ValidateButtonState();
        }

        private void RefreshResultsLabel()
        {
            if (Index == null || !Index.Ready) {
                labelResults.Text = Properties.Resources.IndexNotReady;
            } else if (textBoxQuery.Text.Trim().Length > 0) {
                var countMsg = string.Format(Properties.Resources.Found, listViewResults.VirtualListSize);
                labelResults.Text = Properties.Resources.LabelResults + countMsg;
            } else {
                labelResults.Text = Properties.Resources.LabelResults + Properties.Resources.EnterTerms;
            }
        }

        private void listViewResults_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var resource = Found[e.ItemIndex];
            e.Item = new ListViewItem(new[] { resource.Name, resource.ProjectName, resource.LogicalPath }) {
                Tag = resource,
                ImageIndex = resource.IconIndex,
            };
        }

        #endregion

        internal void OnSolutionClosing()
        {
            reindexToolStripMenuItem_Click(null, null);
        }
    }
}
