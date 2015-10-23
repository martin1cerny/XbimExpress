using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.ExpressParser;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators;
using XbimSchemaComparer.Comparators.SchemaComparers;

namespace XbimSchemaComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ifc2X3 = GetSchema(Schemas.IFC2X3_TC1);
            var ifc4Add1 = GetSchema(Schemas.IFC4_ADD1);
            var ifc4 = GetSchema(Schemas.IFC4);

            //Compare( ifc2X3, ifc4Add1 );
            Compare( ifc2X3, ifc4 );
            //Compare( ifc4, ifc4Add1);

            Console.ReadLine();
        }

        private static void Compare(SchemaModel modelA, SchemaModel modelB)
        {
            Console.WriteLine(@"Schemas to compare: {0}, {1}", modelA.FirstSchema.Name, modelB.FirstSchema.Name);
            var schemas = new List<SchemaDefinition> { modelA.FirstSchema, modelB.FirstSchema };

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
                new ChangedEntitiesComparer()
            };
            foreach (var comparer in schemaComparers)
            {
                comparer.Compare(modelA.FirstSchema, modelB.FirstSchema);
            }


            var w = new StringWriter();
            w.WriteLine("Number of entities:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count());
            w.WriteLine();

            w.WriteLine("Number of non-abstract entities:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count(e => e.Instantiable));
            w.WriteLine();

            w.WriteLine("Number of types:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Types.Count());
            w.WriteLine();

            w.WriteLine("Number of enumerations:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Enumerations.Count());
            w.WriteLine();

            w.WriteLine("Number of select types:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.SelectTypes.Count());
            w.WriteLine();

            w.WriteLine("Number of global rules:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.GlobalRules.Count());
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
            var logName = String.Format("{0}_{1}.txt", modelA.FirstSchema.Name, modelB.FirstSchema.Name);
            using (var file = File.CreateText(logName))
            {
                file.Write(log);
                file.Close();
            }
            Console.WriteLine(@"Comparison saved fo file: " + Path.Combine(Environment.CurrentDirectory, logName));
            Console.WriteLine();
        }

        private static SchemaModel GetSchema(string data)
        {
            var parser = new ExpressParser();
            var result = parser.Parse(data);
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
