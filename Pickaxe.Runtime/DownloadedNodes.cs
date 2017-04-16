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
using System.Linq;
using System.Text;
using System.Collections;
using Pickaxe.Runtime.Dom;

namespace Pickaxe.Runtime
{
    public class DownloadedNodes : IEnumerable<HtmlElement>
    {
        private IEnumerable<HtmlElement> _nodes { get; set; }

        public static DownloadedNodes Empty
        {
            get
            {
                return new DownloadedNodes();
            }
        }

        public DownloadedNodes()
        {
            _nodes = new HtmlElement[0];
        }

        public static DownloadedNodes FromHtmlDoc(HtmlDoc doc)
        {
            return new DownloadedNodes(doc);
        }

        private DownloadedNodes(HtmlDoc doc) : this (new[] { doc.FirstElement})
        {
            if (doc.IsEmpty) //no nodes in root
                _nodes = new HtmlElement[0];
        }

        public DownloadedNodes(IEnumerable<HtmlElement> nodes)
        {
            _nodes = nodes;
        }

        public void Clear()
        {
            foreach (var node in _nodes)
                node.Clear();
        }

        public IEnumerator<HtmlElement> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
