using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxVectors.Masking
{
    public static class SyntaxMasks
    {
    public static ISyntaxMask AllNodes => new AllNodesMask();
  }
}
