using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class WhileTests
    {
        [Test]
        public void While_OneLoop()
        {
            var code = @"

create buffer a(p int)

insert into a
select 2 
     
while(a) {
    truncate table a
}

select *
from a

";
            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.RowCount == 0);
            };

            runable.Run();
            Assert.True(called == 1);

        }
    }
}
