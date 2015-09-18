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
            Assert.IsTrue(called == 1);
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
            Assert.IsTrue(called == 1);
        }
    }
}
