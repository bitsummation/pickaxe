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

using log4net.Appender;
using log4net.Core;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pickaxe.Studio
{
    public class TextBoxAppender : AppenderSkeleton
    {
        public string TextBoxName { get; set; }
        public string FormName { get; set; }

        private Control FindControl(string controlName, Control control)
        {
            if (control.Name == controlName)
                return control;

            foreach (Control child in control.Controls)
            {
                var t = FindControl(controlName, child);
                if (t != null)
                    return t;
            }

            return null;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var property = loggingEvent.LookupProperty(Config.LogKey) as string;
            var form = Application.OpenForms[FormName];
            TextBox textBox = null;
            TabPage tabControl = null;
            if(property != null)
            {
                tabControl = FindControl(property, form) as TabPage;
            }

            if (tabControl != null)
            {
                textBox = FindControl(TextBoxName, tabControl) as TextBox;
            }

            if (textBox == null)
                return;

            textBox.Invoke(new Action(() =>
            {
                if (textBox.Lines.Count() > 300)
                    textBox.Clear();

                textBox.AppendText(RenderLoggingEvent(loggingEvent));
                textBox.ScrollToCaret();
            }));          
        }
    }
}
