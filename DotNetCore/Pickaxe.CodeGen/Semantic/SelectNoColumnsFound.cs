using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.CodeDom.Semantic
{
    public class SelectNoColumnsFound : SemanticException
    {
        public SelectNoColumnsFound(LineInfo line)
            : base(line)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("No columns can be found for select * due to dynamic object. Must Specify columns names {0})", base.Message);
            }
        }
    }
}
