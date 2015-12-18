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
        public void BasicCodeGenTest()
        {
              var input = @"

create buffer postpages (startPage int, endPage int)

insert into postpages
select
	pick 'li.current a' take text,
	pick 'li:nth-child(7) a' take text
from download page 'http://vtss.brockreeve.com/?t=All'
where nodes = 'ol.page-nav'

create buffer pageurls (url string)

each(row in postpages){
	
	insert into pageurls
	select
		'http://vtss.brockreeve.com/Home/Index/' + value + '?t=All'
	from expand (row.startPage to row.endPage)
}

create buffer detailurls (url string, original string)

insert into detailurls
select
	'http://vtss.brockreeve.com' + pick 'h3 a' take attribute 'href',
    url
from download page (select url from pageurls)
where nodes = 'div.topic'

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
