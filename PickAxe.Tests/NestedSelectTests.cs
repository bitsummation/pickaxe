using Moq;
using NUnit.Framework;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class NestedSelectTests
    {
         private IHttpRequestFactory _requestFactory;

        public NestedSelectTests()
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
        public void Nested_Select_Simple()
        {
            var code = @"
  
select t.p
from (
	select
		pick 'li:nth-last-of-type(1)' as p
	from download page 'http://mock.com'
	where nodes = '#match-tests' ) t
";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "p");
                Assert.IsTrue(table.RowCount == 2);
                Assert.IsTrue(table[0][0].ToString() == "6,566,888");
                Assert.IsTrue(table[1][0].ToString() == "2,566,888");
            };

            runable.Run();
            Assert.True(called == 1);
        }

        [Test]
        public void Nested_Select_Join()
        {
            var code = @"
    select t.p, p.k
    from (
	    select
            1 as k,
		    pick 'li:nth-of-type(2)' as p
	    from download page 'http://mock.com'
	    where nodes = '#match-tests'
    ) t
    join ( 
        select
            1 as k 
) p on p.k = t.k
";

            var runable = TestHelper.Compile(code, _requestFactory);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "p");
                Assert.IsTrue(table.RowCount == 2);
                Assert.IsTrue(table[0][0].ToString() == "6,566,888");
                Assert.IsTrue(table[1][0].ToString() == "2,566,888");
            };

            runable.Run();
            Assert.True(called == 1);
           
        }

    }
}
