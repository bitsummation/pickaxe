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
using System.Net;
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
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; //TLS 1.2
            var code = new Code(new string[0]);
            code.Run();
        }
       
        [Test]
        public void BasicCodeGenTest()
        {
              var input = @"

create buffer specialty(id identity, url string, spec string)

insert into specialty
select
	'http://doctor.webmd.com' + pick 'a' take attribute 'href',
	pick 'a'
from download page 'http://doctor.webmd.com/find-a-doctor/specialties'
where nodes ='section.seo-lists div:nth-child(3) li'


--states
select
	'http://doctor.webmd.com' + pick 'a' take attribute 'href',
	d.url,
	spec
from download page (select url from specialty) d with (thread(4))
join specialty s on s.url = d.url and d.nodes = 'div.states li'

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
