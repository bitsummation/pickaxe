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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pickaxe.Studio
{
    public class HighlightListView : ListView
    {
        private IList<ListViewItem.ListViewSubItem> _selectedItems;

        public HighlightListView()
        {
            _selectedItems = new List<ListViewItem.ListViewSubItem>();
            DoubleBuffered = true;
        }

        private void ClearSelectedItems()
        {
            foreach (var item in _selectedItems)
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }

            _selectedItems.Clear();
        }

        private void HighlightSubItem(ListViewItem.ListViewSubItem item)
        {
            item.BackColor = SystemColors.MenuHighlight;
            item.ForeColor = Color.White;

            _selectedItems.Add(item);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            BeginUpdate();

            ClearSelectedItems();

            var info = HitTest(e.X, e.Y);
            if (info.SubItem != null)
            {
                info.Item.UseItemStyleForSubItems = false;
                int index = info.Item.SubItems.IndexOf(info.SubItem);

                if (index == 0) //first item. Highlight all rows
                {
                    foreach (ListViewItem.ListViewSubItem item in info.Item.SubItems)
                        HighlightSubItem(item);
                }
                else
                {
                    HighlightSubItem(info.SubItem);
                }
            }

            EndUpdate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C && _selectedItems.Count > 0)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (var item in _selectedItems)
                builder.AppendLine(item.Text);

            Clipboard.SetText(builder.ToString());
        }
    }
}
