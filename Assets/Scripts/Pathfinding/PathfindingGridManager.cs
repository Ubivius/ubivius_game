﻿using System.Collections;
using System.Collections.Generic;
using ubv.common.world;
using UnityEngine;

public class PathfindingGridManager
{
    private LogicGrid m_logicGrid;
    private List<PathNode> m_pathNodeList;
    private Pathfinding m_pathfinding;

    public PathfindingGridManager(LogicGrid logicGrid)
    {
        m_logicGrid = logicGrid;
        m_pathNodeList = new List<PathNode>();

        for (int x = 0; x < m_logicGrid.Width; x++)
        {
            for (int y = 0; y < m_logicGrid.Height; y++)
            {
                if (m_logicGrid.Grid[x,y].IsWalkable)
                {
                    m_pathNodeList.Add(new PathNode(x, y));
                }
            }
        }

        //Add neighbours to each pathnode
        foreach(PathNode pathnode in m_pathNodeList)
        {
            if (pathnode.X - 1 >= 0)
            {
                // Left
                pathnode.AddNeighbour(GetNode(pathnode.X - 1, pathnode.Y));
                // Left Down
                if (pathnode.Y - 1 >= 0) pathnode.AddNeighbour(GetNode(pathnode.X - 1, pathnode.Y - 1));
                // Left Up
                if (pathnode.Y + 1 < m_logicGrid.Height) pathnode.AddNeighbour(GetNode(pathnode.X - 1, pathnode.Y + 1));
            }

            if (pathnode.X + 1 < m_logicGrid.Width)
            {
                // Right
                pathnode.AddNeighbour(GetNode(pathnode.X + 1, pathnode.Y));
                // Right Down
                if (pathnode.Y - 1 >= 0) pathnode.AddNeighbour(GetNode(pathnode.X + 1, pathnode.Y - 1));
                // Right Up
                if (pathnode.Y + 1 < m_logicGrid.Height) pathnode.AddNeighbour(GetNode(pathnode.X + 1, pathnode.Y + 1));
            }

            // Down
            if (pathnode.Y - 1 >= 0) pathnode.AddNeighbour(GetNode(pathnode.X, pathnode.Y - 1));
            // Up
            if (pathnode.Y + 1 < m_logicGrid.Height) pathnode.AddNeighbour(GetNode(pathnode.X, pathnode.Y + 1));

            pathnode.GetNeighbourList().RemoveAll(null);
        }

        m_pathfinding = new Pathfinding(m_pathNodeList);
    }

    private PathNode GetNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < m_logicGrid.Width && y < m_logicGrid.Height && m_pathNodeList.Contains(new PathNode(x, y)))
        {
            return m_pathNodeList.Find(pathnode => pathnode.X == x && pathnode.Y == y);
        }
        else
        {
            return null;
        }
    }

    public List<PathNode> GetPath(PathNode startNode, PathNode endNode)
    {
        return m_pathfinding.FindPath(startNode, endNode);
    }
}