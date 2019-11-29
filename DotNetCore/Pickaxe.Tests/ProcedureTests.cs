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

namespace PickAxe.Tests
{
    [TestFixture]
    public class ProcedureTests
    {
        [Test]
        public void Procedure_TestBasic()
        {
            var input = @"

  procedure temp (a string, b string) {
    
    select *
    from download page 'http://test.com'

}

";

            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
        }
    }
}
