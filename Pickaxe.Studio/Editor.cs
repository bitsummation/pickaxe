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
using System.Windows.Forms;
using System.Reflection;
using log4net;
using System.Drawing;

namespace Pickaxe.Studio
{
    public partial class Editor : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Editor()
        {
            InitializeComponent();
            editors.Close += OnClose;
        }

        private TabPage CreateEditTab()
        {
            var tabName = "t_" + Guid.NewGuid().ToString("N");
            var tabPage = new TabPage();
            tabPage.Name = tabName;

            var editor = new EditControl(tabName);
            editor.Dock = DockStyle.Fill;

            tabPage.Tag = editor;
            tabPage.Controls.Add(editor);
            return tabPage;
        }

        private void OnSaveAs(object sender, EventArgs e)
        {
            if (ActiveEditControl != null)
            {
                var name = ActiveEditControl.SaveAs();
                if (name != null)
                    editors.SelectedTab.Text = name + CloseTabControl.Filler;
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (ActiveEditControl != null)
            {
                var name = ActiveEditControl.Save();
                if (name != null)
                    editors.SelectedTab.Text = name + CloseTabControl.Filler;
            }
        }

        private void OnNew(object sender, EventArgs e)
        {
           var tabPage = CreateEditTab();
           var editControl = GetEditControlFromTab(tabPage);
           string name = editControl.New();
           tabPage.Text = name + CloseTabControl.Filler;

           editControl.IsRunningChanged += IsRunningChanged;
           editControl.IsDirtyChanged += OnIsDirtyChanged;

           editors.TabPages.Add(tabPage);
           editors.SelectTab(tabPage);
        }

        private void OnClose(int index)
        {
            var editControl = GetEditControlFromTab(editors.TabPages[index]);
            var message = string.Empty;
            if (editControl.IsRunning)
                message = "Code is running. Are you sure you want to close?";
            if(editControl.IsDirty)
                message = "Code is unsaved. Are you sure you want to close?";

            DialogResult result = DialogResult.OK;
            if (!String.IsNullOrEmpty(message))
                result = MessageBox.Show(message, "Confirm Close", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                editControl.Stop();
                editControl.IsRunningChanged -= IsRunningChanged;
                editControl.IsDirtyChanged -= OnIsDirtyChanged;
                editors.TabPages.RemoveAt(index);
            }
        }

        private EditControl ActiveEditControl
        {
            get { return editors.SelectedTab != null ? editors.SelectedTab.Tag as EditControl : null; }
        }

        private EditControl GetEditControlFromTab(TabPage tabPage)
        {
            return tabPage.Tag as EditControl;
        }
        
        private void OnOpen(object sender, EventArgs e)
        {
            var tabPage = CreateEditTab();
            var editControl = GetEditControlFromTab(tabPage);
            string name = editControl.Open();
            if (name != null)
            {
                editControl.IsRunningChanged += IsRunningChanged;
                editControl.IsDirtyChanged += OnIsDirtyChanged;
                tabPage.Text = name + CloseTabControl.Filler;

                editors.TabPages.Add(tabPage);
                editors.SelectTab(tabPage);
            }
        }

        private void MarkDirty(TabPage page)
        {
            page.Text = page.Text + "*";
            page.Text = page.Text.Replace("**", "*");
        }

        private void MarkClean(TabPage page)
        {
            page.Text = page.Text.Replace("*", "");
        }

        private void OnIsDirtyChanged(EditControl control)
        {
            foreach(TabPage tab in editors.TabPages)
            {
                var editControl = GetEditControlFromTab(tab);
                if(control == editControl)
                {
                    if (control.IsDirty)
                        MarkDirty(tab);
                    else
                        MarkClean(tab);
                    
                    break;
                }
            }
        }

        private void OnRun(object sender, EventArgs e)
        {
            if(ActiveEditControl != null)
                ActiveEditControl.Run();
        }

        private void OnStop(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            if(ActiveEditControl != null)
                ActiveEditControl.Stop();
        }

        private void SelectedTabChanged(object sender, EventArgs e)
        {
            SetRun();
        }

        private void IsRunningChanged(EditControl control)
        {
            SetRun();
        }

        private void SetRun()
        {
            if (ActiveEditControl != null)
            {
                runButton.Enabled = !ActiveEditControl.IsRunning;
                stopButton.Enabled = ActiveEditControl.IsRunning;
            }
            else
            {
                runButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }
    }
}
