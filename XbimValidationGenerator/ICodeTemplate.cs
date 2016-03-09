using System.Collections.Generic;

namespace XbimValidationGenerator
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
