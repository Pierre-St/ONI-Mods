using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using PeterHan.PLib.Core;



namespace PierreStirnweiss.DangerousWorld
{
    class DangerousAggroTest
    {
        public class TestMonitor : KMonoBehaviour, ISim1000ms
        {
            public TestMonitor()
            {
            }
            public void Sim1000ms(float delta)
            {
                //List<PositionMonitor.PositionMonitorEntry> entitiesList = new List<PositionMonitor.PositionMonitorEntry>();

                Extents gridCheck = new Extents(Grid.PosToCell(this.gameObject), 5);
#if DEBUG
                PUtil.LogDebug(("Extent: x: {0}, y: {1}, width: {2}, height: {3}").F(gridCheck.x, gridCheck.y, gridCheck.width, gridCheck.height));
                PUtil.LogDebug(("PosToCell: {0}, cell x: {1}, cell y {2}").F(Grid.PosToCell(this.gameObject), Grid.CellToPos(Grid.PosToCell(this.gameObject)).x, Grid.CellToPos(Grid.PosToCell(this.gameObject)).y));
#endif
                //PositionMonitor.Instance.GatherEntries(gridCheck, PositionMonitor.Instance.Layer("DW_Hatches"), entitiesList);
                //GameScenePartitioner.Instance.GatherEntries(gridCheck, GameScenePartitioner.Instance.attackableEntitiesLayer, entitiesList);
                int count = PositionMonitor.Instance.CountEntries(gridCheck, PositionMonitor.Instance.Layer("DW_Hatches"));
#if DEBUG
                //PUtil.LogDebug(("Number of components found: {0}").F(entitiesList.Count));
                PUtil.LogDebug(("Number of Hatches found: {0}").F(count));
#endif
                /*for (int index = 0; index < entitiesList.Count; ++index)
                {
                    GameObject cmp = entitiesList[index].m_object as GameObject;
                    //FactionAlignment cmp = entitiesList[index].m_object as FactionAlignment;
                    PUtil.LogDebug(("Component {0}, name {1}").F(index, cmp.name));
                }*/
            }

            public class Iterator : GameScenePartitioner.Iterator
            {
                public void Iterate(object target_object)
                {
                    FactionAlignment gameObject = target_object as FactionAlignment;
                    //PUtil.LogDebug("Iterate");
                    if (gameObject != null)
                        PUtil.LogDebug(("Object name: {0}").F(gameObject.name));
                }

                public void Cleanup()
                {

                }
            }
        }
    }
}
        
