namespace XbimTranslatorGenerator.Templates
{
    // ReSharper disable once InconsistentNaming
    public partial class ITranslatorTemplate:ICodeTemplate
    {

        public string Name
        {
            get { return "ITranslator"; }
        }

        public string Namespace
        {
            get { return Settings.Namespace; }
        }
    }
}
