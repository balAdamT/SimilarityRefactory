using SimilarityExample.Zero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var _1 = new DoThingWithOne();
            var _2 = new DoThingWithTwo();

            _1.DoThing();
            _2.DoThing();
        }
    }
}
