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

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Pickaxe.Runtime.Dom;

namespace Pickaxe.Runtime
{
    public static class Extensions
    {
        public static TValue NullOperator<TValue>(this TValue a, TValue b) where TValue: class
        {
            return a ?? b;
        }

        public static void WriteFile(this byte[] bytes, string directory, string filename)
        {
            if (bytes.Length != 0)
            {
                string path = Path.Combine(directory, filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static HtmlElement Pick(this HtmlElement node, string selector)
        {
            return node.QuerySelector(selector);
        }

        public static string TakeAttribute(this HtmlElement node, string attribute)
        {
            string text = null;
            if (node != null && node.AttributeExists(attribute))
                text = WebUtility.HtmlDecode(node.TakeAttribute(attribute));

            return text;
        }

        public static string TakeText(this HtmlElement node)
        {
            string text = null;
            if (node != null)
                text = WebUtility.HtmlDecode(node.TakeText());

            return text;
        }

        public static string TakeHtml(this HtmlElement node)
        {
            string text = null;
            if (node != null)
                text = node.TakeHtml();

            return text;
        }

        public static string Match(this string text, string pattern)
        {
            string returnText = null;
            if (text != null)
            {
                var builder = new StringBuilder();
                foreach (Match match in Regex.Matches(text, pattern))
                    builder.Append(match.Value);

                if(builder.Length != 0)
                    returnText = builder.ToString();
            }

            return returnText;
        }

        public static string MatchReplace(this string text, string pattern, string replace)
        {
            string returnText = null;
            if (text != null)
                returnText =  Regex.Replace(text, pattern, replace);

            return returnText;
        }
    }
}
