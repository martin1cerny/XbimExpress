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

        public string Id { get { return Guid.NewGuid().ToString(); } }

        public CSProjectTemplate(string projectName)
        {
            Name = projectName;
        }
    }
}
