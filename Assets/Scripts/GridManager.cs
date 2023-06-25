using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    [SerializeField] private bool showDebug;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 originPosition;

    private Grid3D<GridNode> grid;

    private void Start() {
        grid = new Grid3D<GridNode>(width, height, cellSize, originPosition, showDebug, (Grid3D<GridNode> g, int x, int z) => new GridNode(g, x, z));
    }

    public Grid3D<GridNode> GetGrid() {
        return grid;
    }
}