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
    class AlwaysTrueConditionMutator : BaseMutator
    {
        public override string MutationName => "AlwaysTrueConditionMutator";

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {


            if (ShouldRun(node))
            {
                var newNode = node.WithCondition(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
                return newNode;
            }

            return base.VisitIfStatement(node);
        }
    }
}
