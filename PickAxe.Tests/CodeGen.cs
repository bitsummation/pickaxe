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


create buffer levels(month string, year string, level float)

insert into levels
select
	pick 'td:nth-child(1) p.bold' take text match '(\d+)-(\w+)' replace '$2',
	pick 'td:nth-child(1) p.bold' take text match '(\d+)-(\w+)' replace '$1',
	pick 'td:nth-child(2) p' take text match '\d{3}\.\d{2}'
from download page (
	select
		'http://www.golaketravis.com/waterlevel/' + pick '' take attribute 'href'
	from download page 'http://www.golaketravis.com/waterlevel/'
	where nodes = 'table[width=""600""] td[style=""background-color: #62ABCC;""] p.white a'
	) with (thread(20))
where nodes = 'table[width=""600""] tr'

select *
from levels
where year = '2016'



/*
 select 
    pick '.one' take text
    from download page 'http://brockreeve.com'
    where nodes = 'ol .temp' and nodes = '.blah'
*/

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
