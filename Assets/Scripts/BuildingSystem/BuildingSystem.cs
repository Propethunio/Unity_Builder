using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildingSystem : MonoBehaviour {

    public static BuildingSystem Instance { get; private set; }

    [SerializeField] private List<BuildableObjectSO> buildableObjectSOList;
    [SerializeField] private LayerMask mouseColliderMask;

    private RoadFixerManager roadFixer;
    private BuildingGhostManager buildingGhost;
    private Grid3D<GridNode> grid;
    private BuildableObjectSO buildableObjectSO;
    private BuildableObjectSO.Dir dir;
    private bool placingRoad;
    private Vector3 startRoadPos;
    private Vector2 lastGridPos;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        grid = GridManager.Instance.GetGrid();
        buildingGhost = BuildingGhostManager.Instance;
        roadFixer = RoadFixerManager.Instance;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            TryBuild();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            buildableObjectSO = buildableObjectSOList[0];
            dir = BuildableObjectSO.Dir.Down;
            buildingGhost.RefreshVisual();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            buildableObjectSO = buildableObjectSOList[1];
            dir = BuildableObjectSO.Dir.Down;
            buildingGhost.RefreshVisual();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            buildableObjectSO = buildableObjectSOList[2];
            dir = BuildableObjectSO.Dir.Down;
            buildingGhost.RefreshVisual();
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)) {
            buildableObjectSO = buildableObjectSOList[3];
            dir = BuildableObjectSO.Dir.Down;
            buildingGhost.RefreshVisual();
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            dir = BuildableObjectSO.GetNextDir(dir);
        }
        if(Input.GetKeyDown(KeyCode.X)) {
            DestroyObject();
        }
        if(placingRoad) {
            Vector3 mousePosition = GetMouseWorldPosition3D();
            grid.GetXZ(mousePosition, out int x, out int z);
            Vector2Int currGridPos = new Vector2Int(x, z);
            if(lastGridPos != currGridPos) {
                lastGridPos = currGridPos;
                List<Vector3> path = PathfindingSystem.Instance.FindPath(startRoadPos, mousePosition);
                buildingGhost.RefreshRoad(path);
            }
        }
    }

    private void TryBuild() {
        Vector3 mousePosition = GetMouseWorldPosition3D();
        grid.GetXZ(mousePosition, out int x, out int z);
        GridNode node = grid.GetGridObject(x, z);
        if(node == null) {
            return;
        }
        if(buildableObjectSO.isRoad) {
            if(node.CanBuild() || node.isRoad) {
                BuildRoad(mousePosition, x, z);
                return;
            }
        }
        bool canBuild = true;
        List<Vector2Int> gridPositionList = buildableObjectSO.GetGridPositionList(new Vector2Int(x, z), dir);
        foreach(Vector2Int gridPosition in gridPositionList) {
            if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                canBuild = false;
                break;
            }
        }
        if(canBuild) {
            BuildBuilding(x, z, gridPositionList);
        }
    }

    private void BuildRoad(Vector3 mousePosition, int x, int z) {
        if(!placingRoad) {
            placingRoad = true;
            startRoadPos = mousePosition;
            lastGridPos = new Vector2Int(x, z);
        } else {
            placingRoad = false;
            buildingGhost.CleanOldVisual();
            List<Vector3> posList = PathfindingSystem.Instance.FindPath(startRoadPos, mousePosition);
            List<GridNode> gridNodes = new List<GridNode>();
            foreach(Vector3 pos in posList) {
                grid.GetXZ(pos, out x, out z);
                GridNode node = grid.GetGridObject(x, z);
                node.isRoad = true;
                gridNodes.Add(node);
            }
            foreach(GridNode node in gridNodes) {
                roadFixer.RefreshNeighborns(node.x, node.z, gridNodes);
                roadFixer.RefreshRoadNode(node);
            }
            //buildableObjectSO = buildableObjectSOList[3];
            buildingGhost.RefreshVisual();
        }
    }

    private void BuildBuilding(int x, int z, List<Vector2Int> gridPositionList) {
        Vector2Int rotationOffset = buildableObjectSO.GetRotationOffset(dir);
        Vector3 objectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
        BuildedObject buildedObject = BuildedObject.CreateBuilding(objectWorldPosition, new Vector2Int(x, z), dir, buildableObjectSO);
        GridNode node;
        foreach(Vector2Int gridPosition in gridPositionList) {
            node = grid.GetGridObject(gridPosition.x, gridPosition.y);
            node.SetBuildedObject(buildedObject);
            node.isWalkable = false;
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
                if(node.isRoad) {
                    node.isRoad = false;
                    roadFixer.RefreshNeighborns(node.x, node.z, new List<GridNode>());
                }
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
        return buildableObjectSO.GetRotationAngle(dir);
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