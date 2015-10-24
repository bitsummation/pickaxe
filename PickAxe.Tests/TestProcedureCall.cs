using NUnit.Framework;
using Pickaxe.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    [TestFixture]
    public class TestProcedureCall
    {
        [Test]
        public void TestBasic()
        {
            var input = @"

    exec temp('test', 'test')

";
            var compiler = new Compiler(input);
            var sources = compiler.ToCode();
        }
    }
}
