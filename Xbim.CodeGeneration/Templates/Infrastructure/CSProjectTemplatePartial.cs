using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class CSProjectTemplate
    {
        public string Name { get; private set; }

        public string Id { get; private set; }

        public CSProjectTemplate(string projectName)
        {
            Name = projectName;
            Id = Guid.NewGuid().ToString();
        }
    }
}
