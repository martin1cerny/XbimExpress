﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Xbim.CodeGeneration.Templates.CrossAccess
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Xbim.ExpressParser.SDAI;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class EntityInterfaceImplementation : EntityInterfaceTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n");
            
            #line 8 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 foreach(var u in Using) { 
            
            #line default
            #line hidden
            this.Write("using ");
            
            #line 9 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(u));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 10 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n// ReSharper disable once CheckNamespace\r\nnamespace ");
            
            #line 13 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(OwnNamespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n\tpublic partial class @");
            
            #line 15 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 15 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Interface));
            
            #line default
            #line hidden
            this.Write("\r\n\t{\r\n");
            
            #line 17 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
	foreach(var attribute in ExplicitAttributesToImplement){ 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 18 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetInterfaceCSTypeFull(attribute)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 18 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture("I" + attribute.ParentEntity.Name));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 18 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" \r\n\t\t{ \r\n\t\t\tget\r\n\t\t\t{\r\n");
            
            #line 22 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
		var match = GetMatch(attribute); 
		if (match == null || !match.IsTypeCompatible) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tthrow new System.NotImplementedException();\r\n");
            
            #line 25 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
		} else if (attribute.Domain is EntityDefinition) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn ");
            
            #line 26 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(match.SourceAttribute.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 27 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
		} else { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tthrow new System.NotImplementedException();\r\n");
            
            #line 29 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
		} 
            
            #line default
            #line hidden
            this.Write("\t\t\t} \r\n\t\t}\r\n");
            
            #line 32 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
	} 
            
            #line default
            #line hidden
            
            #line 33 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 foreach(var attribute in InverseAttributesToImplement){  
	var inverseType = "I" + attribute.Domain.Name; 
            
            #line default
            #line hidden
            
            #line 35 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 if (IsEnumeration(attribute)) { 
            
            #line default
            #line hidden
            this.Write("\t\tIEnumerable<");
            
            #line 36 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 36 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture("I" + attribute.ParentEntity.Name));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 36 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" \r\n");
            
            #line 37 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 38 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 38 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Interface));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 38 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.Name));
            
            #line default
            #line hidden
            this.Write(" \r\n");
            
            #line 39 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t{ \r\n\t\t\tget\r\n\t\t\t{\r\n");
            
            #line 43 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 if (IsDoubleAggregation(attribute)) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(IsEnumeration(attribute) ? "Where" : "FirstOrDefault"));
            
            #line default
            #line hidden
            this.Write("<");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => e.");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" != null &&  e.");
            
            #line 44 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(".Any( i => i.Contains(this)));\r\n");
            
            #line 45 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } else if (IsAggregation(attribute)){
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(IsEnumeration(attribute) ? "Where" : "FirstOrDefault"));
            
            #line default
            #line hidden
            this.Write("<");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => e.");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" != null &&  e.");
            
            #line 46 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(".Contains(this));\r\n");
            
            #line 47 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } else {
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn Model.Instances.");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(IsEnumeration(attribute) ? "Where" : "FirstOrDefault"));
            
            #line default
            #line hidden
            this.Write("<");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(inverseType));
            
            #line default
            #line hidden
            this.Write(">(e => (e.");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(attribute.InvertedAttr.Name));
            
            #line default
            #line hidden
            this.Write(" as ");
            
            #line 48 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Name));
            
            #line default
            #line hidden
            this.Write(") == this);\r\n");
            
            #line 49 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t} \r\n\t\t}\r\n");
            
            #line 52 "C:\CODE\XbimGit\XbimExpress\Xbim.CodeGeneration\Templates\CrossAccess\EntityInterfaceImplementation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t}\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
