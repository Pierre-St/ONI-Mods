using PeterHan.PLib.Core;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PierreStirnweiss.DangerousWorld
{
    class PositionMonitor : KMonoBehaviour, ISim1000ms
    {
        private static readonly Predicate<PositionMonitorEntry> isInvalidCheck = (Predicate<PositionMonitorEntry>)(entry =>
        {
            if (entry != null)
                return entry.m_object == null;
            return true;
        });

        public List<PositionMonitorLayer> m_layers = new List<PositionMonitorLayer>(0);

        private PositionMonitor.PositionMonitorNode[,,] m_nodes;
        private List<PositionMonitor.DirtyNode> m_dirtyNodes = new List<DirtyNode>();
        private static PositionMonitor m_instance;

        public static PositionMonitor Instance
        {
            get
            {
                return m_instance;
            }
        }

        public PositionMonitor()
        {
            if ((UnityEngine.Object)PositionMonitor.m_instance == (UnityEngine.Object)null)
                m_instance = this;
            this.m_layers.Add(new PositionMonitorLayer((HashedString)"DW_Default", 0));
            this.m_nodes = new PositionMonitor.PositionMonitorNode[64, Grid.WidthInCells, Grid.HeightInCells];
        }

        public PositionMonitorLayer AddLayer(HashedString layerName)
        {
            if (m_layers.Count >= 64)
            {
#if DEBUG
                PUtil.LogDebug("Maximum number of layers reached. Returning the default layer");
#endif
                return m_layers[0];
            }

            foreach (PositionMonitorLayer l in m_layers)
            {
                if (l.m_name == layerName)
                    return l;
            }

            PositionMonitorLayer layer = new PositionMonitorLayer(layerName, m_layers.Count);
            m_layers.Add(layer);
            return layer;
        }

        public PositionMonitorLayer Layer(HashedString layerName)
        {
            foreach (PositionMonitorLayer l in m_layers)
            {
                if (l.m_name == layerName)
                    return l;
            }

            return (PositionMonitorLayer)null;
        }

        public PositionMonitorEntry Add(GameObject obj, int x, int y, PositionMonitorLayer layer)
        {
            PositionMonitorEntry entry = new PositionMonitorEntry(x, y, layer, obj, this);
            if (!m_nodes[layer.m_layer, x, y].isDirty)
            {
                m_nodes[layer.m_layer, x, y].isDirty = true;
                m_dirtyNodes.Add(new DirtyNode()
                {
                    m_layer = layer.m_layer,
                    m_x = x,
                    m_y = y
                });
            }
            m_nodes[layer.m_layer, x, y].m_entries.Add(entry);
            return entry;
        }

        public void Insert(PositionMonitorEntry entry)
        {
            if (!m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].isDirty)
            {
                m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].isDirty = true;
                m_dirtyNodes.Add(new DirtyNode()
                {
                    m_layer = entry.m_layer.m_layer,
                    m_x = entry.m_x,
                    m_y = entry.m_y
                });
            }
            m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].m_entries.Add(entry); 
        }

        public void Remove(PositionMonitorEntry entry)
        {
            int count = m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].m_entries.Count;
            for (int index = 0; index < count; ++index)
            {
                if (m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].m_entries[index] == entry)
                {
                    if (!m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].isDirty)
                    {
                        m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].isDirty = true;
                        m_dirtyNodes.Add(new DirtyNode()
                        {
                            m_layer = entry.m_layer.m_layer,
                            m_x = entry.m_x,
                            m_y = entry.m_y
                        });
                    }
                    m_nodes[entry.m_layer.m_layer, entry.m_x, entry.m_y].m_entries[index] = (PositionMonitorEntry)null;
                    break;
                }
            }
        }

        public void UpdatePosition(int x, int y, PositionMonitorEntry entry)
        {
            this.Remove(entry);
            entry.m_x = x;
            entry.m_y = y;
            this.Insert(entry);
        }

        public void GatherEntries(Extents area, PositionMonitorLayer layer, List<PositionMonitorEntry> gatheredEntries)
        {
            int xIndexFrom = Math.Max(area.x, 0);
            int xIndexTo = Math.Min(area.x + area.width, Grid.WidthInCells);
            int yIndexFrom = Math.Max(area.y, 0);
            int yIndexTo = Math.Min(area.y + area.height, Grid.HeightInCells);

            for (int iX = xIndexFrom; iX < xIndexTo; ++ iX)
            {
                for (int iY = yIndexFrom; iY < yIndexTo; ++iY)
                {
                    int nbrEntries = m_nodes[layer.m_layer, iX, iY].m_entries.Count;
                    for (int index = 0; index < nbrEntries; ++index)
                    {
                        PositionMonitorEntry entry = m_nodes[layer.m_layer, iX, iY].m_entries[index];
                        if (entry != null)
                        {
                            if (entry.m_object == null)
                            {
                                m_nodes[layer.m_layer, iX, iY].m_entries[index] = (PositionMonitorEntry)null;
                                m_nodes[layer.m_layer, iX, iY].isDirty = true;
                            }
                            else
                            {
                                gatheredEntries.Add(entry);
                            }
                        }
                    }
                }
            }
        }

        public int CountEntries(Extents area, PositionMonitorLayer layer)
        {
            int xIndexFrom = Math.Max(area.x, 0);
            int xIndexTo = Math.Min(area.x + area.width, Grid.WidthInCells);
            int yIndexFrom = Math.Max(area.y, 0);
            int yIndexTo = Math.Min(area.y + area.height, Grid.HeightInCells);

            int countEntries = 0;

            for (int iX = xIndexFrom; iX < xIndexTo; ++iX)
            {
                for (int iY = yIndexFrom; iY < yIndexTo; ++iY)
                {
                    int nbrEntries = m_nodes[layer.m_layer, iX, iY].m_entries.Count;
                    for (int index = 0; index < nbrEntries; ++index)
                    {
                        PositionMonitorEntry entry = m_nodes[layer.m_layer, iX, iY].m_entries[index];
                        if (entry != null)
                        {
                            if (entry.m_object == null)
                            {
                                m_nodes[layer.m_layer, iX, iY].m_entries[index] = (PositionMonitorEntry)null;
                                m_nodes[layer.m_layer, iX, iY].isDirty = true;
                            }
                            else
                            {
                                countEntries++;
                            }
                        }
                    }
                }
            }
            return countEntries;
        }

            public void Sim1000ms(float dt)
        {
            foreach (PositionMonitor.DirtyNode dirtyNode in this.m_dirtyNodes)
            {
                this.m_nodes[dirtyNode.m_layer, dirtyNode.m_y, dirtyNode.m_x].m_entries.RemoveAllSwap(PositionMonitor.isInvalidCheck);
                this.m_nodes[dirtyNode.m_layer, dirtyNode.m_y, dirtyNode.m_x].isDirty = false;
            }
            this.m_dirtyNodes.Clear();
        }

        public class PositionMonitorEntry
        {
            public int m_x;
            public int m_y;
            public PositionMonitorLayer m_layer;
            public GameObject m_object;
            public PositionMonitor m_monitor;

            public PositionMonitorEntry(int x, int y, PositionMonitorLayer layer, GameObject obj, PositionMonitor monitor)
            {
                m_x = x;
                m_y = y;
                m_layer = layer;
                m_object = obj;
                m_monitor = monitor;
                Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(m_object.transform, new System.Action(this.UpdatePosition), "AggroCritter.SetMaster");
            }

            public void UpdatePosition()
            {
                int x = (int)m_object.transform.position.x;
                int y = (int)m_object.transform.position.y;
                if (this.m_monitor == null)
                    return;
                this.m_monitor.UpdatePosition(x, y, this);
            }

            public void Release()
            {
                if (this.m_monitor == null)
                    return;
                this.m_monitor.Remove(this);
            }
        }

        public class PositionMonitorLayer
        {
            public int m_layer;
            public HashedString m_name;

            public PositionMonitorLayer(HashedString name, int layer)
            {
                m_name = name;
                m_layer = layer;
            }
        }

        private struct PositionMonitorNode
        {
            public ArrayRef<PositionMonitorEntry> m_entries;
            public bool isDirty;
        }

        private class DirtyNode
        {
            public int m_x;
            public int m_y;
            public int m_layer;
        }
    }
}
