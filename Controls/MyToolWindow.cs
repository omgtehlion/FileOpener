using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FileOpener.Opener;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FileOpener.Controls
{
    [Guid(GuidList.guidMyToolWindowString)]
    public class MyToolWindow : ToolWindowPane
    {
        private readonly MyControl control;

        public MyToolWindow()
            : base(null)
        {
            Caption = Properties.Resources.WindowTitle;
            BitmapResourceID = 0x12d;
            BitmapIndex = 2;
            control = new MyControl(this);
        }

        public override IWin32Window Window
        {
            get { return control; }
        }

        private void InvokeOnControl(Action act)
        {
            if (!control.IsHandleCreated) {
                control.Handle.GetHashCode();
            }
            control.BeginInvoke(act);
        }

        internal void InvokeCommand()
        {
            var frame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(frame.Show());

            InvokeOnControl(() => {
                Application.DoEvents();
                control.InvokeCommand();
            });
        }

        protected override bool PreProcessMessage(ref Message m)
        {
            if (m.Msg == 0x0100 /*WM_KEYDOWN*/) {
                if ((Keys)m.WParam == Keys.Escape) {
                    //((IVsWindowFrame)Frame).Hide();
                    control.HideToolWindow();
                }
            }
            return base.PreProcessMessage(ref m);
        }

        // we have to save reference to SolutionEvents or otherwise they will be unsubscribed
        EnvDTE.SolutionEvents SolutionEvents;

        public override void OnToolWindowCreated()
        {
            var dte = GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            SolutionEvents = dte.Events.SolutionEvents;
            SolutionEvents.BeforeClosing += SolutionEvents_BeforeClosing;

            base.OnToolWindowCreated();
        }

        void SolutionEvents_BeforeClosing()
        {
            InvokeOnControl(control.OnSolutionClosing);
        }
    }
}
