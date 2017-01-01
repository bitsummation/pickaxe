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
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PickAxe.Tests
{
    public static class TestHelper
    {
        public static void CompileExpectError(Type errorType, string code, IHttpRequestFactory requestFactory)
        {
            CompileExpectError(errorType, 1, code, requestFactory);
        }

        public static void CompileExpectError(Type errorType, int errorCount, string code, IHttpRequestFactory requestFactory)
        {
            var compiler = new Compiler(code);
            var assembly = compiler.ToAssembly();
            Assert.IsTrue(compiler.Errors.Where(x => x.GetType() == errorType).Count() == errorCount);
        }

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
