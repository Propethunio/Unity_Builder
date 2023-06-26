using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingSystem : MonoBehaviour {

    public event EventHandler OnBuildableChange;

    [SerializeField] private List<BuildableObjectSO> buildableObjectSOList;
    [SerializeField] private LayerMask mouseColliderMask;
    private Grid3D<GridNode> grid;
    private BuildableObjectSO buildableObjectSO;
    private BuildableObjectSO.Dir dir = BuildableObjectSO.Dir.Down;

    private void Start() {
        grid = GridManager.Instance.GetGrid();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            BuildObject();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            buildableObjectSO = buildableObjectSOList[0];
            OnBuildableChange?.Invoke(this, EventArgs.Empty);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            buildableObjectSO = buildableObjectSOList[1];
            OnBuildableChange?.Invoke(this, EventArgs.Empty);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            buildableObjectSO = buildableObjectSOList[2];
            OnBuildableChange?.Invoke(this, EventArgs.Empty);
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            dir = BuildableObjectSO.GetNextDir(dir);
        }
        if(Input.GetKeyDown(KeyCode.X)) {
            DestroyObject();
        }
    }

    private void BuildObject() {
        Vector3 mousePosition = GetMouseWorldPosition3D();
        grid.GetXZ(mousePosition, out int x, out int z);
        List<Vector2Int> gridPositionList = buildableObjectSO.GetGridPositionList(new Vector2Int(x, z), dir);
        bool canBuild = true;
        foreach(Vector2Int gridPosition in gridPositionList) {
            if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                canBuild = false;
                break;
            }
        }
        if(canBuild) {
            Vector2Int rotationOffset = buildableObjectSO.GetRotationOffset(dir);
            Vector3 objectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            BuildedObject buildedObject = BuildedObject.Create(objectWorldPosition, new Vector2Int(x, z), dir, buildableObjectSO);
            GridNode node;
            foreach(Vector2Int gridPosition in gridPositionList) {
                node = grid.GetGridObject(gridPosition.x, gridPosition.y);
                node.SetBuildedObject(buildedObject);
                node.isWalkable = false;
            }
        }
    }

    private void DestroyObject() {
        Vector3 mousePosition = GetMouseWorldPosition3D();
        GridNode node = grid.GetGridObject(mousePosition);
        BuildedObject buildedObject = node.GetBuildedObject();
        if(buildedObject != null) {
            buildedObject.DestroySelf();
            List<Vector2Int> gridPositionList = buildedObject.GetGridPositionList();
            foreach(Vector2Int gridPosition in gridPositionList) {
                node = grid.GetGridObject(gridPosition.x, gridPosition.y);
                node.ClearBuildedObject();
            }
        }

    }

    private Vector3 GetMouseWorldPosition3D() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderMask)) {
            return raycastHit.point;
        } else return Vector3.zero;
    }

    public Vector3 GetMouseOverGridPosition() {
        Vector3 mousePosition = GetMouseWorldPosition3D();
        grid.GetXZ(mousePosition, out int x, out int z);
        Vector3 objectWorldPosition = grid.GetWorldPosition(x, z);
        return objectWorldPosition;
    }

    public int GetBuildableRotation() {
        int rotation = buildableObjectSO.GetRotationAngle(dir);
        return rotation;
    }

    public BuildableObjectSO GetBuildableObjectSO() {
        return buildableObjectSO;
    }

    public Vector3 GetOffset() {
        Vector2Int offset = buildableObjectSO.GetRotationOffset(dir);
        Vector3 rotationOffset = new Vector3(offset.x, 0, offset.y) * grid.GetCellSize();
        return rotationOffset;
    }
}