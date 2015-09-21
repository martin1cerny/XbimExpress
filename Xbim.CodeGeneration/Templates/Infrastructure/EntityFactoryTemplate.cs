﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class EntityFactoryTemplate : EntityFactoryTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\n");
            
            #line 8 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 foreach(var u in Using) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 9 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(u));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 10 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 12 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\tpublic sealed class ");
            
            #line 14 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 14 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Inheritance));
            
            #line default
            #line hidden
            this.Write("\r\n\t{\r\n\t\tpublic T New<T>(");
            
            #line 16 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" model, int entityLabel, bool activated) where T: ");
            
            #line 16 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableEntityInterface));
            
            #line default
            #line hidden
            this.Write("\r\n\t\t{\r\n\t\t\treturn (T)New(model, typeof(T), entityLabel, activated);\r\n\t\t}\r\n\r\n\t\tpubl" +
                    "ic T New<T>(");
            
            #line 21 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" model, Action<T> init, int entityLabel, bool activated) where T: ");
            
            #line 21 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableEntityInterface));
            
            #line default
            #line hidden
            this.Write("\r\n\t\t{\r\n\t\t\tvar o = New<T>(model, entityLabel, activated);\r\n\t\t\tinit(o);\r\n\t\t\treturn " +
                    "o;\r\n\t\t}\r\n\r\n\t\tpublic ");
            
            #line 28 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableEntityInterface));
            
            #line default
            #line hidden
            this.Write(" New(");
            
            #line 28 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(@" model, Type t, int entityLabel, bool activated)
		{
			//check that the type is from this assembly
			if(t.Assembly != GetType().Assembly)
				throw new Exception(""This factory only creates types from its assembly"");

			return New(model, t.Name, entityLabel, activated);
		}

		public ");
            
            #line 37 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableEntityInterface));
            
            #line default
            #line hidden
            this.Write(" New(");
            
            #line 37 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" model, string typeName, int entityLabel, bool activated)\r\n\t\t{\r\n\t\t\tif (model == n" +
                    "ull || typeName == null)\r\n\t\t\t\tthrow new ArgumentNullException();\r\n\r\n\t\t\tvar name " +
                    "= typeName.ToUpper();\r\n\t\t\tswitch(name)\r\n\t\t\t{\r\n");
            
            #line 45 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 foreach(var entity in NonAbstractEntities) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.Name.ToUpper()));
            
            #line default
            #line hidden
            this.Write("\": return new ");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.Name));
            
            #line default
            #line hidden
            this.Write(" ( model ) { Activated = activated, EntityLabel = entityLabel };\r\n");
            
            #line 47 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
		if (entity.Name.ToUpper() != entity.PersistanceName.ToUpper()) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.PersistanceName.ToUpper()));
            
            #line default
            #line hidden
            this.Write("\": return new ");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.Name));
            
            #line default
            #line hidden
            this.Write(" ( model ) { Activated = activated, EntityLabel = entityLabel };\r\n");
            
            #line 49 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
		} 
	} 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\treturn null;\r\n\t\t\t}\r\n\t\t}\r\n\t\tpublic ");
            
            #line 55 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableEntityInterface));
            
            #line default
            #line hidden
            this.Write(" New(");
            
            #line 55 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" model, int typeId, int entityLabel, bool activated)\r\n\t\t{\r\n\t\t\tif (model == null)\r" +
                    "\n\t\t\t\tthrow new ArgumentNullException();\r\n\r\n\t\t\tswitch(typeId)\r\n\t\t\t{\r\n");
            
            #line 62 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 foreach(var entity in NonAbstractEntities) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase ");
            
            #line 63 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.TypeId));
            
            #line default
            #line hidden
            this.Write(": return new ");
            
            #line 63 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(entity.Name));
            
            #line default
            #line hidden
            this.Write(" ( model ) { Activated = activated, EntityLabel = entityLabel };\r\n");
            
            #line 64 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\treturn null;\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tpublic IExpressType New(string ty" +
                    "peName)\r\n\t\t{\r\n\t\tif (typeName == null)\r\n\t\t\t\tthrow new ArgumentNullException();\r\n\r" +
                    "\n\t\t\tvar name = typeName.ToUpper();\r\n\t\t\tswitch(name)\r\n\t\t\t{\r\n");
            
            #line 78 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
 foreach(var type in DefinedTypes) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 79 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(type.Name.ToUpper()));
            
            #line default
            #line hidden
            this.Write("\": return new ");
            
            #line 79 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(type.Name));
            
            #line default
            #line hidden
            this.Write(" ();\r\n");
            
            #line 80 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
		if (type.Name.ToUpper() != type.PersistanceName.ToUpper()) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 81 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(type.PersistanceName.ToUpper()));
            
            #line default
            #line hidden
            this.Write("\": return new ");
            
            #line 81 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(type.Name));
            
            #line default
            #line hidden
            this.Write(" ();\r\n");
            
            #line 82 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
		}
   } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\treturn null;\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tprivate static List<string> _sche" +
                    "masIds = new List<string> { ");
            
            #line 89 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\Infrastructure\EntityFactoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", SchemasIds.Select(i => "\"" + i + "\""))));
            
            #line default
            #line hidden
            this.Write(" };\r\n\t\tpublic IEnumerable<string> SchemasIds { get { return _schemasIds; } }\r\n\r\n\t" +
                    "}\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public class EntityFactoryTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
