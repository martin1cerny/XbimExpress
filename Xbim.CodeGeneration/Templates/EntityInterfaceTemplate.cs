﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Xbim.CodeGeneration.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class EntityInterfaceTemplate : EntityTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n");
            
            #line 7 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var u in Using) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 8 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(u));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 9 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\nnamespace ");
            
            #line 11 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\tpublic partial interface I");
            
            #line 13 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 13 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Inheritance));
            
            #line default
            #line hidden
            this.Write("\r\n\t{\r\n\t\t#region Explicit attributes\r\n");
            
            #line 16 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in ExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 17 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(" @");
            
            #line 17 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("  { get; set; }\r\n");
            
            #line 18 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t#endregion\r\n\r\n\t\t#region Inverse attributes\r\n");
            
            #line 22 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in InverseAttributes){   var inverseType = "I" + attribute.Domain.Name; 
            
            #line default
            #line hidden
            this.Write("\t\tIEnumerable<");
            
            #line 23 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write("> @");
            
            #line 23 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("  { get; }\r\n");
            
            #line 24 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t#endregion\r\n\t}\r\n\r\n");
            
            #line 28 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 if (!IsAbstract) { 
            
            #line default
            #line hidden
            this.Write("\t[EntityName(\"");
            
            #line 29 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Type.PersistanceName));
            
            #line default
            #line hidden
            this.Write("\")]\r\n\tpublic partial class @");
            
            #line 30 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write(" : I");
            
            #line 30 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write("\r\n\t{\r\n\t\t#region Implementation of ");
            
            #line 32 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PersistEntityInterface));
            
            #line default
            #line hidden
            this.Write("\r\n\t\tpublic int EntityLabel {get; set;}\r\n\t\tpublic ");
            
            #line 34 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" Model { get; internal set; }\r\n\r\n\t\tprotected bool Activated = false;\r\n\t\t#endregio" +
                    "n\r\n\r\n\t\t//internal constructor makes sure that objects are not created outside of" +
                    " the model/ assembly controlled area\r\n\t\tinternal ");
            
            #line 40 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 40 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInterface));
            
            #line default
            #line hidden
            this.Write(" model)\r\n\t\t{ \r\n\t\t\tModel = model; \r\n");
            
            #line 43 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach (var attribute in AggregatedExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(" = new ");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write("( model );\r\n");
            
            #line 45 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t}\r\n\r\n\t\t#region Explicit attribute fields\r\n");
            
            #line 49 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\tprivate ");
            
            #line 50 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 50 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 51 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t#endregion\r\n\t\r\n\t\t#region Explicit attribute properties\r\n");
            
            #line 55 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t[EntityAttribute(");
            
            #line 56 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAttributeIndex(attribute)));
            
            #line default
            #line hidden
            this.Write(", EntityAttributeState.");
            
            #line 56 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.OptionalFlag ? "Optional" : "Mandatory"));
            
            #line default
            #line hidden
            this.Write(")]\r\n\t\tpublic ");
            
            #line 57 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(" @");
            
            #line 57 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" \r\n\t\t{ \r\n\t\t\tget \r\n\t\t\t{\r\n\t\t\t\tif(Activated) return ");
            
            #line 61 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(";\r\n\t\t\t\t\r\n\t\t\t\tModel.Activate(this, true);\r\n\t\t\t\tActivated = true;\r\n\t\t\t\treturn ");
            
            #line 65 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(";\r\n\t\t\t} \r\n\t\t\tset\r\n\t\t\t{\r\n\t\t\t\tSetValue( v =>  ");
            
            #line 69 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(" = v, ");
            
            #line 69 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetPrivateFieldName(attribute)));
            
            #line default
            #line hidden
            this.Write(", value,  \"");
            
            #line 69 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t\t} \r\n\t\t}\r\n\t\r\n");
            
            #line 73 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t#endregion\r\n\r\n\t\t#region Inverse attributes\r\n");
            
            #line 77 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllInverseAttributes){  
	var inverseType = "I" + attribute.Domain.Name; 
            
            #line default
            #line hidden
            this.Write("\t\tpublic IEnumerable<");
            
            #line 79 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write("> @");
            
            #line 79 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" \r\n\t\t{ \r\n\t\t\tget \r\n\t\t\t{\r\n");
            
            #line 83 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 if (IsDoubleAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.Where<");
            
            #line 84 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => e.");
            
            #line 84 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" != null &&  e.");
            
            #line 84 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(".Any( i => i.Contains(this)));\r\n");
            
            #line 85 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } else if (IsAggregation(attribute)){
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.Where<");
            
            #line 86 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => e.");
            
            #line 86 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" != null &&  e.");
            
            #line 86 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(".Contains(this));\r\n");
            
            #line 87 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } else {
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.Where<");
            
            #line 88 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => e.");
            
            #line 88 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" == this);\r\n");
            
            #line 89 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t} \r\n\t\t}\r\n\t\r\n");
            
            #line 93 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write(@"		#endregion

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged( string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		#endregion

		#region Transactional property setting

		protected void SetValue<TProperty>(Action<TProperty> setter, TProperty oldValue, TProperty newValue, string notifyPropertyName)
		{

			if (!Model.IsTransactional)
			{
				setter(newValue);
				NotifyPropertyChanged(notifyPropertyName);
				return;
			}

			//check there is a transaction
			var txn = Model.CurrentTransaction;
			if (txn == null) throw new Exception(""Operation out of transaction."");

			Action undo = () => setter(oldValue);
			txn.AddReversibleAction(undo);
			setter(newValue);
			NotifyPropertyChanged(notifyPropertyName);
		}

		#endregion

		#region Index access
		private readonly List<string> _attributeNames = new List<string>
		{
");
            
            #line 137 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\"");
            
            #line 138 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("\",\r\n");
            
            #line 139 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t};\r\n\r\n\t\tIEnumerable<string> ");
            
            #line 142 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableInterface));
            
            #line default
            #line hidden
            this.Write(".PropertyNames { get { return _attributeNames; } }\r\n\r\n\t\tobject ");
            
            #line 144 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableInterface));
            
            #line default
            #line hidden
            this.Write(".GetValue(int index)\r\n\t\t{\r\n\t\t\tswitch(index)\r\n\t\t\t{\r\n");
            
            #line 148 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase ");
            
            #line 149 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAttributeIndex(attribute)));
            
            #line default
            #line hidden
            this.Write(": \r\n\t\t\t\t\treturn @");
            
            #line 150 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 151 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\tthrow new System.IndexOutOfRangeException();\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\to" +
                    "bject ");
            
            #line 157 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableInterface));
            
            #line default
            #line hidden
            this.Write(".GetValue(string name)\r\n\t\t{\r\n\t\t\tswitch(name)\r\n\t\t\t{\r\n");
            
            #line 161 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 162 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("\":  \r\n\t\t\t\t\treturn @");
            
            #line 163 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 164 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\tthrow new System.IndexOutOfRangeException();\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tv" +
                    "oid ");
            
            #line 170 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableInterface));
            
            #line default
            #line hidden
            this.Write(".SetValue(int index, object value)\r\n\t\t{\r\n\t\t\tswitch(index)\r\n\t\t\t{\r\n");
            
            #line 174 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase ");
            
            #line 175 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAttributeIndex(attribute)));
            
            #line default
            #line hidden
            this.Write(":\r\n");
            
            #line 176 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		if (IsReferenceTypeAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 177 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(".Add((");
            
            #line 177 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAggregationElementType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value);\r\n");
            
            #line 178 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else if (IsValueTypeAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 179 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(".Add((");
            
            #line 179 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAggregationElementType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value);\r\n");
            
            #line 180 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else if (IsReferenceType(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 181 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" = (");
            
            #line 181 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value;\r\n");
            
            #line 182 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t if (value != null) @");
            
            #line 183 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" = (");
            
            #line 183 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value;\r\n");
            
            #line 184 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t break;\r\n");
            
            #line 186 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\tthrow new System.IndexOutOfRangeException();\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tv" +
                    "oid ");
            
            #line 192 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(InstantiableInterface));
            
            #line default
            #line hidden
            this.Write(".SetValue(string propName, object value)\r\n\t\t{\r\n\t\t\tswitch(propName)\r\n\t\t\t{\r\n");
            
            #line 196 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach(var attribute in AllExplicitAttributes){ 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tcase \"");
            
            #line 197 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write("\":\r\n");
            
            #line 198 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		if (IsReferenceTypeAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 199 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(".Add((");
            
            #line 199 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAggregationElementType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value);\r\n");
            
            #line 200 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else if (IsValueTypeAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 201 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(".Add((");
            
            #line 201 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAggregationElementType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value);\r\n");
            
            #line 202 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else if (IsReferenceType(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t @");
            
            #line 203 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" = (");
            
            #line 203 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value;\r\n");
            
            #line 204 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} else {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t if (value != null) @");
            
            #line 205 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" = (");
            
            #line 205 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCSType(attribute)));
            
            #line default
            #line hidden
            this.Write(")value;\r\n");
            
            #line 206 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
		} 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t break;\r\n");
            
            #line 208 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tdefault:\r\n\t\t\t\t\tthrow new System.IndexOutOfRangeException();\r\n\t\t\t}\r\n\t\t}\r\n\t\t#en" +
                    "dregion\r\n\r\n");
            
            #line 215 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 if (WhereRules.Any()) { 
            
            #line default
            #line hidden
            this.Write("\t\t#region Where rules\r\n");
            
            #line 217 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 foreach (var rule in WhereRules) {
            
            #line default
            #line hidden
            this.Write("\t\t/*");
            
            #line 218 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Description));
            
            #line default
            #line hidden
            this.Write("*/\r\n");
            
            #line 219 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t#endregion\r\n");
            
            #line 221 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t}\r\n\r\n");
            
            #line 224 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\EntityInterfaceTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
