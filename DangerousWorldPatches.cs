﻿using Harmony;
using PeterHan.PLib;
using Klei.AI;
using UnityEngine;
using System;
using System.Collections.Generic;


namespace PierreStirnweiss.DangerousWorld {
    /// <summary>
    /// Patches which will be applied via annotations for DangerousWorld.
    /// </summary>
    public class DangerousWorldPatches
    {
        private static IDictionary<GameObject, int> dupesDict = new Dictionary<GameObject, int>(64);

        public static void OnLoad()
        {
            PUtil.InitLibrary();
        }

        /// <summary>
        /// Handles localization by registering for translation.
        /// </summary>
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            /// <summary>
            /// Applied before Initialize runs.
            /// </summary>
            internal static void Prefix()
            {
#if DEBUG
                /// ModUtil.RegisterForTranslation(typeof(DangerousWorldStrings));
#else
				/// Localization.RegisterForTranslation(typeof(DangerousWorldStrings));
#endif
            }

            internal static void Postfix(Db __instance)
            {
                PUtil.LogDebug("Postfix Db initialise");

                Effect crippledEffect = new Effect("DangerousWorldCrippled", (String)DangerousWorldStrings.CRIPPLED_EFFECT_TITLE, (string)DangerousWorldStrings.CRIPPLED_EFFECT_DESC, 6000.0f, true, true, true);
                crippledEffect.Add(new AttributeModifier("Athletics", -20, (string)DangerousWorldStrings.CRIPPLED_EFFECT_TITLE, false));
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

        /// <summary>
        /// Applied to AttemptInitialRecovery, which determines if the dupe can hop on something of if he miserably falls.
        /// </summary>
        [HarmonyPatch(typeof(FallMonitor.Instance), "AttemptInitialRecovery")]
        public class FallMonitorInstance_AttemptInitialRecovery_Patch
        {
            /// <summary>
            /// Applied after AttemptInitialRecovery runs. Sets the variables depending on whether or not the dupe is falling.
            /// </summary>
            /// <param name="__instance">The current FallMonitor.Instance.</param>
            /// <param name="__navigator">The current FallMonitor.Instance.navigator.</param>
            internal static void Postfix(FallMonitor.Instance __instance, Navigator ___navigator)
            {
                if (__instance.gameObject.HasTag(GameTags.Minion))
                {
#if DEBUG
                    PUtil.LogDebug(("AttemptRecovery: {0} state: {1}").F(__instance.gameObject.name, __instance.GetCurrentState().name));
#endif
                    if (!dupesDict.ContainsKey(__instance.gameObject))
                    {
                        dupesDict.Add(__instance.gameObject, -1);
                    }
                    if (__instance.GetCurrentState() == __instance.sm.falling_pre)
                    {
#if DEBUG
                        PUtil.LogDebug(("AttemptRecovery: {0} is falling").F(__instance.gameObject.name));
#endif
                        dupesDict[__instance.gameObject] = Grid.PosToCell((KMonoBehaviour)___navigator);
#if DEBUG
                        PUtil.LogDebug(("falling from: {0}").F(dupesDict[__instance.gameObject]));
#endif
                    }
                    else
                    {
#if DEBUG
                        PUtil.LogDebug(("AttemptRecovery: {0} recovered").F(__instance.gameObject.name));
#endif
                        dupesDict[__instance.gameObject] = -1;
                    }
                }
            }
        }


        /// <summary>
        /// Applied to TryEntombedEscape. For some reason, when landing the game considers the dupe to be entombed 
        /// and tries to find a tile the dupe could escape to.
        /// </summary>
        /// 
        [HarmonyPatch(typeof(FallMonitor.Instance), "TryEntombedEscape")]
        public static class FallMonitorInstance_TryEntombedEscape_Patch
        {
            /// <summary>
            /// Applied after TryEntombedEscape. Whether or not the dupe manages to escape, if he was falling we apply the same damage.
            /// </summary>
            /// <param name="__instance">The current FallMonitor.Instance.</param>
            /// <param name="__navigator">The current FallMonitor.Instance.navigator.</param>

            internal static void Postfix(FallMonitor.Instance __instance, Navigator ___navigator)
            {
                if (__instance.gameObject.HasTag(GameTags.Minion))
                {
                    if (!dupesDict.ContainsKey(__instance.gameObject))
                    {
#if DEBUG
                        PUtil.LogError("Unknown dupe tryEntombedEscape");
#endif
                        dupesDict.Add(__instance.gameObject, -1);
                    }

#if DEBUG
                    PUtil.LogDebug(("TryEntombedEscape. Dupe: {0}, falled to {1}").F(__instance.gameObject.name, Grid.PosToCell((KMonoBehaviour)___navigator)));
#endif

                    if (dupesDict[__instance.gameObject] >= 0)
                    {
                        Grid.CellToXY(dupesDict[__instance.gameObject], out int xFrom, out int yFrom);
                        Grid.CellToXY(Grid.PosToCell((KMonoBehaviour)___navigator), out int xTo, out int yTo);
                        int dist = yFrom - yTo;

                        Health dupeHealth = __instance.gameObject.GetComponent<Health>();
                        if (dupeHealth != null)
                        {
                            if (dist > 2)
                            {
                                dupeHealth.Damage((dist - 2) * 10f);
                                if ((dist > 5) && (UnityEngine.Random.Range((int)0, (int)100) <= dist * 10))
                                {
#if DEBUG
                                    PUtil.LogDebug(("Dupe: {0}, hasEffect Crippled: {1}").F(__instance.gameObject.name, __instance.gameObject.AddOrGet<Effects>().HasEffect("Crippled")));
#endif
                                    if (!__instance.gameObject.AddOrGet<Effects>().HasEffect("DangerousWorldCrippled"))
                                        __instance.gameObject.AddOrGet<Effects>().Add("DangerousWorldCrippled", true);
#if DEBUG
                                    PUtil.LogDebug(("Dupe: {0}, hasEffect Crippled: {1}").F(__instance.gameObject.name, __instance.gameObject.AddOrGet<Effects>().HasEffect("Crippled")));
#endif
                                }
                            }
                        }
                        dupesDict[__instance.gameObject] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Applied to LandFloor. This is the normal method called on hitting the floor.
        /// This happens if the dupe did not manage to catch a pole or a ladder.
        /// </summary>
        /// 

        [HarmonyPatch(typeof(FallMonitor.Instance), "LandFloor")]
        public static class FallMonitorInstance_LandFloor_Patch
        {
            /// <summary>
            /// Applied after LandFloor runs. If teh dupe was falling, we apply damage.
            /// </summary>
            /// <param name="__instance">The current FallMonitor.Instance.</param>
            /// <param name="__navigator">The current FallMonitor.Instance.navigator.</param>
            internal static void Postfix(FallMonitor.Instance __instance, Navigator ___navigator)
            {
                if (__instance.gameObject.HasTag(GameTags.Minion))
                {
                    if (!dupesDict.ContainsKey(__instance.gameObject))
                    {
#if DEBUG
                        PUtil.LogError("Unknown dupe landFloor");
#endif
                        dupesDict.Add(__instance.gameObject, -1);
                    }

#if DEBUG
                    PUtil.LogDebug(("LandFloor. Dupe: {0}, falled to {1}").F(__instance.gameObject.name, Grid.PosToCell((KMonoBehaviour)___navigator)));
#endif

                    if (dupesDict[__instance.gameObject] >= 0)
                    {
                        Grid.CellToXY(dupesDict[__instance.gameObject], out int xFrom, out int yFrom);
                        Grid.CellToXY(Grid.PosToCell((KMonoBehaviour)___navigator), out int xTo, out int yTo);
                        int dist = yFrom - yTo;

                        Health dupeHealth = __instance.gameObject.GetComponent<Health>();
                        if (dupeHealth != null)
                        {
                            if (dist > 2)
                            {
                                dupeHealth.Damage((dist - 2) * 10f);
                                if ((dist > 5) && (UnityEngine.Random.Range((int) 0, (int) 100) <= dist * 10))
                                {
#if DEBUG
                                    PUtil.LogDebug(("Dupe: {0}, hasEffect Crippled: {1}").F(__instance.gameObject.name, __instance.gameObject.AddOrGet<Effects>().HasEffect("Crippled")));
#endif
                                    if (!__instance.gameObject.AddOrGet<Effects>().HasEffect("DangerousWorldCrippled"))
                                        __instance.gameObject.AddOrGet<Effects>().Add("DangerousWorldCrippled", true);
#if DEBUG
                                    PUtil.LogDebug(("Dupe: {0}, hasEffect Crippled: {1}").F(__instance.gameObject.name, __instance.gameObject.AddOrGet<Effects>().HasEffect("Crippled")));
#endif
                                }
                            }
                        }
                        dupesDict[__instance.gameObject] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Applied to MountLadder, which is called when the dupe manages to grab a ladder while falling.
        /// </summary>
        [HarmonyPatch(typeof(FallMonitor.Instance), "MountLadder")]
        public static class FallMonitorInstance_MountLadder_Patch
        {
            /// <summary>
            /// Applied after MountLadder runs. We do not apply damage there and reinitialise the variables for next time.
            /// </summary>
            /// <param name="__instance">The current FallMonitor.Instance.</param>
            /// <param name="__navigator">The current FallMonitor.Instance.navigator.</param>
            internal static void Postfix(FallMonitor.Instance __instance, Navigator ___navigator)
            {
#if DEBUG
                PUtil.LogDebug(("MountLadder: {0}").F(__instance.gameObject.name));
#endif
                if (__instance.gameObject.HasTag(GameTags.Minion))
                {
                    if (!dupesDict.ContainsKey(__instance.gameObject))
                    {
#if DEBUG
                        PUtil.LogError("Unknown dupe mountLadder");
#endif
                        dupesDict.Add(__instance.gameObject, -1);
                    }

                    dupesDict[__instance.gameObject] = -1;

#if DEBUG
                    PUtil.LogDebug(("Dupe: {0}, recoverd to Ladder").F(__instance.gameObject.name));
#endif
                }
            }
        }

        /// <summary>
        /// Applied to MountPole, which is called when the dupe manages to grab a pole while falling.
        /// </summary>
        [HarmonyPatch(typeof(FallMonitor.Instance), "MountPole")]
        public static class FallMonitorInstance_MountPole_Patch
        {
            /// <summary>
            /// Applied after MountPole runs. We do not apply damage there and reinitialise the variables for next time.
            /// </summary>
            /// <param name="__instance">The current FallMonitor.Instance.</param>
            /// <param name="__navigator">The current FallMonitor.Instance.navigator.</param>
            internal static void Postfix(FallMonitor.Instance __instance, Navigator ___navigator)
            {
#if DEBUG
                PUtil.LogDebug(("MountPole: {0}").F(__instance.gameObject.name));
#endif
                if (__instance.gameObject.HasTag(GameTags.Minion))
                {
                    if (!dupesDict.ContainsKey(__instance.gameObject))
                    {
#if DEBUG
                        PUtil.LogError("Unknown dupe mountPole");
#endif
                        dupesDict.Add(__instance.gameObject, -1);
                    }

                    dupesDict[__instance.gameObject] = -1;

#if DEBUG
                    PUtil.LogDebug(("Dupe: {0}, recoverd to Pole").F(__instance.gameObject.name));
#endif
                }
            }
        }
    }
}
