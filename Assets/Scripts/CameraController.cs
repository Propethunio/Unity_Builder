using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float lerpSpeedMovement;
    [SerializeField] private float lerpSpeedRotation;
    [SerializeField] private float lerpSpeedZoom;
    [SerializeField] private float rotationAmount;
    [SerializeField] private Vector3 zoomAmount;

    private Quaternion newRotation;
    private Vector3 newPosition;
    private Vector3 newZoom;
    private Transform cameraTransform;

    private void Start() {
        cameraTransform = transform.GetChild(0);
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void Update() {
        HandleMovementInput();
        HandleRotationInput();
        HandleZoomInput();
    }

    private void HandleMovementInput() {
        if(Input.GetKey(KeyCode.W)) {
            newPosition += (transform.forward * moveSpeed);
        }
        if(Input.GetKey(KeyCode.S)) {
            newPosition += (transform.forward * -moveSpeed);
        }
        if(Input.GetKey(KeyCode.A)) {
            newPosition += (transform.right * -moveSpeed);
        }
        if(Input.GetKey(KeyCode.D)) {
            newPosition += (transform.right * moveSpeed);
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * lerpSpeedMovement);
    }

    private void HandleRotationInput() {
        if(Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if(Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.down * rotationAmount);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * lerpSpeedRotation);
    }

    private void HandleZoomInput() {
        if(Input.GetKey(KeyCode.Z)) {
            newZoom += zoomAmount;
        }
        if(Input.GetKey(KeyCode.C)) {
            newZoom -= zoomAmount;
        }
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * lerpSpeedZoom);
    }
}