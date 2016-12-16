
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
