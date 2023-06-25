using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private Grid3D<GridNode> grid;
    private List<GridNode> openList;
    private List<GridNode> closedList;

    public Pathfinding(int width, int height) {
        Instance = this;
        grid = new Grid3D<GridNode>(width, height, 10f, Vector3.zero, true, (Grid3D<GridNode> g, int x, int z) => new GridNode(g, x, z));
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXZ(startWorldPosition, out int startX, out int startZ);
        grid.GetXZ(endWorldPosition, out int endX, out int endZ);
        List<GridNode> path = FindPath(startX, startZ, endX, endZ);
        if(path != null) {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(GridNode pathNode in path) {
                vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.z) * grid.GetCellSize() + new Vector3(1, 0, 1) * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        } else return null;
    }

    public List<GridNode> FindPath(int startX, int startY, int endX, int endY) {
        GridNode startNode = grid.GetGridObject(startX, startY);
        GridNode endNode = grid.GetGridObject(endX, endY);
        openList = new List<GridNode> { startNode };
        closedList = new List<GridNode>();

        for(int x = 0; x < grid.GetWidth(); x++) {
            for(int y = 0; y < grid.GetHeight(); y++) {
                GridNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalcFCost();
                pathNode.cameFromNode = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalcDistance(startNode, endNode);
        startNode.CalcFCost();

        while(openList.Count > 0) {
            GridNode currNode = GetLowestFCostNode(openList);
            if(currNode == endNode) {
                return CalcPath(endNode);
            }
            openList.Remove(currNode);
            closedList.Add(currNode);

            foreach(GridNode neighbourNode in GetNeighborList(currNode)) {
                if(closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }
                int tentativeGCost = currNode.gCost + CalcDistance(currNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalcDistance(neighbourNode, endNode);
                    neighbourNode.CalcFCost();

                    if(!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private List<GridNode> GetNeighborList(GridNode currNode) {
        List<GridNode> neighbourList = new List<GridNode>();
        if(currNode.x - 1 >= 0) {
            neighbourList.Add(GetNode(currNode.x - 1, currNode.z));
            if(currNode.z - 1 >= 0) neighbourList.Add(GetNode(currNode.x - 1, currNode.z - 1));
            if(currNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currNode.x - 1, currNode.z + 1));
        }
        if(currNode.x + 1 < grid.GetWidth()) {
            neighbourList.Add(GetNode(currNode.x + 1, currNode.z));
            if(currNode.z - 1 >= 0) neighbourList.Add(GetNode(currNode.x + 1, currNode.z - 1));
            if(currNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currNode.x + 1, currNode.z + 1));
        }
        if(currNode.z - 1 >= 0) neighbourList.Add(GetNode(currNode.x, currNode.z - 1));
        if(currNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currNode.x, currNode.z + 1));
        return neighbourList;
    }

    private GridNode GetNode(int x, int y) {
        return grid.GetGridObject(x, y);
    }

    private List<GridNode> CalcPath(GridNode endNode) {
        List<GridNode> path = new List<GridNode>();
        path.Add(endNode);
        GridNode currNode = endNode;
        while(currNode.cameFromNode != null) {
            path.Add(currNode.cameFromNode);
            currNode = currNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalcDistance(GridNode a, GridNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private GridNode GetLowestFCostNode(List<GridNode> pathNodeList) {
        GridNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++) {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public Grid3D<GridNode> GetGrid() {
        return grid;
    }
}