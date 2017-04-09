using Moq;
using NUnit.Framework;
using Pickaxe.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickAxe.Tests
{
    [TestFixture]
    public class RunJavascriptTest
    {

        [Test]
        public void ReturnObjectArrayOneElement()
        {
            var code = string.Format(@"
   select upc
from download page '{0}\Test.html' with (js) => {{
""
  
            return [{{ upc: t.f, url: t.s}}];
            ""
}} ", Directory.GetCurrentDirectory());

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "upc");
                Assert.IsTrue(table[0][0].ToString() == "first");
            };

            runable.Run();
            Assert.True(called == 1);
        }

        [Test]
        public void ReturnObjectArrayMultipleElementsDifferentProperties()
        {
            var code = string.Format(@"
   select upc, blah
from download page '{0}\Test.html' with (js) => {{
""
  
            return [{{ upc: t.f, url: t.s}}, {{blah:'blah'}}];
            ""
}} ", Directory.GetCurrentDirectory());

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.RowCount == 2);
                Assert.IsTrue(table.Columns().Length == 2);

                Assert.IsTrue(table.Columns()[0] == "upc");
                Assert.IsTrue(table[0][0].ToString() == "first");
                Assert.IsTrue(table[0][1] == null);

                Assert.IsTrue(table[1][0] == null);
                Assert.IsTrue(table[1][1].ToString() == "blah");

            };

            runable.Run();
            Assert.True(called == 1);
        }


        [Test]
        public void ReturnObject()
        {
            var code = string.Format(@"
   select upc, url
from download page '{0}\Test.html' with (js) => {{
""
  
            return {{ upc: t.f, url: t.s}};
            ""
}} ", Directory.GetCurrentDirectory());

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 2);
                Assert.IsTrue(table.Columns()[0] == "upc");
                Assert.IsTrue(table.Columns()[1] == "url");
                Assert.IsTrue(table[0][0].ToString() == "first");
                Assert.IsTrue(table[0][1].ToString() == "second");
            };

            runable.Run();
            Assert.True(called == 1);
        }


        [Test]
        public void ReturnObjectArrayNoJsHint()
        {
            var code = string.Format(@"
   select upc
from download page '{0}\Test.html' => {{
""
  
            return [{{ upc: t.f, url: t.s}}];
            ""
}} ", Directory.GetCurrentDirectory());

            var runable = TestHelper.Compile(code, null);

            int called = 0;
            runable.Select += (table) =>
            {
                called++;
                Assert.IsTrue(table.Columns().Length == 1);
                Assert.IsTrue(table.Columns()[0] == "upc");
                Assert.IsTrue(table[0][0].ToString() == "first");
            };

            runable.Run();
            Assert.True(called == 1);
        }

    }
}
