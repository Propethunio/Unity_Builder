using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildedObject : MonoBehaviour {

    private BuildableObjectSO buildableObjectSO;
    private Vector2Int origin;
    private BuildableObjectSO.Dir dir;

    public static BuildedObject CreateBuilding(Vector3 worldPosition, Vector2Int origin, BuildableObjectSO.Dir dir, BuildableObjectSO buildableObjectSO) {
        BuildedObject buildedObject = Instantiate(buildableObjectSO.prefab, worldPosition, Quaternion.Euler(0, buildableObjectSO.GetRotationAngle(dir), 0)).GetComponent<BuildedObject>();
        buildedObject.buildableObjectSO = buildableObjectSO;
        buildedObject.origin = origin;
        buildedObject.dir = dir;
        return buildedObject;
    }

    public List<Vector2Int> GetGridPositionList() {
        return buildableObjectSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}