using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            dte.ExecuteCommand("Edit.FormatSelection");
            VS.MessageBox.Show("WOW");
        }
    }
}
