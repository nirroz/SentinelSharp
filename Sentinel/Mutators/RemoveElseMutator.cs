using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sentinel.Mutators
{
    class RemoveElseMutator : BaseMutator
    {
        public override string MutationName => "RemoveElseMutator";

        public override SyntaxNode VisitElseClause(ElseClauseSyntax node)
        {
            if (!ShouldRun(node)) { return base.VisitElseClause(node); }

            var newNode = node.WithStatement(SyntaxFactory.EmptyStatement());
            return newNode;
        }
    }
}
