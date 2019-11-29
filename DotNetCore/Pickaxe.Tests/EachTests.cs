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
    public class EachTests
    {
        [Test]
        public void Each_Basic()
        {
            var code = @"

create buffer temp(id int)

insert into temp
select 1

insert into temp
select 2

each(var t in temp){
	
	insert into temp
	select t.id

}

select *
from temp

";

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.RowCount == 4);

                Assert.IsTrue(table[0][0].ToString() == "1");
                Assert.IsTrue(table[1][0].ToString() == "2");
                Assert.IsTrue(table[2][0].ToString() == "1");
                Assert.IsTrue(table[3][0].ToString() == "2");
            };

            runable.Run();
            Assert.IsTrue(called == 1);

        }

    }
}
