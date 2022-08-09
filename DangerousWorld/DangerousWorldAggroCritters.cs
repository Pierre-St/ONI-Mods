using Klei.AI;
using UnityEngine;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PierreStirnweiss.DangerousWorld
{
    public class DangerousWorldAggroCritters
    {
        public class AggroCritter : KMonoBehaviour
        {

            PositionMonitor.PositionMonitorEntry m_entry;
            GameObject m_master;
            public AggroCritter()
            {
#if DEBUG
                PUtil.LogDebug("In AggroCritter cstr");
#endif
            }

                public void SetMaster(GameObject master)
            {
#if DEBUG
                PUtil.LogDebug(("In set Master: {0}").F(master));
#endif
                m_master = master;
                PositionMonitor.PositionMonitorLayer layer = PositionMonitor.Instance.Layer("DW_Hatches");

                if (layer == null)
                {
#if DEBUG
                    PUtil.LogDebug("Null layer");
#endif
                    layer = PositionMonitor.Instance.AddLayer("DW_Hatches");
                }
                Debug.Assert(layer != null, "Hatch layer is null");

#if DEBUG
                PUtil.LogDebug(("Layer: {0}").F(layer.m_name.ToString()));
#endif

                m_entry = PositionMonitor.Instance.Add(m_master, Grid.CellToXY(Grid.PosToCell(m_master)).x, Grid.CellToXY(Grid.PosToCell(m_master)).y, layer);

                //Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "AggroCritter.SetMaster");

                //m_master.Subscribe(1088554450, (System.Action<object>)((component) => { PUtil.LogDebug("In update position callback"); PositionMonitor.Instance.UpdatePosition(Grid.CellToXY(Grid.PosToCell(component as GameObject)).x,
                //Grid.CellToXY(Grid.PosToCell(component as GameObject)).y,
                //m_entry);
                //}));
            }

            public void OnCellChange()
            {
                PositionMonitor.Instance.UpdatePosition(Grid.CellToXY(Grid.PosToCell(m_master)).x,
                Grid.CellToXY(Grid.PosToCell(m_master)).y,
                m_entry);
            }
        }
        public class AggroMonitor : GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>
        {
            public GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.BoolParameter isAggro;
            public GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State calm;
            public AggroStates aggro;

            public class AggroStates : GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State
            {
                public GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State attack;
                public GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State subdued;
            }
            public class Def : StateMachine.BaseDef
            {
                [Flags]
                public enum AggroTypes
                {
                    None = 0,
                    Crowd = 1,
                    Handling = 2
                }

                public AggroTypes aggroType
                {
                    get; set;
                }

                public bool HasAggroType(AggroTypes type)
                {
                    return ((aggroType & type) == type);
                }
                
            }

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = (StateMachine.BaseState)this.calm;

                this.calm.
                    Enter((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State.Callback)(smi =>
                    {
                        //PUtil.LogDebug("Critter enter calm state");
                        smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
                    })).
                    Update("evaluate_aggroLevel_calm", (System.Action<AggroMonitor.Instance, float>)((smi, dt) => smi.EvaluateAggroLevel()),
                        UpdateRate.SIM_200ms).ParamTransition<bool>((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.Parameter<bool>) this.isAggro, 
                        this.aggro, GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.IsTrue);

                this.aggro.
                    Enter((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State.Callback)(smi =>
                    {
                        //PUtil.LogDebug("Critter enter aggro state");
                        smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
                    })).
                    ToggleStatusItem((String)DangerousWorldStrings.AGGRO_CRITTER_AGGRO_TITLE, (String)DangerousWorldStrings.AGGRO_CRITTER_AGGRO_DESC).
                    Update("evaluate_aggroLevel_aggro", (System.Action<AggroMonitor.Instance, float>)((smi, dt) => smi.EvaluateAggroLevel()),
                        UpdateRate.SIM_200ms).ParamTransition<bool>((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.Parameter<bool>)this.isAggro,
                        this.calm, GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.IsFalse);

                this.aggro.attack.
                    /*Enter((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State.Callback)(smi =>
                    {
                        PUtil.LogDebug(("Critter attack: {0}").F(smi.MainFiend.name));
                    })).*/
                    ToggleStatusItem((String)DangerousWorldStrings.AGGRO_CRITTER_ATTACK_TITLE, (String)DangerousWorldStrings.AGGRO_CRITTER_ATTACK_DESC).
                    Update("evaluate_aggroLevel_aggro", (System.Action<AggroMonitor.Instance, float>)((smi, dt) => smi.EvaluateAggroLevel()), UpdateRate.SIM_200ms);

                this.aggro.subdued.
                    /*Enter((StateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.State.Callback)(smi =>
                    {
                        PUtil.LogDebug(("Critter subdued: {0}").F(smi.MainFiend.name));
                    })).*/
                    ToggleStatusItem((String)DangerousWorldStrings.AGGRO_CRITTER_SUBDUED_TITLE, (String)DangerousWorldStrings.AGGRO_CRITTER_SUBDUED_DESC).
                    Update("evaluate_aggroLevel_aggro", (System.Action<AggroMonitor.Instance, float>)((smi, dt) => smi.EvaluateAggroLevel()), UpdateRate.SIM_200ms);
            }

            new public class Instance : GameStateMachine<AggroMonitor, AggroMonitor.Instance, IStateMachineTarget, AggroMonitor.Def>.GameInstance
            {
                public FactionAlignment alignment;
                private readonly List<FactionAlignment> fiends = new List<FactionAlignment>();
                //private readonly List<FactionAlignment> buddies = new List<FactionAlignment>();   Not used for now

                private int buddiesCount = 0;

                private GameObject mainFiend;
                private int maxThreatDistance = 5;
                private Navigator navigator;


                public Instance(IStateMachineTarget master, AggroMonitor.Def def) : base(master, def)
                {
                    this.alignment = master.GetComponent<FactionAlignment>();
                    this.navigator = master.GetComponent<Navigator>();
                }

                public GameObject MainFiend
                {
                    get
                    {
                        return this.mainFiend;
                    }
                }

                public bool WillAttack() //configure?
                {
                    return this.GetComponent<Health>().hitPoints > this.GetComponent<Health>().maxHitPoints * 0.5;
                }

                public void EvaluateAggroLevel()
                {
                    if (this.gameObject.HasTag(GameTags.Creatures.Wild) && this.def.HasAggroType(Def.AggroTypes.Crowd))
                    {
#if DEBUG
                        PUtil.LogDebug(("Evaluate aggro level hasTag wild: {0}").F(this.gameObject.HasTag(GameTags.Creatures.Wild)));
                        //PUtil.LogDebug("EvaluateAggroLevel");
#endif
                        EvaluateBuddiesAndFiends();

                        if (buddiesCount >= 2 * this.fiends.Count) /// for now, make it configurable
                        {
#if DEBUG
                            PUtil.LogDebug(("Aggro: buddies count: {0}").F(buddiesCount));
#endif
                            this.sm.isAggro.Set(true, smi);
                            if (!this.WillAttack())
                            {
#if DEBUG
                                PUtil.LogDebug("Hatch is subdued");
#endif
                                this.GoTo(this.sm.aggro.subdued);
                            }
                            else
                            {
                                if (this.fiends.Count > 0)
                                {
                                    this.mainFiend = PickBestThreat();
                                    if (mainFiend != (GameObject)null)
                                    {
#if DEBUG
                                        PUtil.LogDebug(("Hatch found ennemy: {0}").F(mainFiend.name));
#endif
                                        this.GetSMI<ThreatMonitor.Instance>().SetMainThreat(mainFiend);
                                        this.GoTo(this.sm.aggro.attack);
                                        this.GetSMI<ThreatMonitor.Instance>().GoToThreatened();
                                    }
                                }
                            }
                        }
                        else
                        {
#if DEBUG
                            PUtil.LogDebug(("Calm: buddies count: {0}").F(buddiesCount));
#endif
                            this.sm.isAggro.Set(false, smi);
                        }
                    }
                }

                public void EvaluateBuddiesAndFiends()
                {
#if DEBUG
                    PUtil.LogDebug("EvaluateBuddiesAndFiends");
#endif
                    this.fiends.Clear();
                    //this.buddies.Clear();

#if DEBUG
                    PUtil.LogDebug(("Creature : {0}, pos {1}").F(this.gameObject.name, Grid.PosToCell((KMonoBehaviour)this.navigator)));
#endif
                    Extents gridCheck = new Extents(Grid.PosToCell((KMonoBehaviour)this.navigator), maxThreatDistance);

                    buddiesCount = PositionMonitor.Instance.CountEntries(gridCheck, PositionMonitor.Instance.Layer("DW_Hatches"));
#if DEBUG
                    PUtil.LogDebug(("Number of buddies found: {0}").F(buddiesCount));
#endif

                    ListPool<PositionMonitor.PositionMonitorEntry, PositionMonitor>.PooledList entitiesList = ListPool<PositionMonitor.PositionMonitorEntry, PositionMonitor>.Allocate();
                    PositionMonitor.Instance.GatherEntries(gridCheck, PositionMonitor.Instance.Layer("DW_Minions"), entitiesList);
#if DEBUG
                    PUtil.LogDebug(("Number of components found: {0}").F(entitiesList.Count));
#endif
                    for (int index = 0; index < entitiesList.Count; ++index)
                    {
                        FactionAlignment cmp = entitiesList[index].m_object.GetComponent<FactionAlignment>();
                        if (cmp != null && this.navigator.CanReach((IApproachable)cmp.attackable) && cmp.GetComponent<MinionBrain>() != null && cmp.GetComponent<MinionBrain>().HasTag(GameTags.Minion))
                        {
#if DEBUG
                            PUtil.LogDebug(("Component {0} has tag minion: {1}").F(cmp.name, cmp.GetComponent<MinionBrain>().HasTag(GameTags.Minion)));
#endif
                            this.fiends.Add(cmp);
                        }
                    }
                    entitiesList.Recycle();
                }

                public GameObject PickBestThreat()
                {
                    if (this.fiends.Count == 0)
                        return (GameObject) null;

                    float num1 = 1f;
                    Vector2 myPosition = (Vector2)this.gameObject.transform.GetPosition();
                    GameObject gameObject = (GameObject)null;
                    float num2 = float.PositiveInfinity;
                    for (int index = this.fiends.Count - 1; index >= 0; --index)
                    {
                        FactionAlignment threat = this.fiends[index];
                        float num3 = Vector2.Distance(myPosition, (Vector2)threat.transform.GetPosition()) / num1;
                        if ((double)num3 < (double)num2)
                        {
                            num2 = num3;
                            gameObject = threat.gameObject;
                        }
                    }
                    return gameObject;
                }
            }
        }
    }
}
