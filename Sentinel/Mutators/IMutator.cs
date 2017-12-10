using Microsoft.CodeAnalysis;

namespace Sentinel.Mutators
{
    internal interface IMutator
    {
        int chosenIndex { get; }
        string MutationName { get; }
        SyntaxNode ChangedNode { get; }
        IMutationResult MutateRandom(string text);
        IMutationResult MutateSpecific(string text, int index);
    }

    internal interface IMutationResult
    {
        MutationStatus MutationStatus { get; }
        string NewText { get; }
    }

    public class MutationResult : IMutationResult
    {
        public MutationStatus MutationStatus { get; set; }
        public string NewText { get; set; }
    }

    public enum MutationStatus
    {
        Success,
        CantParse,
        NoSuitableMutationFound
    }
}