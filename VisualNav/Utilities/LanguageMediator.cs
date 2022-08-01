using Microsoft.VisualStudio.Text;
using System.IO;

namespace VisualNav.Utilities
{
    /// <summary>
    /// This class provides the GetCurrentActiveFileExtension() method, can be used to find out the current language;
    ///
    /// @see https://refactoring.guru/design-patterns/mediator
    /// </summary>
    internal static class LanguageMediator
    {
        private static string _currentLanguage = ""; // file ext of current language

        private static void SelectionEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentLanguage = e.To == null ? "" : Path.GetExtension(e.To.Name);
        }

        public static async Task InitializeAsync()
        {
            VS.Events.SelectionEvents.SelectionChanged += SelectionEventsOnSelectionChanged;
            var buffer = await VS.Documents.GetActiveDocumentViewAsync();
            _currentLanguage = buffer?.TextBuffer == null ? "" : Path.GetExtension(buffer.TextBuffer.GetFileName());
        }

        // call GetCurrentActiveFileExtension() for the current language
        public static string GetCurrentActiveFileExtension()
        {
            return _currentLanguage;
        }
    }
}