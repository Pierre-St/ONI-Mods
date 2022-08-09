using HarmonyLib;
using PeterHan.PLib.Core;
using Klei.AI;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PierreStirnweiss.DangerousWorld
{
    public class DangerousWorldAggroCrittersPatches
    {

        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public static class Game_OnPrefabInit_Patch
        {
            internal static void Prefix()
            {
                //PUtil.LogDebug("Game Prefab prefix");
            }

            internal static void Postfix(Game __instance)
            {
                // First disable entirely if not activated
                if (!DangerousWorldOptions.Instance.AggroCrittersOption) 
                    return;

                //PUtil.LogDebug("Game pre PrefabInit");
                PositionMonitor monitor = __instance.FindOrAdd<PositionMonitor>();
                monitor.AddLayer("DW_Hatches");
                monitor.AddLayer("DW_Minions");
            }
        }

        [HarmonyPatch(typeof(HatchConfig), "OnPrefabInit")]
        public class HatchConfig_OnPrefabInit_Patch
        {
            public static void Prefix()
            {
                //PUtil.LogDebug("Hatch config OnPrefabInit prefix");
            }
            public static void Postfix(GameObject prefab)
            {
                // First disable entirely if not activated
                if (!DangerousWorldOptions.Instance.AggroCrittersOption)
                    return;

                //PUtil.LogDebug("HatchConfig OnPrefabInit postfix");
                //prefab.AddOrGet<DangerousAggroTest.TestMonitor>();
                prefab.AddOrGetDef<DangerousWorldAggroCritters.AggroMonitor.Def>().aggroType = DangerousWorldAggroCritters.AggroMonitor.Def.AggroTypes.Crowd;
            }
        }

        [HarmonyPatch(typeof(HatchConfig), "OnSpawn")]
        public class HatchConfig_OnSpawn_Patch
        {
            public static void Postfix(GameObject inst)
            {
                // First disable entirely if not activated
                if (!DangerousWorldOptions.Instance.AggroCrittersOption)
                    return;

                PositionMonitor.PositionMonitorLayer layer = PositionMonitor.Instance.Layer("DW_Hatches");

                if (layer == null)
                {
#if DEBUG
                    PUtil.LogDebug("Null layer");
#endif
                    layer = PositionMonitor.Instance.AddLayer("DW_Hatches");
                }
                Debug.Assert(layer != null, "Hatch layer is null");

                PositionMonitor.Instance.Add(inst, Grid.CellToXY(Grid.PosToCell(inst)).x, Grid.CellToXY(Grid.PosToCell(inst)).y, layer);
            }
        }

        [HarmonyPatch(typeof(MinionConfig), "OnSpawn")]
        public class MinionConfig_OnSpawn_Patch
        {
            public static void Postfix(GameObject go)
            {
                // First disable entirely if not activated
                if (!DangerousWorldOptions.Instance.AggroCrittersOption)
                    return;

                PositionMonitor.PositionMonitorLayer layer = PositionMonitor.Instance.Layer("DW_Minions");

                if (layer == null)
                {
#if DEBUG
                    PUtil.LogDebug("Null layer");
#endif
                    layer = PositionMonitor.Instance.AddLayer("DW_Minions");
                }
                Debug.Assert(layer != null, "Minion layer is null");

                PositionMonitor.Instance.Add(go, Grid.CellToXY(Grid.PosToCell(go)).x, Grid.CellToXY(Grid.PosToCell(go)).y, layer);
            }
        }

        [HarmonyPatch(typeof(ThreatMonitor.Instance), "WillFight")]
        public class ThreatMonitorInstance_WillFight_Patch
        {
            public static void Postfix(ref bool __result, ThreatMonitor.Instance __instance)
            {
                // First disable entirely if not activated
                if (!DangerousWorldOptions.Instance.AggroCrittersOption)
                    return;

                DangerousWorldAggroCritters.AggroMonitor.Instance aggroMonitor = __instance.GetSMI<DangerousWorldAggroCritters.AggroMonitor.Instance>();
                if (aggroMonitor != null && aggroMonitor.GetCurrentState() != aggroMonitor.sm.calm)
                {
                    __result = aggroMonitor.WillAttack();
                }
            }
        }
    }
}
