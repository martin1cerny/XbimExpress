using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.CodeGeneration
{
    public interface ICodeTemplate
    {
        string TransformText();
        void WriteLine(string format, params object[] args);
        void WriteLine(string textToAppend);
        void Write(string format, params object[] args);
        void Write(string textToAppend);

        string Name { get; }
        string Namespace { get; }
        string Inheritance { get; }
        IEnumerable<string> Using { get; }  
    }
}
