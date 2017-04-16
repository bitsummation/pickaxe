/* Copyright 2015 Brock Reeve
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using log4net;
using Pickaxe.Runtime;
using Pickaxe.Emit;

namespace Pickaxe.Studio
{
    public partial class EditControl : UserControl
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ScrapeFile _loadedFile;
        private Runable _runable;
        private HighlightColor _defaultHighlight;
        private HighlightColor _executionHighlight;
        private string _logValue;
        private bool _isRunning;
        private bool _isDirty;

        public event Action<EditControl> IsRunningChanged;
        public event Action<EditControl> IsDirtyChanged;

        public EditControl(string logValue)
        {
            InitializeComponent();
            InitHighlighting();
            _isRunning = false;
            _isDirty = false;
            _logValue = logValue;
        }

        private void DoInvoke(Action action)
        {
            if (IsHandleCreated)
            {
                var form = this.FindForm();
                if (form != null)
                {
                    if (form.InvokeRequired)
                        form.BeginInvoke(action);
                    else
                        action();
                }
            }
        }

        private void OnDocumentChanged(object sender, DocumentEventArgs e) //mark dirty
        {
            IsDirty = true;
        }

        private void OnIsRunningChanged()
        {
            if (IsRunningChanged != null)
                IsRunningChanged(this);
        }

        private void OnIsDirtyChanged()
        {
            if (IsDirtyChanged != null)
                IsDirtyChanged(this);
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                bool oldValue = _isDirty;
                _isDirty = value;

                if (oldValue != _isDirty)
                    OnIsDirtyChanged();
            }
        }

        public bool IsRunning {
            get { return _isRunning; }
            private set
            {
                bool oldValue = _isRunning;
                _isRunning = value;

                if (oldValue != _isRunning)
                    OnIsRunningChanged();
            }
        }

        private void InitHighlighting()
        {
            string dir = Path.Combine(Application.StartupPath, @"Highlighting\");
            if (Directory.Exists(dir))
            {
                var fsmProvider = new FileSyntaxModeProvider(dir);
                HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider);
                textEditorControl1.SetHighlighting("Scrape");
            }

            _executionHighlight = new HighlightColor(Color.Black, Color.Yellow, false, false);
            var highlightingStrategy = textEditorControl1.Document.HighlightingStrategy as DefaultHighlightingStrategy;
            _defaultHighlight = highlightingStrategy.GetColorFor("Selection");
        }

        private void AutoSave()
        {
            if (_loadedFile != null)
            {
                using (var writer = new StreamWriter(_loadedFile.File, false))
                {
                    writer.Write(textEditorControl1.Document.TextContent);
                }
                IsDirty = false;
            }
        }

        public string Open()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = "(*.s)|*.s";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _loadedFile = new ScrapeFile() { File = openFileDialog.FileName };
                    using (var stream = new FileStream(_loadedFile.File, FileMode.Open, FileAccess.Read))
                    {
                        textEditorControl1.LoadFile(_loadedFile.File, stream, true, true);
                    }

                    textEditorControl1.ActiveTextAreaControl.Document.DocumentChanged += OnDocumentChanged;
                    return Path.GetFileName(_loadedFile.File);
                }
            }

            return null; //they canceled.
        }

        public string New()
        {
            textEditorControl1.ActiveTextAreaControl.Document.DocumentChanged += OnDocumentChanged;
            return "untitled";
        }

        public string SaveAs()
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "(*.s)|*.s";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(saveDialog.FileName))
                    {
                        using (var stream = saveDialog.OpenFile())
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.Write(textEditorControl1.Document.TextContent);
                        }
                    }

                    _loadedFile = new ScrapeFile() { File = saveDialog.FileName };
                    IsDirty = false;
                    return Path.GetFileName(_loadedFile.File);
                }
            }

            return null; //canceled
        }

        public string Save()
        {
            if (_loadedFile == null) //save as.
            {
                return SaveAs();
            }
            else
            {
                AutoSave();
                return Path.GetFileName(_loadedFile.File);
            }
        }

        public void Run()
        {
            AutoSave();
            OnPreCompile();

            var source = textEditorControl1.ActiveTextAreaControl.SelectionManager.SelectedText;
            if (string.IsNullOrEmpty(source))
                source = textEditorControl1.Document.TextContent;

            ThreadPool.QueueUserWorkItem((state) => Compile(source), source); //compile on seperate thread so we don't block UI,
        }

        public void StopApplication()
        {
            if (_runable != null)
                _runable.Stop(null);
        }

        public void Stop()
        {
            if (_runable != null)
                _runable.Stop(() => ProgramFinished());

            statusLabel.Text = "Stopping...";
        }

        private void ListErrors(string[] errors)
        {
            DoInvoke(new Action(() =>
            {
                foreach (var error in errors)
                    messagesTextBox.AppendText(error + Environment.NewLine);
                resultsTabs.SelectedIndex = 1;
            }));
        }

        private void Compile(string source)
        {
            ThreadContext.Properties[Config.LogKey] = _logValue;
            Config.LogValue = _logValue;

            DoInvoke(new Action(() =>
            {
                messagesTextBox.Clear();
                statusLabel.Text = "Running...";

                //var highlightingStrategy = textEditorControl1.Document.HighlightingStrategy as DefaultHighlightingStrategy;
                //highlightingStrategy.SetColorFor("Selection", _executionHighlight);
            }));

            var compiler = new Compiler(source);
            var generatedAssembly = compiler.ToAssembly();

            if (compiler.Errors.Any())
                ListErrors(compiler.Errors.Select(x => x.Message).ToArray());

            if (!compiler.Errors.Any())
            {
                _runable = new Runable(generatedAssembly);
                _runable.Select += OnSelectResults;
                _runable.Progress += OnProgress;
                _runable.Highlight += OnHighlight;

                try
                {
                    _runable.Run();
                }
                catch (ThreadAbortException)
                {
                    Log.Info("Program aborted");
                }
                catch (Exception e)
                {
                    Log.Fatal("Unexpected Exception", e);
                }
            }

            ProgramFinished();
        }

        private void ProgramFinished()
        {
            DoInvoke(new Action(() =>
            {
                statusLabel.Text = "Ready.";
                progressBar.Visible = false;
                progressText.Visible = false;
                IsRunning = false;
                //var highlightingStrategy = textEditorControl1.Document.HighlightingStrategy as DefaultHighlightingStrategy;
                //highlightingStrategy.SetColorFor("Selection", _defaultHighlight);
            }));   
        }

        private void OnHighlight(int line)
        {
            DoInvoke(new Action(() =>
            {
                var start = new TextLocation(0, line - 1);
                var end = new TextLocation(1000, line - 1);

                textEditorControl1.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);
                textEditorControl1.ActiveTextAreaControl.Caret.Position = end;
                textEditorControl1.ActiveTextAreaControl.ScrollToCaret();
            }));
        }

        private void OnProgress(ProgressArgs e)
        {
            DoInvoke(new Action(() =>
            {
                progressText.Text = string.Format("{0}/{1}", e.CompletedOperations, e.TotalOperations);
                int value = 0;
                if (e.TotalOperations > 0)
                    value = (int)((e.CompletedOperations / (float)e.TotalOperations) * 100);

                progressBar.Value = value;
            }));
        }

        private void OnSelectResults(RuntimeTable<ResultRow> result)
        {
            StringFormat format = StringFormat.GenericTypographic;
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            var columns = new List<ColumnHeader>();
            var items = new List<ListViewItem>();

            columns.Add(new ColumnHeader() { Text = "" });

            foreach (var column in result.Columns()) //headers
                columns.Add(new ColumnHeader() { Text = column });

            for (int x = 0; x < result.RowCount; x++)
            {
                var rowItem = new ListViewItem((x + 1).ToString());
                rowItem.UseItemStyleForSubItems = false;
                foreach (var columnValue in result[x])
                {
                    var subitem = new ListViewItem.ListViewSubItem();
                    if (columnValue == null)
                    {
                        subitem.Text = "NULL";
                        subitem.BackColor = Color.FromArgb(255,255,225);
                    }
                    else
                    {
                        subitem.Text = columnValue.ToString();
                    }

                    rowItem.SubItems.Add(subitem);
                }

                items.Add(rowItem);
            }

            DoInvoke(new Action(() =>
            {
                resultsListView.BeginUpdate();
                resultsListView.Columns.Clear();
                resultsListView.Items.Clear();

                resultsTabs.SelectedIndex = 0;
                resultsListView.GridLines = true;
                resultsListView.Columns.AddRange(columns.ToArray());
                resultsListView.Items.AddRange(items.ToArray());
                resultsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                resultsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsListView.EndUpdate();
            }));
        }

        private void OnPreCompile()
        {
            resultsListView.Columns.Clear();
            resultsListView.Items.Clear();
            resultsListView.GridLines = false;
            progressBar.Value = 0;
            progressBar.Visible = true;
            progressText.Text = "0/0";
            progressText.Visible = true;
            resultsTabs.SelectedIndex = 1;
            IsRunning = true;
        }
    }

}
