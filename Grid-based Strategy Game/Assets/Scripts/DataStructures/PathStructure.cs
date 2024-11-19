using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class PathStructure : MonoBehaviour
{
    public List<PathNode> pathNodeList;
    public List<Vector3> vectorPathList;

    public PathStructure(List<PathNode> pathNodeList, Vector3 worldPos, float cellSize)
    {
        this.pathNodeList = pathNodeList;

        vectorPathList = new List<Vector3>();
        GetVectorList(worldPos, cellSize);
    }
    public void GetVectorList(Vector3 worldOrigin, float cellSize)
    {
        vectorPathList = new List<Vector3>();
        foreach (PathNode pathNode in pathNodeList)
        {
            vectorPathList.Add(pathNode.GetWorldVector(worldOrigin, cellSize));
        }
    }
}
