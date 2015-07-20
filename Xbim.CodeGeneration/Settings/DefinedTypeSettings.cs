using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.CodeGeneration.Settings
{
    public class DefinedTypeSettings
    {
        public DefinedTypeSettings()
        {
            BaseType = "IBaseType";
        }

        public string BaseType { get; set; }
    }
}
