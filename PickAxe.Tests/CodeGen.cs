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

select
		case size
        when 5 then 200
        when 661023 then 200
        else 20
        end,
        
        /*
		case pick '.listing-media a img' take attribute 'src' match '^http.*'
		when null then pick '.listing-media a img' take attribute 'data-original' match 'scaler/\d+/\d+/' replace 'scaler/'
		else pick '.listing-media a img' take attribute 'src' match 'scaler/\d+/\d+/' replace 'scaler/'
		end,*/

        case
        when pick '.listing-mileage' take text match '\d+' = 10000 then 'low mileage'
        else 100
        end,
		
		/*
        case
        when size < 5 then 200
        else 100
        end,*/

		pick '.listing-title span.atcui-truncate span' take text,
		pick '.price-offer-wrapper .primary-price span' take text match '\d+',
		pick '.listing-mileage' take text match '\d+',
		size + ' test',
		size
	from download page 'http://www.autotrader.com/car-dealers/Austin+TX-78717/64661397/Nyle+Maxwell+Chrysler+Dodge+Jeep+of+Austin?listingTypes=used'
	where nodes = 'div.listing-findcar'
";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
