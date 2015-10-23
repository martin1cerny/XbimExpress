using System.Collections.Generic;
using System.Linq;
using XbimTranslatorGenerator.Differences;

namespace XbimTranslatorGenerator.Templates
{
    public partial class TranslatorTemplate: ICodeTemplate
    {
        private readonly EntityDefinitionMatch _match;

        public TranslatorTemplate(EntityDefinitionMatch match)
        {
            _match = match;
        }

        public string Name { get { return _match.Source.Name + "Translator"; } }
        public string Namespace { get { return Settings.Namespace; } }

        private IEnumerable<ExplicitAttributeMatch> MatchesToImplement
        {
            get { return _match.AttributeMatches.Where(am => am.MatchType != AttributeMatchType.Identity); }
        } 

        //properties used in the template
        public int[] TranslatesProperties { get
        {
            return MatchesToImplement.Select(am => am.SourceAttributeOrder).ToArray();
        } }
        public string TranslatesEntity {
            get { return _match.Source.Name.ToUpper(); }
        }
        public string OriginalSchema { get { return _match.Source.ParentSchema.Name; } }
    }
}
