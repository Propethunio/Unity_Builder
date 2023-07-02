using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool placingFarm;
    private Vector3 startBuildPos;
    private Vector2 lastBuildPos;

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
        if(Input.GetKeyDown(KeyCode.Alpha5)) {
            buildableObjectSO = buildableObjectSOList[4];
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
            if(lastBuildPos != currGridPos) {
                lastBuildPos = currGridPos;
                List<Vector3> path = PathfindingSystem.Instance.FindPath(startBuildPos, mousePosition);
                buildingGhost.RefreshRoad(path);
            }
        }
        if(placingFarm) {
            Vector3 mousePosition = GetMouseWorldPosition3D();
            grid.GetXZ(mousePosition, out int x, out int z);
            Vector2Int currGridPos = new Vector2Int(x, z);
            if(lastBuildPos != currGridPos) {
                lastBuildPos = currGridPos;
                List<GridNode> farmNodes = GetFarmNodes(startBuildPos, mousePosition);
                buildingGhost.RefreshFarm(farmNodes);
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
        if(buildableObjectSO.isFarm) {
            if(node.CanBuild() || node.isFarm) {
                BuildFarm(mousePosition, x, z);
                return;
            }
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

    private void BuildFarm(Vector3 mousePosition, int x, int z) {
        if(!placingFarm) {
            placingFarm = true;
            startBuildPos = mousePosition;
            lastBuildPos = new Vector2Int(x, z);
            return;
        }
        List<GridNode> gridNodes = GetFarmNodes(startBuildPos, mousePosition);
        bool canBuild = true;
        foreach(GridNode node in gridNodes) {
            if(!node.CanBuild() && !node.isFarm) {
                canBuild = false;
                break;
            }
        }
        if(canBuild) {
            foreach(GridNode node in gridNodes) {
                if(!node.isFarm) {
                    node.isFarm = true;
                    Vector2Int rotationOffset = buildableObjectSO.GetRotationOffset(dir);
                    Vector3 objectWorldPosition = grid.GetWorldPosition(node.x, node.z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                    BuildedObject buildedObject = BuildedObject.CreateBuilding(objectWorldPosition, new Vector2Int(node.x, node.z), dir, buildableObjectSO);
                    node.SetBuildedObject(buildedObject);
                }
            }
            placingFarm = false;
            buildingGhost.RefreshVisual();
        }
    }

    private List<GridNode> GetFarmNodes(Vector3 startPos, Vector3 endPos) {
        List<GridNode> nodes = new List<GridNode>();
        grid.GetXZ(startPos, out int startX, out int startZ);
        grid.GetXZ(endPos, out int endX, out int endZ);
        for(int x = Mathf.Min(startX, endX); x <= Mathf.Max(startX, endX); x++) {
            for(int z = Mathf.Min(startZ, endZ); z <= Mathf.Max(startZ, endZ); z++) {
                GridNode node = grid.GetGridObject(x, z);
                if(node == null) {
                    return null;
                }
                nodes.Add(grid.GetGridObject(x, z));
            }
        }
        return nodes;
    }

    private void BuildRoad(Vector3 mousePosition, int x, int z) {
        if(!placingRoad) {
            placingRoad = true;
            startBuildPos = mousePosition;
            lastBuildPos = new Vector2Int(x, z);
        } else {
            placingRoad = false;
            buildingGhost.CleanOldVisual();
            List<Vector3> posList = PathfindingSystem.Instance.FindPath(startBuildPos, mousePosition);
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