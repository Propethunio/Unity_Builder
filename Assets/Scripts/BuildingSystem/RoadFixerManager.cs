using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadFixerManager : MonoBehaviour {

    public static RoadFixerManager Instance { get; private set; }

    [SerializeField] private BuildableObjectSO roadBasic;
    [SerializeField] private BuildableObjectSO roadEnd;
    [SerializeField] private BuildableObjectSO roadStraight;
    [SerializeField] private BuildableObjectSO roadTurn;
    [SerializeField] private BuildableObjectSO roadShapeT;
    [SerializeField] private BuildableObjectSO roadShapeX;

    private Grid3D<GridNode> grid;
    private BuildableObjectSO buildableObjectSO;
    private BuildableObjectSO.Dir dir;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        grid = GridManager.Instance.GetGrid();
    }

    public void RefreshNeighborns(int x, int z, List<GridNode> nodeList) {
        List<GridNode> neighborNodesList = GetNeighborNodesList(x, z);
        foreach(GridNode neighborNode in neighborNodesList) {
            if(neighborNode != null && !nodeList.Contains(neighborNode)) {
                if(neighborNode.isRoad) {
                    RefreshRoadNode(neighborNode);
                }
            }
        }
    }

    private List<GridNode> GetNeighborNodesList(int x, int z) {
        List<GridNode> neighborNodesList = new List<GridNode>();
        neighborNodesList.Add(grid.GetGridObject(x, z + 1));
        neighborNodesList.Add(grid.GetGridObject(x, z - 1));
        neighborNodesList.Add(grid.GetGridObject(x - 1, z));
        neighborNodesList.Add(grid.GetGridObject(x + 1, z));
        return neighborNodesList;
    }

    public void RefreshRoadNode(GridNode node) {
        BuildedObject buildedObject = node.GetBuildedObject();
        if(buildedObject != null) {
            buildedObject.DestroySelf();
        }
        Vector3 objectWorldPosition = SetRoadVisual(node.x, node.z);
        buildedObject = BuildedObject.CreateBuilding(objectWorldPosition, new Vector2Int(node.x, node.z), dir, buildableObjectSO);
        node.SetBuildedObject(buildedObject);
    }

    public Vector3 SetRoadVisual(int x, int z) {
        List<GridNode> neighborNodesList = GetNeighborNodesList(x, z);
        List<bool> isRoadBoolList = new List<bool>();
        int connections = 0;
        foreach(GridNode neighborNode in neighborNodesList) {
            if(neighborNode != null) {
                if(neighborNode.isRoad || neighborNode.isTempRoad) {
                    connections++;
                    isRoadBoolList.Add(true);
                    continue;
                }
            }
            isRoadBoolList.Add(false);
        }
        SetRoadShapeAndDir(connections, isRoadBoolList);
        Vector2Int rotationOffset = buildableObjectSO.GetRotationOffset(dir);
        Vector3 objectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
        return objectWorldPosition;
    }

    private void SetRoadShapeAndDir(int connections, List<bool> isRoadBoolList) {
        if(connections == 0) {
            buildableObjectSO = roadBasic;
        } else if(connections == 1) {
            SetRoadConnection1(isRoadBoolList);
        } else if(connections == 2) {
            SetRoadConnection2(isRoadBoolList);
        } else if(connections == 3) {
            SetRoadConnection3(isRoadBoolList);
        } else {
            buildableObjectSO = roadShapeX;
        }
    }

    private void SetRoadConnection1(List<bool> isRoadBoolList) {
        buildableObjectSO = roadEnd;
        bool top = isRoadBoolList[0];
        bool down = isRoadBoolList[1];
        bool left = isRoadBoolList[2];
        if(top) {
            dir = BuildableObjectSO.Dir.Top;
        } else if(down) {
            dir = BuildableObjectSO.Dir.Down;
        } else if(left) {
            dir = BuildableObjectSO.Dir.Left;
        } else {
            dir = BuildableObjectSO.Dir.Right;
        }
    }

    private void SetRoadConnection2(List<bool> isRoadBoolList) {
        bool top = isRoadBoolList[0];
        bool down = isRoadBoolList[1];
        bool left = isRoadBoolList[2];
        if(top) {
            if(down) {
                buildableObjectSO = roadStraight;
                dir = BuildableObjectSO.Dir.Down;
            } else if(left) {
                buildableObjectSO = roadTurn;
                dir = BuildableObjectSO.Dir.Top;
            } else {
                buildableObjectSO = roadTurn;
                dir = BuildableObjectSO.Dir.Right;
            }
        } else if(down) {
            if(left) {
                buildableObjectSO = roadTurn;
                dir = BuildableObjectSO.Dir.Left;
            } else {
                buildableObjectSO = roadTurn;
                dir = BuildableObjectSO.Dir.Down;
            }
        } else {
            buildableObjectSO = roadStraight;
            dir = BuildableObjectSO.Dir.Left;
        }
    }

    private void SetRoadConnection3(List<bool> isRoadBoolList) {
        buildableObjectSO = roadShapeT;
        bool top = isRoadBoolList[0];
        bool down = isRoadBoolList[1];
        bool left = isRoadBoolList[2];
        if(!top) {
            dir = BuildableObjectSO.Dir.Down;
        } else if(!down) {
            dir = BuildableObjectSO.Dir.Top;
        } else if(!left) {
            dir = BuildableObjectSO.Dir.Right;
        } else {
            dir = BuildableObjectSO.Dir.Left;
        }
    }

    public BuildableObjectSO GetBuildableRoadSO() {
        return buildableObjectSO;
    }

    public int GetRoadRotation() {
        return buildableObjectSO.GetRotationAngle(dir);
    }
}