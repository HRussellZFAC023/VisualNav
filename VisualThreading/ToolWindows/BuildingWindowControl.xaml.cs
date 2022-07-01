﻿using CefSharp;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Command = VisualThreading.Schema.Command;
using MessageBox = System.Windows.MessageBox;

namespace VisualThreading.ToolWindows
{
    public partial class BuildingWindowControl
    {
        private Command currentCommand;
        private string language;
        private readonly string _workspace;
        private readonly string _toolbox; 
        private readonly dynamic _schema;

        public BuildingWindowControl(Schema.Schema commands, string fileExt, string blockly, string toolbox, string workspace, string schema)
        {
            currentCommand = null;
            _toolbox = toolbox;
            _workspace = workspace;
            _schema = JsonConvert.DeserializeObject(schema);

            InitializeComponent();
            Focus();
            Browser.LoadHtml(blockly);


            Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
                return;

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/blockly_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/blocks_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/msg/js/en.js"));
                
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/colour.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/lists.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/logic.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/loops.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/math.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/procedures.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/text.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/generators/csharp/variables.js")); 

                await Browser.EvaluateScriptAsync("init", _toolbox, _workspace, false);
            }).FireAndForget();
        }


        private void combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            ComboBoxItem item = block_language.SelectedItem as ComboBoxItem;

            if (item != null)
            {
                language = (String)item.Content;
            }
        }


        private void ShowCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/blockly_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/blocks_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/msg/js/en.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/javascript_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/python_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/php_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/lua_compressed.js"));
                Browser.ExecuteScriptAsync(File.ReadAllText(@"blockly/dart_compressed.js"));

                var result = await Browser.EvaluateScriptAsync(
                    "showCode", language);

                MessageBox.Show((string)result.Result);
            }).FireAndForget();
        }

        public void SetCurrentCommand(Command c)
        {
            // Color: 
            // Parent: Logic
            // Preview: 
            // Text: controls_if
            // System.Diagnostics.Debug.WriteLine(c);

            currentCommand = c;
            var color = c.Color;
            var parent = c.Parent;
            var text = c.Text;

            var blocks = _schema["RadialMenu"][0]["commands"];
            var blockType = "";
            foreach (var block in blocks)
            {
                if (block["parent"] == parent && block["text"] == text)
                {
                    blockType = block["type"];
                }
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Browser.EvaluateScriptAsync("addNewBlockToArea", blockType, color);
            }).FireAndForget();

        }
    }
}