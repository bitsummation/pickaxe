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
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    [TestFixture]
    public class WhereTests
    {
        private IHttpRequestFactory _requestFactory;

        public WhereTests()
        {
            var html = @"

<div>
    <span class=""center"">here</span>
    <a href=""http://google.com"">link</a>
    <div class=""dollar"">$6,566.00</div>
    <span class=""address"">4332 Forest Hill Blvd<br>West Palm Beach, FL 33406</span>
    <div id=""match-tests"">
        <li>6,566</li>
        <li>6</li>
        <li>8,975</li>
        <li>6,566,888</li>
    </div>
    <div id=""match-tests"">
        <li>10,566</li>
        <li>3</li>
        <li>1,975</li>
        <li>2,566,888</li>
    </div>
    <div class=""counts"">
        <div class=""address"">
            <div id=""first-name"">1</div>
            <div id=""last-name"">2</div>
        </div>
        <div class=""address"">
            <div id=""first-name"">3</div>
            <div id=""last-name"">4</div>
        </div>
        <div class=""address"">
            <div id=""first-name"">5</div>
            <div id=""last-name"">6</div>
        </div>
    <div>
</div>

";
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.Download()).Returns(System.Text.Encoding.UTF8.GetBytes(html));

            var requestFactory = new Mock<IHttpRequestFactory>();
            requestFactory.Setup(x => x.Create("http://mock.com")).Returns(httpRequest.Object);
            _requestFactory = requestFactory.Object;
        }

        [Test]
        public void Where_Boolean()
        {
            var code = @"
        
create buffer temp(first int, second int)

insert into temp
 select
    pick 'li:first-child' take text match '[\d\.]+',
    pick 'li:nth-child(2)' take text
    from download page 'http://mock.com'
    where nodes = '#match-tests'

select *
from temp
where first < 10000 and second = 6
 
";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "6566");
                Assert.IsTrue(table[0][1].ToString() == "6");
            };

            runable.Run();
            Assert.True(called == 1);

        }

        [Test]
        public void Where_Nodes()
        {

            var code = @"      
 select
    pick 'div#first-name' take text,
    pick 'div#last-name' take text
    from download page 'http://mock.com'
    where nodes = 'div.counts .address'

";
            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.RowCount == 3);
                Assert.IsTrue(table[0][0].ToString() == "1");
                Assert.IsTrue(table[0][1].ToString() == "2");
                Assert.IsTrue(table[1][0].ToString() == "3");
                Assert.IsTrue(table[1][1].ToString() == "4");
                Assert.IsTrue(table[2][0].ToString() == "5");
                Assert.IsTrue(table[2][1].ToString() == "6");
            };

            runable.Run();
            Assert.True(called == 1);
        }
    }
}
