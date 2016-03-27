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

using NUnit.Framework;
using Pickaxe.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class CodeGen
    {
        [Test]
        public void TestCodeRunner()
        {
            Code code = new Code(new string[0]);
            code.Run();
        }
       
        [Test]
        public void BasicCodeGenTest()
        {
              var input = @"

var downloadPage = download page 'http://vtss.brockreeve.com/Topic/Index/20'

select
		pick 'h3' take text, --title
		pick 'p:nth-child(3)' take text, --post
		pick 'p.author a' take text --user
from downloadPage
where nodes = 'div.topic'

select
	pick 'p:nth-child(2)' take text,
	pick 'p.author a' take text
from downloadPage
where nodes = 'div.reply'

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
