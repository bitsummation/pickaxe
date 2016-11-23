using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pickaxe.Runtime
{
    public class DownloadedNodes : IEnumerable<HtmlNode>
    {
        private IEnumerable<HtmlNode> _nodes { get; set; }

        public DownloadedNodes(IEnumerable<HtmlNode> nodes)
        {
            _nodes = nodes;
        }

        public IEnumerator<HtmlNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
