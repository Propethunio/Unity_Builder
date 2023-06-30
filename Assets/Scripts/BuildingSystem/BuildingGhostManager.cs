using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhostManager : MonoBehaviour {

    public static BuildingGhostManager Instance { get; private set; }

    private GameObject ghostObject;
    private List<BuildedObject> ghostRoadList;
    private List<GridNode> roadNodesList;
    private Grid3D<GridNode> grid;
    private BuildingSystem buildingSystem;
    private RoadFixerManager roadFixer;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        grid = GridManager.Instance.GetGrid();
        buildingSystem = BuildingSystem.Instance;
        roadFixer = RoadFixerManager.Instance;
        ghostRoadList = new List<BuildedObject>();
        roadNodesList = new List<GridNode>();
        RefreshVisual();
    }

    private void LateUpdate() {
        if(ghostObject != null) {
            Vector3 ghostPosition = buildingSystem.GetMouseOverGridPosition() + buildingSystem.GetOffset();
            ghostPosition.y += 1f;
            ghostObject.transform.position = Vector3.Lerp(ghostObject.transform.position, ghostPosition, Time.deltaTime * 15f);
            ghostObject.transform.rotation = Quaternion.Lerp(ghostObject.transform.rotation, Quaternion.Euler(0, buildingSystem.GetBuildableRotation(), 0), Time.deltaTime * 15f);
        }
    }

    public void RefreshVisual() {
        CleanOldVisual();
        BuildableObjectSO buildableObjectSO = buildingSystem.GetBuildableObjectSO();
        if(buildableObjectSO != null) {
            ghostObject = Instantiate(buildableObjectSO.visual, buildingSystem.GetMouseOverGridPosition() + buildingSystem.GetOffset(), Quaternion.Euler(0, buildingSystem.GetBuildableRotation(), 0));
        }
    }

    public void RefreshRoad(List<Vector3> RoadList) {
        CleanOldVisual();
        foreach(Vector3 road in RoadList) {
            grid.GetXZ(road, out int x, out int z);
            GridNode node = grid.GetGridObject(x, z);
            node.isTempRoad = true;
            roadNodesList.Add(node);
        }
        foreach(GridNode roadNode in roadNodesList) {
            Vector3 objectWorldPosition = roadFixer.SetRoadVisual(roadNode.x, roadNode.z) + new Vector3(0, 0.01f, 0);
            BuildableObjectSO buildableObjectSO = roadFixer.GetBuildableRoadSO();
            int rotation = roadFixer.GetRoadRotation();
            BuildedObject buildedObject = Instantiate(buildableObjectSO.visual, objectWorldPosition, Quaternion.Euler(0, rotation, 0)).GetComponent<BuildedObject>();
            ghostRoadList.Add(buildedObject);
        }
    }

    public void CleanOldVisual() {
        if(ghostObject != null) {
            Destroy(ghostObject);
        }
        if(ghostRoadList.Count > 0) {
            foreach(BuildedObject ghostRoad in ghostRoadList) {
                ghostRoad.DestroySelf();
            }
            ghostRoadList.Clear();
        }
        if(roadNodesList.Count > 0) {
            foreach(GridNode node in roadNodesList) {
                node.isTempRoad = false;
            }
            roadNodesList.Clear();
        }
    }
}