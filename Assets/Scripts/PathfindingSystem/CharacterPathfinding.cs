using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPathfinding : MonoBehaviour {

    private const float baseSpeed = 30f;
    private int index;
    private List<Vector3> pathVectorList;

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        if(pathVectorList != null) {
            Vector3 targetPosition = pathVectorList[index];
            if(Vector3.Distance(transform.position, targetPosition) > .1f) {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position += moveDir * baseSpeed * Time.deltaTime;
            } else {
                index++;
                if(index >= pathVectorList.Count) {
                    pathVectorList = null;
                }
            }
        }
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        index = 0;
        pathVectorList = PathfindingSystem.Instance.FindPath(transform.position, targetPosition);
        if(pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }
}