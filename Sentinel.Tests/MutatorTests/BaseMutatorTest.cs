using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentinel.Mutators;

namespace Sentinel.Tests.MutatorTests
{
    public class BaseMutatorTest
    {
        private readonly Func<IMutator> _mutatorFactory;

        public BaseMutatorTest(Func<IMutator> mutatorFactory)
        {
            _mutatorFactory = mutatorFactory;
        }

        protected void DoTest(string input, string output)
        {
            var mutator = _mutatorFactory();

            var res = mutator.MutateSpecific(input, 0);

            Assert.AreEqual(res.MutationStatus, MutationStatus.Success);
            Assert.AreNotEqual(null, mutator.ChangedNode);
            var newText = res.NewText;
            Assert.AreEqual(output.Trim(), newText.Trim());
        }


        protected void DoTestNoChange(string input)
        {
            var mutator = _mutatorFactory();

            var res = mutator.MutateSpecific(input, 0);

            Assert.AreEqual(res.MutationStatus, MutationStatus.Success);
            var newText = res.NewText;
            Assert.AreEqual(input.Trim(), newText.Trim());
        }

    }
}
