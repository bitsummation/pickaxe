using NUnit.Framework;
using Pickaxe.Emit;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    public static class TestHelper
    {
        public static Runable Compile(string code, IHttpRequestFactory requestFactory)
        {
            var compiler = new Compiler(code);
            var assembly = compiler.ToAssembly();
            Assert.IsTrue(compiler.Errors.Count == 0);
            var runable = new Runable(assembly);
            if(requestFactory != null)
                runable.SetRequestFactory(requestFactory);
            return runable;
        }
    }
}
