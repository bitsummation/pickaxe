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
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Pickaxe.Parser.Antlr
{
    internal class AntlrLexer : ScrapeLexer
    {
        public AntlrLexer(ICharStream input)
            : base(input)
        {
            Errors = new List<ParseException>();
        }

        public List<ParseException> Errors { get; private set; }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            string headerError = GetErrorHeader(e);
            string error = GetErrorMessage(e, tokenNames);

            Errors.Add(new ParseException(new LineInfo(e.Line, e.CharPositionInLine), error));

            base.DisplayRecognitionError(tokenNames, e);
        }
    }

}
