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
            //var code = new Code(new string[0]);
            //code.Run();
        }
       
        [Test]
        public void BasicCodeGenTest()
        {
              var input = @"

       
 select
    case when pick 'li:first-child' take text match '[\d\.]+' < 9000 then 2 end
    from download page 'http://mock.com'
    where nodes = '#match-tests'


";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
