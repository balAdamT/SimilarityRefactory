using Microsoft.CodeAnalysis;
using SyntaxComparision.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Information
{
    public class SingleTreeInformation : ISyntaxInformation
    {
        private SemanticModel model;

        public SingleTreeInformation()
        {

        }

        public SingleTreeInformation(SemanticModel model)
        {
            this.model = model;
        }

        public SemanticModel Provide(SyntaxTree tree)
        {
            return model;
        }
    }
}
