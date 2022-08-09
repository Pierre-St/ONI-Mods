using PeterHan.PLib.Buildings;
using Klei.AI;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
namespace PierreStirnweiss.DangerousWorld
{
    /// <summary>
    /// Building used to manufacture all sorts of prosthetics.
    /// </summary>
    public sealed class DangerousWorldProstheticsFabricatorConfig : IBuildingConfig
    {
        // The building template
        internal static PBuilding ProstheticsFabricator;

        // The building ID
        internal const string ID = "DW_ProstheticsFabricator";



        // Register building
        internal static PBuilding CreateBuilding()
        {
            return ProstheticsFabricator = new PBuilding(ID, DangerousWorldStrings.PROSTHETICS_FABRICATOR_NAME)
            {
                AddAfter = "SuitFabricator",
                Animation = "prosthetics_fabricator_kanim",
                Category = "EQUIPMENT",
                ConstructionTime = 240.0f,
                Decor = TUNING.BUILDINGS.DECOR.NONE,
                Description = DangerousWorldStrings.PROSTHETICS_FABRICATOR_DESC,
                EffectText = DangerousWorldStrings.PROSTHETICS_FABRICATOR_EFFECT,
                Entombs = true,
                Floods = true,
                HeatGeneration = 0f,
                Height = 3,
                HP = 100,
                Ingredients = {
                    new BuildIngredient(TUNING.MATERIALS.REFINED_METAL, tier: 4),
                    new BuildIngredient(TUNING.MATERIALS.PLASTIC, tier: 2)
                },
                OverheatTemperature = 75.0f + Constants.CELSIUS2KELVIN,
                Placement = BuildLocationRule.OnFloor,
                RotateMode = PermittedRotations.Unrotatable,
                PowerInput = new PowerRequirement(480.0f, new CellOffset(0, 0)),
                SceneLayer = Grid.SceneLayer.Building,
                Tech = "Suits",
                Width = 4
            };
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<Prioritizable>();
            ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
            fabricator.resultState = ComplexFabricator.ResultState.Heated;
            fabricator.heatedTemperature = 318.15f;
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_suit_fabricator_kanim")
            };
            Prioritizable.AddRef(go);
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
            this.ConfigureRecipes();
        }

        private void ConfigureRecipes()
        {
            ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[2] { new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 5f), new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 1f) };
            ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[1] { new ComplexRecipe.RecipeElement("DW_Splinter".ToTag(), 1f) };
            AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("DW_ProstheticsFabricator", (IList<ComplexRecipe.RecipeElement>)ingredients1, (IList<ComplexRecipe.RecipeElement>)results1), ingredients1, results1)
            {
                time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
                description = DangerousWorldStrings.PROSTHETICS_SPLINTER_RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    (Tag) "DW_ProstheticsFabricator"
                },
                requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
            };
/*            
            ComplexRecipe.RecipeElement[] ingredients2 = new ComplexRecipe.RecipeElement[2] { new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f), new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f) };
            ComplexRecipe.RecipeElement[] results2 = new ComplexRecipe.RecipeElement[1] { new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f) };
            AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>)ingredients2, (IList<ComplexRecipe.RecipeElement>)results2), ingredients2, results2)
            {
                time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
                description = (string)STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    (Tag) "SuitFabricator"
                },
                requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
            };
            ComplexRecipe.RecipeElement[] ingredients3 = new ComplexRecipe.RecipeElement[2] { new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f), new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f) };
            ComplexRecipe.RecipeElement[] results3 = new ComplexRecipe.RecipeElement[1] { new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f) };
            AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>)ingredients3, (IList<ComplexRecipe.RecipeElement>)results3), ingredients3, results3)
            {
                time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
                description = (string)STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    (Tag) "SuitFabricator"
                },
                requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
            };
            ComplexRecipe.RecipeElement[] ingredients4 = new ComplexRecipe.RecipeElement[2] { new ComplexRecipe.RecipeElement((Tag)SimHashes.Steel.ToString(), 200f), new ComplexRecipe.RecipeElement((Tag)SimHashes.Petroleum.ToString(), 25f) };
            ComplexRecipe.RecipeElement[] results4 = new ComplexRecipe.RecipeElement[1] { new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f) };
            JetSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>)ingredients4, (IList<ComplexRecipe.RecipeElement>)results4), ingredients4, results4)
            {
                time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
                description = (string)STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    (Tag) "SuitFabricator"
                },
                requiredTech = Db.Get().TechItems.jetSuit.parentTech.Id
            };
        */

/*}

        public override BuildingDef CreateBuildingDef()
        {
            return ProstheticsFabricator?.CreateDef();
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            ProstheticsFabricator?.CreateLogicPorts(go);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            ProstheticsFabricator?.CreateLogicPorts(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            ProstheticsFabricator?.DoPostConfigureComplete(go);
        }
    }

    public sealed class DangerousWorldSplinterConfig : IEquipmentConfig
    {
        public const string ID = "DW_Splinter";
        public static ComplexRecipe recipe;
        public static int SPLINTER_MASS = 5;
        private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasAtmoSuit;
//        private AttributeModifier expertAthleticsModifier;

        public EquipmentDef CreateEquipmentDef()
        {
            List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
            AttributeModifiers.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, DangerousWorldStrings.PROSTHETICS_SPLINTER_NAME, false, false, true));
//            this.expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)-TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name, false, false, true);
            EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(ID, TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, (float)DangerousWorldSplinterConfig.SPLINTER_MASS, "suit_oxygen_kanim", string.Empty, "body_oxygen_kanim", 6, AttributeModifiers, (string)null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, (Tag[]) null, (string)null);
            equipmentDef.RecipeDescription = DangerousWorldStrings.PROSTHETICS_SPLINTER_RECIPE_DESC;
            //            equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
            //            equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
//            equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
            if (Db.Get().effects.Get("DangerousWorldCrippled") != null)
                equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("DangerousWorldCrippled"));
            equipmentDef.OnEquipCallBack = (System.Action<Equippable>)(eq =>
            {
/*                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
//                Navigator component1 = targetGameObject.GetComponent<Navigator>();
//                if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
//                    component1.SetFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
//                MinionResume component2 = targetGameObject.GetComponent<MinionResume>();
//                if (!((UnityEngine.Object)component2 != (UnityEngine.Object)null) || !component2.HasPerk((HashedString)Db.Get().SkillPerks.ExosuitExpertise.Id))
//                    return;
//                targetGameObject.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(this.expertAthleticsModifier);
*/

/*});
            equipmentDef.OnUnequipCallBack = (System.Action<Equippable>)(eq =>
            {
/*                if (eq.assignee == null)
                    return;
                Ownables soleOwner = eq.assignee.GetSoleOwner();
                if (!((UnityEngine.Object)soleOwner != (UnityEngine.Object)null))
                    return;
                GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
                if ((bool)((UnityEngine.Object)targetGameObject))
                {
                    targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(this.expertAthleticsModifier);
                    Navigator component1 = targetGameObject.GetComponent<Navigator>();
                    if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        component1.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
                    Effects component2 = targetGameObject.GetComponent<Effects>();
                    if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.HasEffect("SoiledSuit"))
                        component2.Remove("SoiledSuit");
                }
                eq.GetComponent<Storage>().DropAll(eq.transform.GetPosition(), true, true, new Vector3(), false);
*/  

/*});
//            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Atmo_Suit");
//            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
            return equipmentDef;
        }

        public void DoPostConfigure(GameObject go)
        {
//            SuitTank suitTank = go.AddComponent<SuitTank>();
//            suitTank.element = "Oxygen";
//            suitTank.capacity = 75f;
//            go.AddComponent<HelmetController>();
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.MedicalSupplies, false);
//            component.AddTag(GameTags.Clothes, false);
//            component.AddTag(GameTags.PedestalDisplayable, false);
//            component.AddTag(GameTags.AirtightSuit, false);
//            Storage storage = go.AddOrGet<Storage>();
//            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
//            storage.showInUI = true;
//            go.AddOrGet<AtmoSuit>();
//            go.AddComponent<SuitDiseaseHandler>();
        }
    }
}
*/