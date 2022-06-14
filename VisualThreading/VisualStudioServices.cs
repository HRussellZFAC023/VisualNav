using EnvDTE;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VisualThreading
{
    public static class VisualStudioServices
    {
        public static DTE DTE
        {
            get;
            set;
        }

        public static IServiceProvider OLEServiceProvider
        {
            get;
            set;
        }

        public static System.IServiceProvider ServiceProvider
        {
            get;
            set;
        }
    }
}