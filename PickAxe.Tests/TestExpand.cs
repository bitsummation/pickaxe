using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    [TestFixture]
    public class TestExpand
    {
        [Test]
        public void TestExpandExpression()
        {
            var code = @"      
 select *
    from expand(0 to 10){1 + $*10}
";
            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "value");
                Assert.IsTrue(table.RowCount == 11);
                for (int x = 0; x < table.RowCount; x++)
                {
                    var val = int.Parse(table[x][0].ToString());
                    Assert.IsTrue(val == (1+x*10));
                }
            };

            runable.Run();

        }

        [Test]
        public void TestExpandBasic()
        {
            var code = @"      
 select *
    from expand(0 to 10)
";
            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "value");
                Assert.IsTrue(table.RowCount == 11);
                for (int x = 0; x < table.RowCount; x++)
                {
                    var val = int.Parse(table[x][0].ToString());
                    Assert.IsTrue(val == x);
                }
            };

            runable.Run();
        }
    }
}
