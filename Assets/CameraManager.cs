using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform targetTransform; //object camera will be following

    public Transform cameraPivot; // the object the camera uses to pivot (Look up and Look down)
    private float defaultPosition;
    public Transform cameraTransform; // The transform of the autal camera object in the scene

    public float cameraCollisionOffSet = 0.2f; // how much the camera will jump off of object its is colliding with
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public LayerMask collisionLayers; // the layers we want our camers to collide with


    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraFollowSpeed = 0.2f;
    public float lookAngle; // Camera Looking Up and Down
    public float pivotAngle; // Camera Looking LEft and Right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;


    public float cameraXInput;
    public float cameraYInput;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;
    InputManager inputManager;


    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z; 
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position,targetTransform.position,ref cameraFollowVelocity,cameraFollowSpeed);
        transform.position = targetPos;
    }

    private void RotateCamera()
    {
        cameraXInput = inputManager.cameraInputX;
        cameraYInput = inputManager.cameraInputY;

        lookAngle = lookAngle + (cameraXInput * cameraLookSpeed);
        pivotAngle = pivotAngle - (cameraYInput * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
            {

            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = targetPosition - (distance - cameraCollisionOffSet);


        }

        if(Mathf.Abs(targetPosition)< minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
