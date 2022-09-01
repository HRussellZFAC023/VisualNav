using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE;

namespace VisualNav.Utilities
{
    internal static class Formatter
    {
        public static DTE dte;

        public static void Format()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var currentDoc = dte.ActiveDocument;
            currentDoc.Activate();
            if (dte.ActiveWindow.Kind == "Document")
            {
                dte.ExecuteCommand("Edit.FormatDocument");
                VS.MessageBox.Show("WOW");
            }


        }
    }
}
