using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.ExpressParser.SDAI;

namespace Xbim.ExpressParser.Tests
{
    [TestClass]
    public class SdaiSchemaTests
    {
        [TestMethod]
        public void SchemaCreation()
        {
            var model = new SchemaModel();
            var schema = model.FirstSchema;
            schema.Name = "Sample Schema";
            schema.Identification = "SAMPLE_TEST";

            var definedType = model.New(schema, (DefinedType t) =>
            {
                t.Name = "Identifier";
                t.Domain = model.PredefinedSimpleTypes.StringType;
            });

            var entity = model.New(schema, (EntityDefinition e) => { e.Name = "Model"; });
            entity.AddAttribute(model.New(schema, (ExplicitAttribute a) =>
            {
                a.Name = "Name";
                a.Domain = definedType;
            }));
            entity.AddAttribute(model.New(schema, (ExplicitAttribute a) =>
            {
                a.Name = "Description";
                a.Domain = model.PredefinedSimpleTypes.StringType;
            }));
        }
    }
}