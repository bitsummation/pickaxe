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

  create buffer temp(id int)
  insert into temp
  select 2     

    select 
       size
    from download page 'http://'
    where size < '20' and nodes = 'p.new' --(id > 10 and id < 400) or id = 10

    select id
    from temp

--nodes
";

              var join = @"

    create buffer a (id int, name string)
    create buffer b (id int, name string)

    select *
    from a
    join b on a.id = b.id

";


            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
            Assert.IsTrue(compiler.Errors.Count == 0);
        }
    }
}
