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

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Runtime
{
    public class LazyDownloadPage : DownloadPage
    {
        private ThreadedDownloadPageTable _parent;
        private DownloadPage _inner;

        public LazyDownloadPage(ThreadedDownloadPageTable parent)
        {
            _parent = parent;
        }

        private DownloadPage Inner
        {
            get
            {
                _parent.Process();

                if (_inner == null)
                    _inner = _parent.GetResult();

                return _inner;
            }
        }

        public void ClearNodes()
        {
            Inner.nodes = null;
        }

        public override string url
        {
            get
            {
                return Inner.url;
            }
        }

        public override IEnumerable<HtmlNode> nodes
        {
            get
            {
                return Inner.nodes;
            }
        }

        public override DateTime date
        {
            get
            {
                return Inner.date;
            }
        }

        public override int size
        {
            get
            {
                return Inner.size;
            }
        }

    }
}
