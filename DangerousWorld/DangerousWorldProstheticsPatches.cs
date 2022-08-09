/*using Harmony;
using Klei.AI;
using PeterHan.PLib;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PierreStirnweiss.DangerousWorld
{
    class DangerousWorldProstheticsPatches
    {
        [HarmonyPatch(typeof(GeneratedEquipment), "LoadGeneratedEquipment")]
        public class GeneratedEquipment_LoadGeneratedEquipment_Patch
        {
            internal static void Prefix()
            {
                Strings.Add("STRINGS.EQUIPMENT.PREFABS.DW_SPLINTER.NAME", DangerousWorldStrings.PROSTHETICS_SPLINTER_NAME);
                Strings.Add("STRINGS.EQUIPMENT.PREFABS.DW_SPLINTER.DESC", DangerousWorldStrings.PROSTHETICS_SPLINTER_DESC);
                Strings.Add("STRINGS.EQUIPMENT.PREFABS.DW_SPLINTER.EFFECT", DangerousWorldStrings.PROSTHETICS_SPLINTER_EFFECT);
                Strings.Add("STRINGS.EQUIPMENT.PREFABS.DW_SPLINTER.RECIPE_DESC", DangerousWorldStrings.PROSTHETICS_SPLINTER_RECIPE_DESC);
                Strings.Add("STRINGS.EQUIPMENT.PREFABS.DW_SPLINTER.GENERICNAME", DangerousWorldStrings.PROSTHETICS_SPLINTER_GENERICNAME);
            }
            internal static void Postfix()
            {
                EquipmentConfigManager.Instance.RegisterEquipment((IEquipmentConfig)new DangerousWorldSplinterConfig());
            }
        }
/*
        [HarmonyPatch(typeof(MedicalCotConfig), "DoPostConfigureComplete")]
        public class MedicalCotConfig_DoPostConfigureComplete_Patch
        {
            private static Tag SUPPLY_TAG = (Tag)"DW_Splinter";
            internal static void Postfix(GameObject go)
            {
                Storage storage = go.AddOrGet<Storage>();
                storage.showInUI = true;
                ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
                manualDeliveryKg.SetStorage(storage);
                manualDeliveryKg.requestedItemTag = MedicalCotConfig_DoPostConfigureComplete_Patch.SUPPLY_TAG;
                manualDeliveryKg.capacity = 10f;
                manualDeliveryKg.refillMass = 5f;
                manualDeliveryKg.minimumMass = 1f;
                manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.DoctorFetch.IdHash;
                manualDeliveryKg.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
            }
        }
*/

/*
        [HarmonyPatch(typeof(Clinic), "CanAutoAssignTo")]
        public class Clinic_CanAutoAssignTo_Patch
        {
            internal static void Postfix(ref bool __result, Clinic __instance, MinionAssignablesProxy worker)
            {
                bool canCureCrippled = false;
                /*Storage storage = __instance.gameObject.GetComponent<Storage>();
                PUtil.LogDebug(("Clinic canAutoAssign. GameObject.storage: {0}").F(storage));
                if (storage)
                {
                    foreach (GameObject go in __instance.GetComponent<Storage>().items)
                    {
                        if (go && go.HasTag(GameTags.MedicalSupplies))
                        {
                            Tag tag = go.PrefabID();
                            if (tag == (Tag)"DW_Splinter")
                                canCureCrippled = true;
                        }
                    }
                }*/

/*
                MinionIdentity target = worker.target as MinionIdentity;
                Effects effects = target.GetComponent<Effects>();
                //PUtil.LogDebug(("Clinic CanAutoAssign. effects: {0}").F(effects));
                bool isCrippled = effects.HasEffect("DangerousWorldCrippled");
                //PUtil.LogDebug(("Clinic CanAutoAssignPatch:  canCure {0}, hasCrippled: {1}").F(canCureCrippled, isCrippled));

                if (__result || isCrippled)
                {
                    PUtil.LogDebug(("can assign for: {0}").F(target.name));
                    //__result = true;
                }
                else
                {
                    PUtil.LogDebug(("cannot assign for: {0}").F(target.name));
                    //__result = false;
                }
                //__result = __result || (isCrippled /*& canCureCrippled*/
/*);
             }
        }

        [HarmonyPatch(typeof(Clinic), "CanManuallyAssignTo")]
        public class Clinic_CanManuallyAssignTo_Patch
        {
            internal static void Postfix(ref bool __result, Clinic __instance, MinionAssignablesProxy worker)
            {
                bool canCureCrippled = false;
                /*
                foreach (GameObject go in __instance.GetComponent<Storage>().items)
                {
                    if (go.HasTag(GameTags.MedicalSupplies))
                    {
                        Tag tag = go.PrefabID();
                        if (tag == (Tag)"DW_Splinter")
                            canCureCrippled = true;
                    }
                }*/
/*
                MinionIdentity target = worker.target as MinionIdentity;
                Effects effects = target.GetComponent<Effects>();
                //PUtil.LogDebug(("Clinic CanManuallyAssign. effects: {0}").F(effects));
                bool isCrippled = effects.HasEffect("DangerousWorldCrippled");
                //PUtil.LogDebug(("Clinic CanManuallyAssignPatch:  canCure {0}, hasCrippled: {1}").F(canCureCrippled, isCrippled));
                //PUtil.LogDebug(("Clinic CanManuallyAssignPatch: result modif: {0}").F(isCrippled & canCureCrippled));

                PUtil.LogDebug(("Clinic manuallyassign. result : {0}").F(__result ));
                PUtil.LogDebug(("Clinic manuallyassign. result | isCrippled crippled: {0}").F((__result || isCrippled)));

                if (__result || isCrippled)
                {
                    PUtil.LogDebug(("can assign for: {0}").F(target.name));
                    //__result = true;
                }
                else
                {
                    PUtil.LogDebug(("cannot assign for: {0}").F(target.name));
                    //__result = false;
                }
                //__result = __result || (isCrippled /*& canCureCrippled*/
/*);
            }
        }

        [HarmonyPatch(typeof(Clinic), "OnCompleteWork")]
        public class Clinic_OnCompleteWork_Patch
        {
            internal static void Postfix(Clinic __instance, Worker worker)
            {
                bool canCureCrippled = false;
                /*foreach (GameObject go in __instance.GetComponent<Storage>().items)
                {
                    if (go.HasTag(GameTags.MedicalSupplies))
                    {
                        Tag tag = go.PrefabID();
                        if (tag == (Tag)"DW_Splinter")
                            canCureCrippled = true;
                    }
                }*/
 /*               bool isCrippled = worker.GetComponent<Effects>().HasEffect("DangerousWorldCrippled");

                if (/*canCureCrippled &&*/ 
 /*isCrippled)
                {
                    Effects component = worker.GetComponent<Effects>();
                    component.Remove("DangerousWorldCrippled");
                }    
            }
        }
    }
}
 */