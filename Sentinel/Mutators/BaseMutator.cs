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
    abstract class BaseMutator : CSharpSyntaxRewriter, IMutator
    {
        private bool IsDetector;

        protected BaseMutator()
        {
        }

        public abstract string MutationName { get; }

        public SyntaxNode ChangeNode { get; protected set; }

        private int counter = 0;

        public int chosenIndex { get; set; }

        public SyntaxNode ChangedNode { get; set; }

        public IMutationResult MutateRandom(string text)
        {
            SyntaxNode root = GetRoot(text);
            if (root == null)
            {
                return new MutationResult { MutationStatus = MutationStatus.CantParse };
            }

            counter = VisitAndCount(root);
            var index = new Random().Next(0, counter);
            return MutateSpecific(root, index);
        }

        private int VisitAndCount(SyntaxNode node)
        {
            IsDetector = true;
            Visit(node);
            IsDetector = false;
            return counter;
        }

        private SyntaxNode GetRoot(string text)
        {
            return CSharpSyntaxTree.ParseText(text)?.GetRoot();
        }

        public IMutationResult MutateSpecific(string text, int index)
        {
            SyntaxNode root = GetRoot(text);
            if (root == null)
            {
                return new MutationResult { MutationStatus = MutationStatus.CantParse };
            }

            return MutateSpecific(root, index);
        }

        private IMutationResult MutateSpecific(SyntaxNode node, int index)
        {
            IsDetector = false;
            chosenIndex = index;
            var newRoot = Visit(node);
            var res = new MutationResult {MutationStatus = MutationStatus.Success, NewText = newRoot.ToFullString()};
            return res;
        }

        protected bool ShouldRun(SyntaxNode node)
        {
            if (IsDetector)
            {
                counter++;
                return false;
            }

            chosenIndex--;

            var shouldRun = (chosenIndex + 1 == 0);
            if (shouldRun)
            {
                ChangedNode = node;
            }

            return shouldRun;
        }


        /// <summary>
        /// Skip TestClasses
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.AttributeLists.ToFullString().ToLower().Contains("testclass"))
            {
                return node;
            }

            return base.VisitClassDeclaration(node);
        }
    }
}
