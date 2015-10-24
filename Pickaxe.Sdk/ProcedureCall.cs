using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pickaxe.Sdk
{
    public class ProcedureCall : AstNode
    {
        public string Name { get; set; }

        public AstNode[] Args
        {
            get { return Children.ToArray(); }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
