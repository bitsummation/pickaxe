using NUnit.Framework;
using Pickaxe.CodeDom.Visitor;
using Pickaxe.Emit;
using Pickaxe.Parser;
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
        public void BasicTest()
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

            var parser = new CodeParser(input);
            var ast = parser.Parse();
            if(parser.Errors.Count > 0)
            {
                foreach (var error in parser.Errors)
                    System.Console.WriteLine(error.Message);
                return;
            }

            var generator = new CodeDomGenerator(ast);
            var unit = generator.Generate();
            if (generator.Errors.Count > 0)
            {
                foreach (var error in generator.Errors)
                    System.Console.WriteLine(error.Message);
                return;
            }

            var persist = new Persist(unit);
            var code = persist.ToCSharpSource();
            persist.ToAssembly();
        }
    }
}
