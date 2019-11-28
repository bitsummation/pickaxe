using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.CodeDom
{
    public static class Extensions
    {
        public static Type GenerateType(this CodeTypeReference reference)
        {
            Type type = Type.GetType(reference.BaseType);
            var typeArgs = new List<Type>();

            foreach (CodeTypeReference arg in reference.TypeArguments)
            {
                Type argType = Type.GetType(arg.BaseType);
                typeArgs.Add(argType);
            }

            if (typeArgs.Count > 0)
            {
                type = type.MakeGenericType(typeArgs.ToArray());
            }

            return type;
        }
    }
}
