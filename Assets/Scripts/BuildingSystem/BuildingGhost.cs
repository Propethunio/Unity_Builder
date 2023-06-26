using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    [SerializeField] BuildingSystem testing;
    private GameObject ghostObject;

    private void Start() {
        RefreshVisual();
        testing.OnBuildableChange += Testing_OnBuildableChange;
    }

    private void Testing_OnBuildableChange(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        if(ghostObject != null) {
            Vector3 ghostPosition = testing.GetMouseOverGridPosition() + testing.GetOffset();
            ghostPosition.y += 1f;
            ghostObject.transform.position = Vector3.Lerp(ghostObject.transform.position, ghostPosition, Time.deltaTime * 15f);
            ghostObject.transform.rotation = Quaternion.Lerp(ghostObject.transform.rotation, Quaternion.Euler(0, testing.GetBuildableRotation(), 0), Time.deltaTime * 15f);
        }
    }

    private void RefreshVisual() {
        if(ghostObject != null) {
            Destroy(ghostObject);
        }
        BuildableObjectSO buildableObjectSO = testing.GetBuildableObjectSO();
        if(buildableObjectSO != null) {
            ghostObject = Instantiate(buildableObjectSO.visual, testing.GetMouseOverGridPosition() + testing.GetOffset(), Quaternion.Euler(0, testing.GetBuildableRotation(), 0));
        }
    }
}