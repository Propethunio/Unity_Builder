using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class HeatMapVisual : MonoBehaviour {
    private GridMap grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate() {
        if(updateMesh) {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    public void SetGrid(GridMap grid) {
        this.grid = grid;
        UpdateHeatMapVisual();
        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, GridMap.OnGridValueChangedEventArgs e) {
        updateMesh = true;
    }

    private void UpdateHeatMapVisual() {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        for(int x = 0; x < grid.GetWidth(); x++) {
            for(int y = 0; y < grid.GetHeight(); y++) {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                int gridValue = grid.GetValue(x, y);
                float gridValueNormalized = (float)gridValue / GridMap.HEAT_MAP_MAX_VALUE;
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
*/