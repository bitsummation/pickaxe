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
using Pickaxe.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class CodeGen
    {
        [Test]
        public void BasicCodeGenTest()
        {
              var input = @"

proxies ('50.207.44.25:8080', '69.89.107.3:8088', '50.232.240.6:8080', '104.197.107.186:3128'
,'104.197.39.26:3128', '128.114.232.28:80')
with test {	
	select
		pick 'div#atcui-navigation-container li.cars-for-sale a' take text
	from download page 'http://www.autotrader.com/'
}

select
	pick '.pageof' take text match '\D+(\d+)\D+(\d+)' replace '$1',
	pick '.pageof' take text match '\D+(\d+)\D+(\d+)' replace '$2'
from download page 'http://www.autotrader.com/car-dealers/Los+Angeles+CA-90005?filterName=pagination&firstRecord=1&numRecords=10&searchRadius=50&sortBy=distanceASC&vehicleInventory=used'

exec dealer('http://www.autotrader.com/car-dealers/Los+Angeles+CA-90005?filterName=pagination&firstRecord=','&numRecords=10&searchRadius=50&sortBy=distanceASC&vehicleInventory=used')

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
