using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentinel.Mutators;

namespace Sentinel.Tests.MutatorTests
{
    [TestClass]
    public class MethodRemoverTests : BaseMutatorTest
    {
        public MethodRemoverTests() : base(() => new MethodRemoverMutator())
        {
        }

        [TestMethod]
        public void MethodRemoverMutator_voidMethod_onceChange()
        {
            var input =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    class SomeClass
    {
        void SomeVoidMethod(string paramOne, int paramTwo)
        {
            SomeMethodContent(1);
            SomeMoreContent(true);
        }
    }
}
";


            var output =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    class SomeClass
    {
        void SomeVoidMethod(string paramOne, int paramTwo)
{}    }
}
";

            DoTest(input, output);

        }

        [TestMethod]
        public void MethodRemoverMutator_MethodWithReturnType_onceChange()
        {
            var input =
                @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    class SomeClass
    {
        object SomeVoidMethod(string paramOne, int paramTwo)
        {
            SomeMethodContent(1);
            var someReturnValue = SomeMoreContent(true);
            return someReturnValue;
        }
    }
}
";


            var output =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    class SomeClass
    {
        object SomeVoidMethod(string paramOne, int paramTwo)
{return(default(        object ));}    }
}";

            DoTest(input, output);

        }


        [TestMethod]
        public void MethodRemoverMutator_InterfaceNotAffected()
        {
            var input =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    interface SomeInterface
    {
        void SomeInterfaceMethod(string paramOne, int paramTwo);
    }
}
";

            DoTestNoChange(input);

        }

        [TestMethod]
        public void MethodRemoverMutator_PartialMethodNotAffected()
        {
            var input =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Tests.MutatorTests
{
    partial class SomePartialClass
    {
        partial void SomePartialMethod(string paramOne, int paramTwo);
    }
}
";

            DoTestNoChange(input);

        }


    }
}
