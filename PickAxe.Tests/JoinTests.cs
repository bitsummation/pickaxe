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
    public class JoinTests
    {

        [Test]
        public void Join_Three_SelectArgs()
        {

            var code = @"

   create buffer a (id int, name string)
create buffer b (id int, name string)
create buffer c (id int, name string)

insert into a
select 1, 'a'

insert into a
select 2, 'first'

insert into b
select 1, 'b'

insert into c
select 1, 'c'

select a.name, b.name, c.name
from a
join b on a.id = b.id
join c on c.id = b.id

";

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 3);
                Assert.IsTrue(table.RowCount == 1);

                Assert.IsTrue(table[0][0].ToString() == "a");
                Assert.IsTrue(table[0][1].ToString() == "b");
                Assert.IsTrue(table[0][2].ToString() == "c");
            };

            runable.Run();
            Assert.IsTrue(called == 1);
        }

        [Test]
        public void Join_Two_SelectArgs()
        {

            var code = @"

   create buffer a (id int, name string)
create buffer b (id int, name string)

insert into a
select 1, 'first'

insert into a
select 2, 'first'

insert into b
select 1, 'second'

select a.name, b.name
from a
join b on a.id = b.id

";
            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.RowCount == 1);

                Assert.IsTrue(table[0][0].ToString() == "first");
                Assert.IsTrue(table[0][1].ToString() == "second");
            };

            runable.Run();
            Assert.IsTrue(called == 1);
        }
    }
}
