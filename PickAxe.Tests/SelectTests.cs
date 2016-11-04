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
</div>

";
            var httpRequest = new Mock<IHttpRequest>();
            httpRequest.Setup(x => x.Download()).Returns(System.Text.Encoding.UTF8.GetBytes(html));

            var requestFactory = new Mock<IHttpRequestFactory>();
            requestFactory.Setup(x => x.Create(It.IsAny<IHttpWire>())).Returns(httpRequest.Object);
            _requestFactory = requestFactory.Object;
        }

        [Test]
        public void Select_StringConcat()
        {
             var code = @"
        
 select
    pick '#match-tests li:nth-child(2)' take text + 'concat'
    from download page 'http://mock.com'

";

             var runable = TestHelper.Compile(code, _requestFactory);

             int called = 0;
             runable.Select += (table) =>
             {
                 called++;
                 Assert.IsTrue(table.Columns().Length == 1);
                 Assert.IsTrue(table.Columns()[0] == "(No column name)");
                 Assert.IsTrue(table.RowCount == 1);
                 Assert.IsTrue(table[0][0].ToString() == "6concat");
             };

             runable.Run();
             Assert.True(called == 1);

        }

        [Test]
        public void Select_TestReplace()
        {
            var code = @"
        
 select
    pick '.address' take html match '(.*)<br>(.*)' replace '$1'
    from download page 'http://mock.com'

";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == ".address");
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "4332 Forest Hill Blvd");
            };

            runable.Run();
            Assert.True(called == 1);
        }

        [Test]
        public void Select_TestMatch()
        {
            var code = @"
        
 select
    pick 'div.dollar' take text match '[\d\.]+'
    from download page 'http://mock.com'

";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "div.dollar");
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(table[0][0].ToString() == "6566.00");
            };

            runable.Run();
            Assert.True(called == 1);

        }

        [Test]
        public void Select_TestPickTakeAttribute()
        {
            var code = @"
        
 select
    pick 'a' take attribute 'href'
    from download page 'http://mock.com'
 
";

            var runable = TestHelper.Compile(code, _requestFactory);
           
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
        public void Select_TestPickTakeText()
        {
            var code = @"
        
 select
    pick '.center' take text
    from download page 'http://mock.com'
 
";

            var runable = TestHelper.Compile(code, _requestFactory);

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

        [Test]
        public void Select_TestCaseBoolean()
        {
            var code = @"
  
create buffer temp(id int)
      
insert into temp
select 3

insert into temp
select 2

insert into temp
select 5

 select
    id,
    case when id < 5 and id > 2 then 'hit' end
    from temp
 
";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.RowCount == 3);
                Assert.IsTrue(table[0][0].ToString() == "3");
                Assert.IsTrue(table[0][1].ToString() == "hit");
            };

            runable.Run();
            Assert.True(called == 1);
        }

        [Test]
        public void Select_TestCaseBooleanPick()
        {
            var code = @"
        
 select
    case when pick 'li:first-child' take text match '[\d\.]+' < 9000 then 2 end
    from download page 'http://mock.com'
    where nodes = '#match-tests'
 
";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.RowCount == 2);
                Assert.IsTrue(table[0][0].ToString() == "2");
            };

            runable.Run();
            Assert.True(called == 1);
        }
    }
}
