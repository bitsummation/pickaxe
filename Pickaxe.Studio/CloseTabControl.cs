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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Pickaxe.Studio
{
    public class CloseTabControl : TabControl
    {
        public const string Filler = " xx";

        private IDictionary<int, Rectangle> _closeBounds;

        public CloseTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            _closeBounds = new Dictionary<int, Rectangle>();
        }

        public event Action<int> Close;

        private void OnClose(int index)
        {
            if (Close != null)
                Close(index);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var bounds = e.Bounds;
            if (e.State != DrawItemState.Selected)
                bounds = new Rectangle(e.Bounds.X - 4, e.Bounds.Y - 2, e.Bounds.Width, e.Bounds.Height);

            var fullCaption = TabPages[e.Index].Text;
            var caption = fullCaption.Replace(Filler, "");
            var captionSize = Size.Round(e.Graphics.MeasureString(caption, e.Font));
            var closeSize = Size.Round(e.Graphics.MeasureString("x", e.Font));

            e.Graphics.DrawString(caption, e.Font, Brushes.Black, bounds.X + 3, bounds.Y + 5);

            var closeStart = bounds.X + captionSize.Width + 4;
            var closeBounds = new Rectangle(closeStart, bounds.Y + 5, closeSize.Width, closeSize.Height);
            e.Graphics.DrawString("x", e.Font, Brushes.Black, closeBounds.X, closeBounds.Y);
            _closeBounds[e.Index] = closeBounds;
           
            e.DrawFocusRectangle();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            using (Graphics g = CreateGraphics())
            using (Pen p = new Pen(SystemColors.Control, 1))
            {
                for (int x = 0; x < TabPages.Count; x++)
                {
                    if (_closeBounds[x].Contains(e.Location))
                        g.DrawRectangle(Pens.DarkGray, _closeBounds[x]);
                    else
                        g.DrawRectangle(p, _closeBounds[x]);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            for (int x = 0; x < TabPages.Count; x++)
            {               
                if (_closeBounds[x].Contains(e.Location))
                {
                    OnClose(x);
                    break;
                }
            }
        }
    }
}
