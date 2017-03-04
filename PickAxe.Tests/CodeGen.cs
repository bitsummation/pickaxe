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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class CodeGen
    {
        [Test]
        public void TestCodeRunner()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; //TLS 1.2
            //var code = new Code(new string[0]);
            //code.Run();
        }
       
        [Test]
        public void BasicCodeGenTest()
        {
            var input = @"

 select *
from download page 'https://www.walmart.com/ip/Gatorade-Variety-Pack-12-Oz-18-Pk/16224470' with (js) => {
""
	var primaryProductId = __WML_REDUX_INITIAL_STATE__.product.primaryProduct;

	var primaryProduct = __WML_REDUX_INITIAL_STATE__.product.products[primaryProductId];

	return [{upc:primaryProduct.upc, url:url}];
""
}      

";
            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
