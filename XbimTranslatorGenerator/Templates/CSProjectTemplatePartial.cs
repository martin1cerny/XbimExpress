using System;

namespace XbimTranslatorGenerator.Templates
{
    // ReSharper disable once InconsistentNaming
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
