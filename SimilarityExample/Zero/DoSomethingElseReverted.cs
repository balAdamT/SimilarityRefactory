using SimilarityExample.One;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityExample.Zero
{
    class DoSomethingElseReverted
    {
        public void DoThing()
        {
            var a = new A();
            var b = new A();
            b.Increase();
            a.Decrease();
        }
    }
}
