﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.ExpressParser;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators;
using XbimSchemaComparer.Comparators.SchemaComparers;

namespace XbimSchemaComparer
{
    /// <summary>
    /// This utility can be used to compare two similar schemas (schema versions).
    /// It was used to compare IFC2x3 and various versions of IFC4 as an independent
    /// identification of changes and also to estimate amount of changes and their impact
    /// between versions.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = @"c:\Users\Martin\Source\Repos\XbimEssentials\Schemas";

            var ifc2X3 = GetSchema(File.ReadAllText("IFC2X3_TC1.exp"), SchemaSources.IFC2x3_TC1);
            var ifc4Add1 = GetSchema(File.ReadAllText("IFC4_ADD1.exp"), SchemaSources.IFC4_ADD1);
            var ifc4Add2 = GetSchema(File.ReadAllText("IFC4_ADD2.exp"), SchemaSources.IFC4_ADD2);
            var ifc4 = GetSchema(File.ReadAllText("IFC4.exp"), SchemaSources.IFC4);
            var ifc4x1 = GetSchema(File.ReadAllText("IFC4x1_FINAL.exp"), SchemaSources.IFC4X1_FINAL);
            var ifc4x1Extension = GetSchema(File.ReadAllText("IFC4_ADD2.exp") + File.ReadAllText("IfcAlignmentExtension.exp"), SchemaSources.IFC4X1_FINAL + "_Extension");
            var ifc4x3_RC1 = GetSchema(File.ReadAllText("IFC4X3_RC1.exp"), SchemaSources.IFC4x3_RC1);
            var ifc4x3_RC2 = GetSchema(File.ReadAllText("IFC4X3_RC2.exp"), SchemaSources.IFC4x3_RC2);
            var ifcRail = GetSchema(File.ReadAllText(@"c:\Users\Martin\Source\IfcRail\schemas\v0.5\IFC_Rail_pilot_EXPRESS_V0.5.exp"), "IFCRail");
            var ifcBridge = GetSchema(File.ReadAllText(@"c:\Users\Martin\Downloads\BRie.exp"), "IFCBridge");

            // Compare( ifc2X3, ifc4Add1 );
            // Compare( ifc2X3, ifc4 );
            // Compare( ifc4Add1, ifc4Add2 );
            // Compare( ifc4, ifc4Add1);
            // Compare( ifc4x1Extension, ifcRail);
            // Compare( ifc4x1, ifcRail);
            // Compare(ifc4Add2, ifc4x1);
            // Compare(ifc4Add2, ifcRail);
            // Compare(ifc4Add2, ifcRail);
            // Compare(ifcRail, ifcBridge);
            // Compare( ifc4x1Extension, ifc4x3_RC1);
            Compare(ifc4x3_RC1, ifc4x3_RC2);

            Console.ReadLine();
        }

        private static string Schema(SchemaModel model)
        {
            return string.Join(", ", model.Get<SchemaDefinition>().Select(s => s.Name));
        }

        private static void Compare(SchemaModel modelA, SchemaModel modelB)
        {
            Console.WriteLine(@"Schemas to compare: {0}, {1}", modelA.FirstSchema.Source, modelB.FirstSchema.Source);
            var models = new [] { modelA, modelB };

            var schemaComparers = new ISchemaComparer[]
            {
                new AddedEntitiesComparer(), 
                new AddedSelectsComparer() , 
                new AddedTypesComparer(),
                new AddedEnumerationsComparer(), 
                new RemovedEntitiesComparer(), 
                new RemovedSelectsComparer(), 
                new RemovedTypesComparer(),
                new RemovedEnumerationsComparer(),
                new ChangedEntitiesComparer(),
                new ChangedSelectsComparer(),
                new ChangedEnumerationsComparer()
            };
            foreach (var comparer in schemaComparers)
            {
                comparer.Compare(modelA, modelB);
            }


            var w = new StringWriter();
            w.WriteLine("Number of entities:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<EntityDefinition>().Count());
            w.WriteLine();

            w.WriteLine("Number of non-abstract entities:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<EntityDefinition>().Count(e => e.Instantiable));
            w.WriteLine();

            w.WriteLine("Number of types:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<DefinedType>().Count());
            w.WriteLine();

            w.WriteLine("Number of enumerations:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<EnumerationType>().Count());
            w.WriteLine();

            w.WriteLine("Number of select types:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<SelectType>().Count());
            w.WriteLine();

            w.WriteLine("Number of global rules:");
            foreach (var model in models)
                w.WriteLine("{0}: {1}", Schema(model), model.Get<GlobalRule>().Count());
            w.WriteLine();

            foreach (var cmp in schemaComparers)
            {
                //filter only to specified entities
                var results = cmp.Results.ToList();//.Where(r => EntitiesToCheck.Contains(r.OldObjectName)).ToList();
                
                w.WriteLine("{0} ({1}):", cmp.Name, results.Count);
                Console.WriteLine(@"{0} ({1}):", cmp.Name, results.Count);
                foreach (var result in results)
                    w.WriteLine(result.Message);
                w.WriteLine();
            }

            var log = w.ToString();
            var logName = String.Format("{0}_{1}.txt", modelA.FirstSchema.Source, modelB.FirstSchema.Source);
            using (var file = File.CreateText(logName))
            {
                file.Write(log);
                file.Close();
            }
            Console.WriteLine(@"Comparison saved fo file: " + Path.Combine(Environment.CurrentDirectory, logName));
            Console.WriteLine();
        }

        private static SchemaModel GetSchema(string data, string source)
        {
            var parser = new ExpressParser();
            var result = parser.Parse(data, source);
            if (!result)
                throw new Exception("Error parsing schema file");
            return parser.SchemaInstance;
        }

        private static readonly List<string> EntitiesToCheck = new List<string>{
            "IfcOrganizationRelationship",
"IfcApprovalActorRelationship",
"IfcApprovalPropertyRelationship",
"IfcApprovalRelationship",
"IfcResourceApprovalRelationship",
"IfcPermeableCoveringProperties",
"IfcRelInteractionRequirements",
"IfcSpaceProgram",
"IfcTypeProduct",
"IfcDistributionElementType",
"IfcActuatorType",
"IfcAlarmType",
"IfcConstraintAggregationRelationship",
"IfcConstraintClassificationRelationship",
"IfcConstraintRelationship",
"IfcMetric",
"IfcObjective",
"IfcPropertyConstraintRelationship",
"IfcConstructionEquipmentResource",
"IfcConstructionMaterialResource",
"IfcConstructionProductResource",
"IfcCrewResource",
"IfcLaborResource",
"IfcSubContractResource",
"IfcPerformanceHistory",
"IfcRelAssociatesConstraint",
"IfcTimeSeriesSchedule",
"IfcAppliedValueRelationship",
"IfcCostValue",
"IfcCurrencyRelationship",
"IfcEnvironmentalImpactValue",
"IfcReferencesValueDocument",
"IfcCoordinatedUniversalTimeOffset",
"IfcDayInMonthNumber",
"IfcDaylightSavingHour",
"IfcHourInDay",
"IfcMinuteInHour",
"IfcMonthInYearNumber",
"IfcSecondInMinute",
"IfcDistributionElement",
"IfcElectricGeneratorType",
"IfcElectricMotorType",
"IfcElectricTimeControlType",
"IfcLampType",
"IfcMotorConnectionType",
"IfcActionRequest",
"IfcCondition",
"IfcConditionCriterion",
"IfcEquipmentStandard",
"IfcFurnitureStandard",
"IfcMove",
"IfcOrderAction",
"IfcPermit",
"IfcConnectionPointGeometry",
"IfcConnectionPointEccentricity",
"IfcConnectionPortGeometry",
"IfcGridPlacement",
"IfcVirtualGridIntersection",
"IfcBlock",
"IfcBoxedHalfSpace",
"IfcFacetedBrepWithVoids",
"IfcRectangularPyramid",
"IfcRightCircularCone",
"IfcRightCircularCylinder",
"IfcSectionedSpine",
"IfcSphere",
"IfcCartesianTransformationOperator2DnonUniform",
"IfcOffsetCurve2D",
"IfcOffsetCurve3D",
"IfcPointOnCurve",
"IfcPointOnSurface",
"IfcBezierCurve",
"IfcRationalBezierCurve",
"IfcRectangularTrimmedSurface",
"IfcAirToAirHeatRecoveryType",
"IfcCoilType",
"IfcCondenserType",
"IfcCooledBeamType",
"IfcEvaporativeCoolerType",
"IfcEvaporatorType",
"IfcFilterType",
"IfcGasTerminalType",
"IfcHeatExchangerType",
"IfcHumidifierType",
"IfcTubeBundleType",
"IfcVibrationIsolatorType",
"IfcFuelProperties",
"IfcGeneralMaterialProperties",
"IfcHygroscopicMaterialProperties",
"IfcMechanicalMaterialProperties",
"IfcMechanicalConcreteMaterialProperties",
"IfcExtendedMaterialProperties",
"IfcMechanicalSteelMaterialProperties",
"IfcOpticalMaterialProperties",
"IfcProductsOfCombustionProperties",
"IfcRelaxation",
"IfcThermalMaterialProperties",
"IfcWaterProperties",
"IfcComplexNumber",
"IfcElectricChargeMeasure",
"IfcElectricConductanceMeasure",
"IfcElectricResistanceMeasure",
"IfcStackTerminalType",
"IfcBlobTexture",
"IfcExternallyDefinedHatchStyle",
"IfcFillAreaStyleTiles",
"IfcFillAreaStyleTileSymbolWithStyle",
"IfcImageTexture",
"IfcPixelTexture",
"IfcSpecularRoughness",
"IfcSymbolStyle",
"IfcTextAlignment",
"IfcTextDecoration",
"IfcTextStyleWithBoxCharacteristics",
"IfcTextTransformation",
"IfcAnnotationCurveOccurrence",
"IfcAnnotationSurface",
"IfcAnnotationSymbolOccurrence",
"IfcBoxAlignment",
"IfcTextureCoordinateGenerator",
"IfcTextureMap",
"IfcTextureVertex",
"IfcVertexBasedTextureMap",
"IfcDraughtingCallout",
"IfcDimensionCurveDirectedCallout",
"IfcAngularDimension",
"IfcDiameterDimension",
"IfcDraughtingCalloutRelationship",
"IfcDimensionCalloutRelationship",
"IfcDimensionCurve",
"IfcTerminatorSymbol",
"IfcDimensionCurveTerminator",
"IfcDimensionPair",
"IfcLinearDimension",
"IfcPreDefinedSymbol",
"IfcPreDefinedDimensionSymbol",
"IfcPreDefinedPointMarkerSymbol",
"IfcPreDefinedTerminatorSymbol",
"IfcProjectionCurve",
"IfcRadiusDimension",
"IfcStructuredDimensionCallout",
"IfcLightDistributionData",
"IfcLightIntensityDistribution",
"IfcLightSourceAmbient",
"IfcLightSourceDirectional",
"IfcLightSourceGoniometric",
"IfcLightSourcePositional",
"IfcLightSourceSpot",
"IfcDraughtingPreDefinedTextFont",
"IfcExternallyDefinedTextFont",
"IfcFontStyle",
"IfcFontVariant",
"IfcFontWeight",
"IfcPlanarBox",
"IfcPresentableText",
"IfcTextFontName",
"IfcProcedure",
"IfcWorkControl",
"IfcReinforcementBarProperties",
"IfcRibPlateProfileProperties",
"IfcSectionProperties",
"IfcSectionReinforcementProperties",
"IfcStructuralProfileProperties",
"IfcStructuralSteelProfileProperties",
"IfcSoundProperties",
"IfcAccelerationMeasure",
"IfcAmountOfSubstanceMeasure",
"IfcAngularVelocityMeasure",
"IfcContextDependentMeasure",
"IfcCurvatureMeasure",
"IfcDescriptiveMeasure",
"IfcBoolean",
"IfcDoseEquivalentMeasure",
"IfcDynamicViscosityMeasure",
"IfcElectricCapacitanceMeasure",
"IfcElectricCurrentMeasure",
"IfcElectricVoltageMeasure",
"IfcEnergyMeasure",
"IfcForceMeasure",
"IfcFrequencyMeasure",
"IfcHeatFluxDensityMeasure",
"IfcHeatingValueMeasure",
"IfcIdentifier",
"IfcIlluminanceMeasure",
"IfcInductanceMeasure",
"IfcInteger",
"IfcIntegerCountRateMeasure",
"IfcIonConcentrationMeasure",
"IfcIsothermalMoistureCapacityMeasure",
"IfcKinematicViscosityMeasure",
"IfcLabel",
"IfcLinearForceMeasure",
"IfcLinearMomentMeasure",
"IfcLinearStiffnessMeasure",
"IfcLinearVelocityMeasure",
"IfcLogical",
"IfcLuminousFluxMeasure",
"IfcLuminousIntensityDistributionMeasure",
"IfcLuminousIntensityMeasure",
"IfcMagneticFluxDensityMeasure",
"IfcMagneticFluxMeasure",
"IfcMassDensityMeasure",
"IfcMassFlowRateMeasure",
"IfcMassPerLengthMeasure",
"IfcModulusOfElasticityMeasure",
"IfcModulusOfLinearSubgradeReactionMeasure",
"IfcModulusOfRotationalSubgradeReactionMeasure",
"IfcModulusOfSubgradeReactionMeasure",
"IfcMoistureDiffusivityMeasure",
"IfcMolecularWeightMeasure",
"IfcMomentOfInertiaMeasure",
"IfcNumericMeasure",
"IfcParameterValue",
"IfcPHMeasure",
"IfcPlanarForceMeasure",
"IfcPositivePlaneAngleMeasure",
"IfcPowerMeasure",
"IfcPressureMeasure",
"IfcRadioActivityMeasure",
"IfcReal",
"IfcRotationalFrequencyMeasure",
"IfcRotationalMassMeasure",
"IfcRotationalStiffnessMeasure",
"IfcSectionalAreaIntegralMeasure",
"IfcSectionModulusMeasure",
"IfcShearModulusMeasure",
"IfcSolidAngleMeasure",
"IfcSoundPowerMeasure",
"IfcSoundPressureMeasure",
"IfcSpecificHeatCapacityMeasure",
"IfcTemperatureGradientMeasure",
"IfcText",
"IfcPlaneAngleMeasure",
"IfcThermalAdmittanceMeasure",
"IfcThermalConductivityMeasure",
"IfcThermalExpansionCoefficientMeasure",
"IfcThermalResistanceMeasure",
"IfcThermalTransmittanceMeasure",
"IfcThermodynamicTemperatureMeasure",
"IfcTorqueMeasure",
"IfcVaporPermeabilityMeasure",
"IfcVolumetricFlowRateMeasure",
"IfcWarpingConstantMeasure",
"IfcWarpingMomentMeasure",
"IfcSpecularExponent",
"IfcDistributionChamberElement",
"IfcDistributionChamberElementType",
"IfcEnergyProperties",
"IfcElectricalBaseProperties",
"IfcFluidFlowProperties",
"IfcRelFlowControlElements",
"IfcSoundValue",
"IfcSpaceThermalLoadProperties",
"IfcChamferEdgeFeature",
"IfcRoundedEdgeFeature",
"IfcAsset",
"IfcInventory",
"IfcActor",
"IfcRelAssignsToActor",
"IfcServiceLife",
"IfcServiceLifeFactor",
"IfcCostItem",
"IfcCostSchedule",
"IfcProjectOrder",
"IfcProjectOrderRecord",
"IfcRelAssignsToProjectOrder",
"IfcRelAssociatesAppliedValue",
"IfcRelSchedulesCostItems",
"IfcRelAssociatesProfileProperties",
"IfcRelConnectsStructuralMember",
"IfcRelConnectsWithEccentricity",
"IfcStructuralAnalysisModel",
"IfcStructuralCurveConnection",
"IfcStructuralCurveMember",
"IfcStructuralCurveMemberVarying",
"IfcStructuralLinearAction",
"IfcStructuralLinearActionVarying",
"IfcStructuralLoadGroup",
"IfcStructuralPlanarAction",
"IfcStructuralPlanarActionVarying",
"IfcStructuralPointAction",
"IfcStructuralPointConnection",
"IfcStructuralPointReaction",
"IfcStructuralResultGroup",
"IfcStructuralSurfaceConnection",
"IfcStructuralSurfaceMember",
"IfcStructuralSurfaceMemberVarying",
"IfcReinforcementDefinitionProperties",
"IfcReinforcingBar",
"IfcReinforcingMesh",
"IfcTendon",
"IfcTendonAnchor",
"IfcBoundaryEdgeCondition",
"IfcBoundaryFaceCondition",
"IfcBoundaryNodeCondition",
"IfcBoundaryNodeConditionWarping",
"IfcFailureConnectionCondition",
"IfcSlippageConnectionCondition",
"IfcStructuralLoadLinearForce",
"IfcStructuralLoadPlanarForce",
"IfcStructuralLoadSingleDisplacement",
"IfcStructuralLoadSingleDisplacementDistortion",
"IfcStructuralLoadSingleForce",
"IfcStructuralLoadSingleForceWarping",
"IfcStructuralLoadTemperature",
"IfcIrregularTimeSeries",
"IfcIrregularTimeSeriesValue",
"IfcRegularTimeSeries",
"IfcTimeSeriesReferenceRelationship",
"IfcTimeSeriesValue",
"IfcExternallyDefinedSurfaceStyle",
"IfcOneDirectionRepeatFactor",
"IfcSurfaceStyleRefraction",
"IfcSurfaceStyleWithTextures",
"IfcTwoDirectionRepeatFactor",
"IfcDefinedSymbol",
"IfcExternallyDefinedSymbol",
"IfcAsymmetricIShapeProfileDef",
"IfcCenterLineProfileDef",
"IfcCraneRailAShapeProfileDef",
"IfcCraneRailFShapeProfileDef",
"IfcCShapeProfileDef",
"IfcSurfaceOfRevolution",
"IfcAreaMeasure",
"IfcCompoundPlaneAngleMeasure",
"IfcCountMeasure",
"IfcMassMeasure",
"IfcMonetaryMeasure",
"IfcNormalisedRatioMeasure",
"IfcPositiveRatioMeasure",
"IfcRatioMeasure",
"IfcTimeStamp",
"IfcVolumeMeasure",
"IfcNullStyle",
"IfcProjectionElement",
"IfcRelConnectsElements",
"IfcRelConnectsWithRealizingElements",
"IfcRelCoversBldgElements",
"IfcRelProjectsElement",
"IfcRoundedRectangleProfileDef",
"IfcTrapeziumProfileDef",
"IfcZShapeProfileDef",
"IfcPhysicalComplexQuantity",
"IfcQuantityCount",
"IfcQuantityTime",
"IfcRelCoversSpaces",
"IfcRelReferencedInSpatialStructure",
"IfcRampFlightType",
"IfcRelConnectsStructuralActivity",
"IfcRelConnectsStructuralElement",
"IfcYearNumber",
"IfcClassificationItem",
"IfcClassificationItemRelationship",
"IfcClassificationNotation",
"IfcClassificationNotationFacet",
"IfcDocumentElectronicFormat",
"IfcDocumentInformationRelationship",
"IfcLibraryInformation",
"IfcLibraryReference",
"IfcDimensionCount",
"IfcProxy",
"IfcRelAssignsToResource",
"IfcRelAssociatesLibrary",
"IfcRelOverridesProperties",
"IfcLengthMeasure",
"IfcAbsorbedDoseMeasure",
"IfcPositiveLengthMeasure",
"IfcTimeMeasure",
"IfcPropertyDependencyRelationship",
"IfcPropertyTableValue",
"IfcRepresentationContext",
"IfcShapeAspect",
"IfcTopologyRepresentation",
"IfcEdge",
"IfcEdgeCurve",
"IfcLoop",
"IfcEdgeLoop",
"IfcFaceSurface",
"IfcOrientedEdge",
"IfcPath",
"IfcSubedge",
"IfcVertex",
"IfcVertexLoop",
"IfcVertexPoint",
"IfcGloballyUniqueId",
"IfcTable",
"IfcTableRow"}; 
    }
}
