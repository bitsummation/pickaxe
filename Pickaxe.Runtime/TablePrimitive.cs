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

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Runtime
{
    public abstract class TablePrimitive
    {
        public static TablePrimitive Integer = new IntegerImpl();
        public static TablePrimitive Float = new FloatImpl();
        public static TablePrimitive String = new StringImpl();
        public static TablePrimitive Identity = new IdentityImpl();

        public static TablePrimitive FromType(Type type)
        {
            TablePrimitive primitive = null;

            if (Integer.Type == type)
                primitive = Integer;
            else if (Float.Type == type)
                primitive = Float;
            else if (String.Type == type)
                primitive = String;

            return primitive;
        }

        public static TablePrimitive FromString(string type)
        {
            TablePrimitive primitive = Integer;

            if (Integer.TypeString == type)
                primitive = Integer;
            else if (Float.TypeString == type)
                primitive = Float;
            else if (String.TypeString == type)
                primitive = String;
            else if (Identity.TypeString == type)
                primitive = Identity;
            else
                throw new InvalidOperationException("Unexpected Type");

            return primitive;
        }

        public virtual bool IsIdentity { get { return false; } }
        public abstract string TypeString { get; }
        public abstract Type Type { get; }
        public abstract CodeExpression ToNative(CodeExpression expression);

        private class IntegerImpl : TablePrimitive
        {
            public override string TypeString
            {
                get { return "int"; }
            }

            public override Type Type
            {
                get { return typeof(int); }
            }

            public override CodeExpression ToNative(CodeExpression expression)
            {
                return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Convert"), "ToInt32", expression);
            }
        }

        private class IdentityImpl : IntegerImpl
        {
            public override string TypeString
            {
                get { return "identity"; }
            }

            public override bool IsIdentity { get { return true; } }
        }

        private class FloatImpl : TablePrimitive
        {
            public override string TypeString
            {
                get { return "float"; }
            }

            public override Type Type
            {
                get { return typeof(float); }
            }

            public override CodeExpression ToNative(CodeExpression expression)
            {
                return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Convert"), "ToSingle", expression);
            }
        }

        private class StringImpl : TablePrimitive
        {
            public override string TypeString
            {
                get { return "string"; }
            }

            public override Type Type
            {
                get { return typeof(string); }
            }

            public override CodeExpression ToNative(CodeExpression expression)
            {
                return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Convert"), "ToString", expression);
            }
        }
    }
}
