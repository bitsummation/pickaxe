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
    public class PrimitiveFunctionTests
    {
        [Test]
        public void GetDateTest()
        {
            var code = @"
        
 select getdate() as currentDate

";
            var runable = TestHelper.Compile(code, null);
            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "currentDate");
                Assert.IsTrue(table.RowCount == 1);
                Assert.IsTrue(!string.IsNullOrEmpty(table[0][0].ToString()));
            };

            runable.Run();
            Assert.True(called == 1);

        }
    }
}
