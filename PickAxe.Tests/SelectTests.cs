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
        private IHttpRequestFactory _requestFactory;

        public SelectTests()
        {
            var html = @"

<div>
    <span class=""center"">here</span>
    <a href=""http://google.com"">link</a>
    <div id=""match-tests"">
        <li>6,566.00</li>
        <li>6.00</li>
        <li>8,975.00</li>
        <li>6,566,888.00</li>
    </div>
</div>

";
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.Download()).Returns(System.Text.Encoding.UTF8.GetBytes(html));

            var requestFactory = new Mock<IHttpRequestFactory>();
            requestFactory.Setup(x => x.Create("http://mock.com")).Returns(httpRequest.Object);
            _requestFactory = requestFactory.Object;
        }

        private Runable Compile(string code)
        {
            var compiler = new Compiler(code);
            var assembly = compiler.ToAssembly();
            Assert.IsTrue(compiler.Errors.Count == 0);
            var runable = new Runable(assembly);
            runable.SetRequestFactory(_requestFactory);
            return runable;
        }


        [Test]
        public void PickTakeAttribute()
        {
            var code = @"
        
 select
    pick 'a' take attribute 'href'
    from download page 'http://mock.com'
 
";

            var runable = Compile(code);
           
            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "http://google.com");
            };

            runable.Run();
            Assert.True(called == 1);

        }

        [Test]
        public void PickTakeText()
        {
            var code = @"
        
 select
    pick '.center' take text
    from download page 'http://mock.com'
 
";

            var runable = Compile(code);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "here");
            };

            runable.Run();
            Assert.True(called == 1);
        }
    }
}
