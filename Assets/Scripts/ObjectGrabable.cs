using UnityEngine;

public class ObjectGrabable : MonoBehaviour
{
    [SerializeField] private Rigidbody objectRigiBody;
    private float dragValue;
    private Transform objectGrabPoint;
    private Vector3 previousPosition;
    private Vector3 calculatedVelocity;

    private void Awake()
    {
        dragValue = objectRigiBody.linearDamping;
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        objectGrabPoint = objectGrabPointTransform;
        objectRigiBody.useGravity = false;
        objectRigiBody.linearVelocity = Vector3.zero;
        objectRigiBody.linearDamping = dragValue;
        previousPosition = transform.position;
    }

    public void Drop(Vector3 cameraForward)
    {
        objectGrabPoint = null;
        objectRigiBody.useGravity = true;

        float baseThrowForce = 5f;
        float swingMultiplier = 3f;

        Vector3 finalThrowVelocity = (calculatedVelocity * swingMultiplier) + (cameraForward * baseThrowForce);

        objectRigiBody.linearDamping = 0f;
        objectRigiBody.AddForce(finalThrowVelocity, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        if (objectGrabPoint != null)
        {
            float grabObjectMoveSpeed = 5f;
            Vector3 positionSmoothed = Vector3.Lerp(transform.position, objectGrabPoint.position, Time.deltaTime * grabObjectMoveSpeed);
            objectRigiBody.MovePosition(positionSmoothed);

            calculatedVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
            previousPosition = transform.position;
        }
    }
}
