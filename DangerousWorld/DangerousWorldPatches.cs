using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using PeterHan.PLib.AVC;
using PeterHan.PLib.PatchManager;
using PeterHan.PLib.Buildings;
using PeterHan.PLib.Options;
using Klei.AI;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PierreStirnweiss.DangerousWorld {
    /// <summary>
    /// Patches which will be applied via annotations for DangerousWorld.
    /// </summary>
    public sealed class DangerousWorldPatches : KMod.UserMod2
    {
        internal static DangerousWorldOptions Options { get; private set; }
/*
        /// <summary>
        /// Loads settings when the mod starts up.
        /// </summary>
        [PLibMethod(RunAt.OnStartGame)]
        internal static void LoadSettings()
        {
            var newOptions = POptions.ReadSettings<DangerousWorldOptions>();
            if (newOptions != null)
                Options = newOptions;

            DangerousWorldOptions.Instance.InitialiseOptions();
            ///            PUtil.LogDebug("DangerousWorld settings: {0}".F(Options.?????));
        }
*/
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            PUtil.InitLibrary(true);
            //new PBuildingManager().Register(DangerousWorldProstheticsFabricatorConfig.CreateBuilding());

            PUtil.LogDebug("OnLoad PLib initialised");

            Options = new DangerousWorldOptions();
            new POptions().RegisterOptions(this, typeof(DangerousWorldOptions));
            new PPatchManager(harmony).RegisterPatchClass(typeof(DangerousWorldPatches));
            new PVersionCheck().Register(this, new SteamVersionChecker());

            DangerousWorldOptions.Instance.InitialiseOptions();

            PUtil.LogDebug("OnLoad base end");
        }

        /// <summary>
        /// Initialise Db with common elements.
        /// Also handles localization by registering for translation.
        /// </summary>
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            /// <summary>
            /// Applied before Initialize runs.
            /// </summary>
            internal static void Prefix()
            {
                /// _____________________________ Localisation stuff ______________________________________
#if DEBUG
                /// ModUtil.RegisterForTranslation(typeof(DangerousWorldStrings));
#else
				/// Localization.RegisterForTranslation(typeof(DangerousWorldStrings));
#endif
            }

            internal static void Postfix(Db __instance)
            {
                /// ____________________________ New effects used throughout the mod ________________________

                PUtil.LogDebug("DB initialise postfix");
                Effect crippledEffect = new Effect("DangerousWorldCrippled", (String)DangerousWorldStrings.CRIPPLED_EFFECT_TITLE, (string)DangerousWorldStrings.CRIPPLED_EFFECT_DESC, (float)(DangerousWorldOptions.Instance.fallOptions.crippledDuration * 600), true, true, true);
                crippledEffect.Add(new AttributeModifier("Athletics", DangerousWorldOptions.Instance.fallOptions.crippledModifier, (string)DangerousWorldStrings.CRIPPLED_EFFECT_TITLE, false));
                __instance.effects.Add(crippledEffect);

#if DEBUG
                Effect test;
                AttributeModifier attributeModifier;

                for (int idx = 0; idx < __instance.effects.Count; ++idx)
                {
                    test = (Effect)__instance.effects.GetResource(idx);
                    PUtil.LogDebug(("Effect {0}: id: {1}, name: {2}").F(idx, __instance.effects.GetResource(idx).Id, __instance.effects.GetResource(idx).Id));
                    for (int idx1 = 0; idx1 < test.SelfModifiers.Count; ++idx1)
                    {
                        attributeModifier = test.SelfModifiers[idx1];
                        PUtil.LogDebug(("Effect attribute modifier: {0}, attribute: {1}, value: {2}").F(attributeModifier.Description, attributeModifier.AttributeId, attributeModifier.Value));
                    }
                }
#endif
            }
        }
    }
}
