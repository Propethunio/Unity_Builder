using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;

public class Grid3D<GridObject> {

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private GridObject[,] gridArray;

    public Grid3D(int width, int height, float cellSize, Vector3 originPosition, bool showDebug, Func<Grid3D<GridObject>, int, int, GridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new GridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++) {
            for(int z = 0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }
        if(showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for(int x = 0; x < gridArray.GetLength(0); x++) {
                for(int z = 0; z < gridArray.GetLength(1); z++) {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void SetGridObject(Vector3 worldPosition, GridObject value) {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public void SetGridObject(int x, int z, GridObject value) {
        if(x >= 0 && z >= 0 && x < width && z < height) {
            gridArray[x, z] = value;
        }
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public GridObject GetGridObject(Vector3 worldPosition) {
        GetXZ(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }

    public GridObject GetGridObject(int x, int z) {
        if(x >= 0 && z >= 0 && x < width && z < height) {
            return gridArray[x, z];
        } else return default(GridObject);
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }
}