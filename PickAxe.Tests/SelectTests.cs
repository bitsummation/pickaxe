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

using Moq;
using NUnit.Framework;
using Pickaxe.Emit;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    [TestFixture]
    public class SelectTests
    {

        [Test]
        public void PickTakeText()
        {
            var html = @"

<div>
    <span class=""center"">here</span>
</div>

";

            var code = @"
        
 select
    pick '.center' take text
    from download page 'http://mock.com'
 
";

            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.Download()).Returns(System.Text.Encoding.UTF8.GetBytes(html));

            var requestFactory = new Mock<IHttpRequestFactory>();
            requestFactory.Setup(x => x.Create("http://mock.com")).Returns(httpRequest.Object);

            var compiler = new Compiler(code);
            var assembly = compiler.ToAssembly();
            Assert.IsTrue(compiler.Errors.Count == 0);
            var runable = new Runable(assembly);
            runable.SetRequestFactory(requestFactory.Object);

            bool raised = false;
            runable.Select += (table) =>
            {
                raised = true;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "here");
            };

            runable.Run();
            Assert.True(raised);
        }
    }
}
