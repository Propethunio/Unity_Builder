using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode {

    private Grid3D<GridNode> grid;
    public int x;
    public int z;
    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;
    public GridNode cameFromNode;
    private BuildedObject buildedObject;

    public GridNode(Grid3D<GridNode> grid, int x, int z) {
        this.grid = grid;
        this.x = x;
        this.z = z;
        isWalkable = true;
    }

    public void SetBuildedObject(BuildedObject buildedObject) {
        this.buildedObject = buildedObject;
    }

    public void ClearBuildedObject() {
        this.buildedObject = null;
    }

    public bool CanBuild() {
        return buildedObject == null;
    }

    public BuildedObject GetBuildedObject() {
        return buildedObject;
    }

    public override string ToString() {
        return x + "," + z;
    }

    public void CalcFCost() {
        fCost = gCost + hCost;
    }
}