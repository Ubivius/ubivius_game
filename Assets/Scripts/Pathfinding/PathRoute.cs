﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class PathRoute
    {
        public List<PathNode> m_pathNodeList;
        public List<Vector2> pathVectorList;

        public PathRoute(List<PathNode> pathNodeList, List<Vector2> pathVectorList)
        {
            this.m_pathNodeList = pathNodeList;
            this.pathVectorList = pathVectorList;
        }

        public PathRoute(List<PathNode> pathNodeList, Vector3 worldOrigin, float nodeSize)
        {
            this.m_pathNodeList = pathNodeList == null ? new List<PathNode>() : pathNodeList;
            pathVectorList = new List<Vector2>();
            foreach (PathNode pathNode in m_pathNodeList)
            {
                pathVectorList.Add(pathNode.GetWorldVector(worldOrigin, nodeSize));
            }
        }

        public void AddVector(Vector2 vector)
        {
            pathVectorList.Add(vector);
        }
    }
}
