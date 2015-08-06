﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xbim.ExpressParser.ExpressDefinitions {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Schemas {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Schemas() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xbim.ExpressParser.ExpressDefinitions.Schemas", typeof(Schemas).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (* 
        ///LPM/6 Express Schema (Long Form) Formal Release
        ///
        ///Dated July 21st 2003
        ///
        ///   This schema incorporates the changes made to STEP Parts 41, 42, 43 and 45
        ///   The EXPRESS constructs contained in LPM/5 taken from those parts have
        ///   been amended to be accordance with the following documents:
        ///   - ISO 10303-41 2nd Edition (ISO TC 184/SC4/WG12 N525 2000-05-30)
        ///   - ISO 10303-42 2nd Edition (ISO TC 184/SC4/WG12 N617 2000-09-23 as modified by TC1 WG12 N616)
        ///   - ISO 10303-43 2nd Edition
        ///   - ISO 10303-45  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CIS2_lpm61 {
            get {
                return ResourceManager.GetString("CIS2_lpm61", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///Copyright by:
        ///BIM Academy, Northumbria University, 2015
        ///
        ///License: Creative Commons Attribution 4.0 International Public License
        ///(https://creativecommons.org/licenses/by/4.0/legalcode)
        ///
        ///This work is part of xBIM Toolkit (https://github.com/xBimTeam/)
        ///
        ///Contents:
        ///Object model for COBie 2.4 in form of EXPRESS definitions based on COBie Responsibility Matrix. It reflects 
        ///all relations in between different data objects and makes it possible to handle COBie data as an graph structure
        ///rather than ta [rest of string was truncated]&quot;;.
        /// </summary>
        public static string COBieExpress {
            get {
                return ResourceManager.GetString("COBieExpress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///Copyright by:
        ///International Alliance for Interoperability, 1996-2007
        ///
        ///All rights reserved. No part of this documentation may be
        ///reproduced, stored in a retrieval system, or transmitted
        ///in any form or by any means, without the prior written
        ///permission of the owner.
        ///
        ///Contents:
        ///full IFC object model EXPRESS definitions for the IFC2x Edition 3 Technical Corrigendum 1 release
        ///
        ///- express longform distribution
        ///- compiled for EXPRESS version 1 technical corrigendum 2
        ///
        ///Issue date:
        ///July 10, 2007
        ///        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string IFC2X3_TC1 {
            get {
                return ResourceManager.GetString("IFC2X3_TC1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///Copyright by:
        ///buildingSMART International Ltd, 1996-2013
        ///Any technical documentation made available by buildingSMART International Limited 
        ///is the copyrighted work of buildingSMART International Limited and is owned by the 
        ///buildingSMART International Limited. It may be photocopied, used in software development, 
        ///or translated into another computer language without prior written consent from 
        ///buildingSMART International Limited provided that full attribution is given. 
        ///Prior written consent is re [rest of string was truncated]&quot;;.
        /// </summary>
        public static string IFC4 {
            get {
                return ResourceManager.GetString("IFC4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///Copyright by:
        ///buildingSMART International Limited, 1996-2015
        ///
        ///Any technical documentation made available by buildingSMART International Limited
        ///is the copyrighted work of buildingSMART International Limited and is owned by the 
        ///buildingSMART International Limited. It may be photocopied, used in software development, 
        ///or translated into another computer language without prior written consent from 
        ///buildingSMART International Limited provided that full attribution is given. 
        ///Prior written consent  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string IFC4_ADD1 {
            get {
                return ResourceManager.GetString("IFC4_ADD1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SCHEMA ISO13584_generic_expressions_schema;
        ///
        ///
        ///
        ///ENTITY generic_expression
        ///ABSTRACT SUPERTYPE OF(ONEOF(simple_generic_expression,
        ///								unary_generic_expression,
        ///								binary_generic_expression,
        ///								multiple_arity_generic_expression));
        ///WHERE 
        ///	WR1: is_acyclic(SELF);
        ///END_ENTITY;
        ///
        ///ENTITY simple_generic_expression
        ///ABSTRACT SUPERTYPE OF (ONEOF(generic_literal, generic_variable))
        ///SUBTYPE OF (generic_expression);
        ///END_ENTITY;
        ///
        ///ENTITY generic_literal
        ///ABSTRACT SUPERTYPE
        ///SUBTYPE OF (simple [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ISO13584_generic_expressions_schema {
            get {
                return ResourceManager.GetString("ISO13584_generic_expressions_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///2004-01-12
        ///ISO TC184/SC4/WG12 N2887 - ISO/IS 10303-41 Fundamentals of product description and support - EXPRESS
        ///*)
        ///
        ///SCHEMA application_context_schema;
        ///  REFERENCE FROM basic_attribute_schema (description_attribute, get_description_value, get_id_value, id_attribute);
        ///  REFERENCE FROM date_time_schema (year_number);
        ///  REFERENCE FROM support_resource_schema (identifier, label, text);
        ///  ENTITY application_context;
        ///    application : label;
        ///  DERIVE
        ///    description : text := get_description_value(S [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step41_application_context_schema {
            get {
                return ResourceManager.GetString("Step41_application_context_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  (* Document WG12 N4855 date 2007-05-08. EXPRESS listing corresponding to part
        /// 42  edition 3 IS as modified by TC1  - WG12 N4854*)
        /// SCHEMA geometry_schema;
        ///   REFERENCE FROM representation_schema
        ///     (definitional_representation,
        ///      founded_item,
        ///      functionally_defined_transformation,
        ///      item_in_context,
        ///      representation,
        ///      representation_item,
        ///      representation_context,
        ///      using_representations);
        ///   REFERENCE FROM measure_schema
        ///     (global_unit_assigned_context,
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step42_geometry_schema {
            get {
                return ResourceManager.GetString("Step42_geometry_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///ISO TC184/SC4/WG12 N4828 - ISO/CD 10303-43 Representation structures - EXPRESS
        ///*) 
        ///
        ///SCHEMA representation_schema;
        ///
        ///REFERENCE FROM basic_attribute_schema
        ///  (get_description_value,
        ///   get_id_value);
        ///
        ///REFERENCE FROM measure_schema
        ///  (measure_value,
        ///   measure_with_unit);
        ///
        ///REFERENCE FROM support_resource_schema
        ///  (bag_to_set,
        ///   identifier,
        ///   label,
        ///   text);
        ///
        ///  TYPE compound_item_definition = SELECT
        ///    (list_representation_item,
        ///     set_representation_item);
        ///  END_TYPE;
        ///
        ///  TYPE f [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step43_representation_schema {
            get {
                return ResourceManager.GetString("Step43_representation_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SCHEMA product_structure_schema;
        ///
        ///  REFERENCE FROM product_definition_schema
        ///    (product,
        ///     product_definition,
        ///     product_definition_relationship,
        ///     acyclic_product_definition_relationship);
        ///
        ///  REFERENCE FROM measure_schema
        ///     (measure_with_unit);
        ///
        ///  REFERENCE FROM support_resource_schema
        ///     (identifier, label, text);
        ///
        ///ENTITY alternate_product_relationship;
        ///  name        : label;
        ///  definition  : OPTIONAL text;
        ///  alternate   : product;
        ///  base        : product;
        ///  basis       : [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step44_product_structure_schema {
            get {
                return ResourceManager.GetString("Step44_product_structure_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*
        ///$Id: 10303-045e2.exp,v 1.1 2010/02/25 02:34:56 loffredo Exp $ 
        ///ISO TC184/SC4/WG12 N6751 - ISO/IS 10303-45 Material properties - EXPRESS
        ///Supersedes ISO TC184/SC4/WG12 N5100 - ISO/IS 10303-45 Material properties - EXPRESS
        ///changes to material_designation as per SEDS #1423.
        ///*)
        ///
        ///
        ///SCHEMA material_property_definition_schema;
        ///
        ///REFERENCE FROM material_property_representation_schema
        ///     (material_property_representation);
        ///
        ///REFERENCE FROM measure_schema
        ///     (measure_with_unit);
        ///
        ///REFERENCE FROM pro [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step45_material_property_definition_schema {
            get {
                return ResourceManager.GetString("Step45_material_property_definition_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (*  
        ///
        ///    The following EXPRESS is for ISO 10303-46:1994 Technical Corrigendum 2
        ///
        ///
        ///
        ///    This is document ISO TC184/SC4/WG12 N996
        ///
        ///*)
        ///
        ///
        ///
        ///SCHEMA presentation_organization_schema;
        ///
        ///
        ///REFERENCE FROM presentation_resource_schema
        ///   (colour,
        ///    planar_box,
        ///    planar_extent,
        ///    presentation_scaled_placement);
        ///
        ///
        ///REFERENCE FROM presentation_definition_schema
        ///
        ///    (annotation_occurrence,
        ///
        ///     symbol_representation,
        ///
        ///     symbol_representation_relationship);
        ///
        ///
        ///
        ///REFERENCE FROM present [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step46_presentation_organization_schema {
            get {
                return ResourceManager.GetString("Step46_presentation_organization_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (* 
        ///This is WG12 N548.  This incorporates the changes identified in ISO 10303-47 TC1 
        ///  into the EXPRESS of the document
        ///*)
        ///
        ///SCHEMA shape_aspect_definition_schema;
        ///REFERENCE FROM product_property_definition_schema
        ///    (shape_aspect,
        ///     shape_aspect_relationship);
        ///REFERENCE FROM measure_schema
        ///    (measure_with_unit);
        ///REFERENCE FROM support_resource_schema
        ///    (bag_to_set, label, identifier);
        ///
        ///TYPE limit_condition = ENUMERATION OF
        ///  (maximum_material_condition,
        ///   least_material_condition,        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step47_shape_aspect_definition_schema {
            get {
                return ResourceManager.GetString("Step47_shape_aspect_definition_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SCHEMA method_definition_schema;
        ///
        ///REFERENCE FROM action_schema
        ///    (action_method, 
        ///     action_method_relationship, 
        ///     action_relationship);
        ///
        ///REFERENCE FROM document_schema
        ///    (document, 
        ///     document_usage_constraint);
        ///
        ///REFERENCE FROM effectivity_schema
        ///    (effectivity);
        ///
        ///REFERENCE FROM measure_schema
        ///    (count_measure);
        ///
        ///REFERENCE FROM support_resource_schema
        ///    (label, 
        ///     text);
        ///
        ///REFERENCE FROM process_property_schema
        ///    (product_definition_process,
        ///     property_pro [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step49_method_definition_schema {
            get {
                return ResourceManager.GetString("Step49_method_definition_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (* WG12 N4886 Part 50 EXPRESS as modified by TC1 2007-07-25 (WG12 N4885) *)
        ///
        ///(* Genenerated from: ../../irs/wg12n921.exp *)
        ///
        ///SCHEMA mathematical_functions_schema;
        ///-- This file constitutes document WG12 N921
        ///-- Master document: ISO 10303-50:2001
        ///-- EXPRESS last modified: 2001-09-07
        ///
        ///REFERENCE FROM ISO13584_generic_expressions_schema     -- ISO 13584-20
        ///  (binary_generic_expression,
        ///   environment,
        ///   generic_expression,
        ///   generic_literal,
        ///   generic_variable,
        ///   multiple_arity_generic_express [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step50_mathematical_functions_schema {
            get {
                return ResourceManager.GetString("Step50_mathematical_functions_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (* THIS IS DOCUMENT ISO TC184/SC4/WG12 N2837 - EXPRESS CODE FOR THE 
        ///   IS VERSION OF ISO 10303-55: &apos;PROCEDURAL &amp; HYBRID REPRESENTATION&apos; *)
        ///
        ///SCHEMA procedural_model_schema;
        ///
        ///REFERENCE FROM support_resource_schema                  -- ISO 10303-41
        ///  (text);
        ///
        ///REFERENCE FROM representation_schema                    -- ISO 10303-43
        ///  (item_in_context,
        ///   representation,
        ///   representation_item,
        ///   representation_item_relationship,
        ///   representation_relationship,
        ///   using_representations);
        ///
        ///REFEREN [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step55_procedural_model_schema {
            get {
                return ResourceManager.GetString("Step55_procedural_model_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (* $Id: 10303-056.exp,v 1.1 2007/07/12 22:12:20 loffredo Exp $ 
        ///ISO TC184/SC4/WG12 N3207 - ISO/IS 10303-56 State - EXPRESS
        ///Supersedes ISO TC184/SC4/WG12 N2465
        ///*) 
        ///
        ///
        ///SCHEMA state_observed_schema;
        ///
        ///REFERENCE FROM state_type_schema   -- ISO 10303-56
        ///  (state_type); 
        ///
        ///REFERENCE FROM support_resource_schema   -- ISO 10303-41
        ///  (label,
        ///   text); 
        ///
        ///
        ///ENTITY ascribable_state;
        ///  name : label;
        ///  description : OPTIONAL text;
        ///  pertaining_state_type : state_type;
        ///  ascribed_state_observed : state_obs [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Step56_state_observed_schema {
            get {
                return ResourceManager.GetString("Step56_state_observed_schema", resourceCulture);
            }
        }
    }
}
