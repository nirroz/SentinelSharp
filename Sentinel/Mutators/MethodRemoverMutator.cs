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
    class MethodRemoverMutator : BaseMutator
    {
        public override string MutationName => "MethodRemoverMutator";

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.AttributeLists.ToFullString().ToLower().Contains("testmethod"))
            {
                return base.VisitMethodDeclaration(node);
            }

            if (node.Body == null)
            {
                return base.VisitMethodDeclaration(node);
            }

            if (!ShouldRun(node)) { return base.VisitMethodDeclaration(node); }

            var returnTypeString = node.ReturnType.ToFullString().Trim().ToLower();

            switch (returnTypeString)
            {
                case "void":
                    return node.WithBody(SyntaxFactory.Block());
                case "int":
                    return node.WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.ParenthesizedExpression(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        SyntaxFactory.Literal(-1))))));
                case "bool":
                    return node.WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.ParenthesizedExpression(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))));


                default:
                    return node.WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.ParenthesizedExpression(
                                    SyntaxFactory.DefaultExpression(node.ReturnType)))));
            }
        }
    }
}
