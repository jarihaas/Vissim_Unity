using UnityEngine;

public class TrackingCamController : MonoBehaviour
{
    public Transform trackedObject;
    public Vector3 offset = new Vector3(0f, 4f, -10f);

    void LateUpdate()
    {
        if (trackedObject == null)
            return;

        transform.position = trackedObject.position + offset;
        transform.LookAt(trackedObject.position);
    }
}
