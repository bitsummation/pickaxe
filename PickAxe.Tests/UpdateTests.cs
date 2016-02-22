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
    public class UpdateTests
    {
        [Test]
        public void Update_Test_WhereOnly()
        {
            var code = @"

    create buffer videos(video string, processed int)

    insert into videos
    select '1', 0

    insert into videos
    select '2', 0

    update videos
    set processed = 1
    where video = '2'

    select *
    from videos
";

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.Columns()[0] == "video");
                Assert.IsTrue(table.Columns()[1] == "processed");
                Assert.IsTrue(table.RowCount == 2);

                Assert.IsTrue(table[0][0].ToString() == "1");
                Assert.IsTrue(table[0][1].ToString() == "0");

                Assert.IsTrue(table[1][0].ToString() == "2");
                Assert.IsTrue(table[1][1].ToString() == "1");
            };

            runable.Run();
            Assert.True(called == 1);
        }

        [Test]
        public void Update_TestAllRows_NoFrom_NoWhere()
        {
            var code = @"

    create buffer videos(video string, processed int)

    insert into videos
    select '1', 0

    insert into videos
    select '2', 0

    update videos
    set processed = 1

    select *
    from videos
";

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.Columns()[0] == "video");
                Assert.IsTrue(table.Columns()[1] == "processed");
                Assert.IsTrue(table.RowCount == 2);

                Assert.IsTrue(table[0][0].ToString() == "1");
                Assert.IsTrue(table[0][1].ToString() == "1");

                Assert.IsTrue(table[1][0].ToString() == "2");
                Assert.IsTrue(table[1][1].ToString() == "1");
            };

            runable.Run();
            Assert.True(called == 1);

        }
    }
}
