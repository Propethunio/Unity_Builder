using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class CharacterPathfindingMovementHandler : MonoBehaviour {

    private const float speed = 40f;
    private int index;
    private List<Vector3> pathVectorList;

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        if(pathVectorList != null) {
            Vector3 targetPosition = pathVectorList[index];
            if(Vector3.Distance(transform.position, targetPosition) > .05f) {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position += moveDir * speed * Time.deltaTime;
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
        pathVectorList = Pathfinding.Instance.FindPath(transform.position, targetPosition);
        if(pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }
}