using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float optimalDist;
    private float targetDist;
    private Transform cameraObject;
    private int mapLayer;
    private float camZoomSpeed;
    private Vector3 dir;
    void Awake()
    {
        cameraObject = transform.GetChild(0);
        optimalDist = 9.25f;
        targetDist = optimalDist;
        mapLayer = 1 << 3;
        camZoomSpeed = 5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        dir = (cameraObject.position - transform.parent.position).normalized;
        if (Physics.Raycast(transform.parent.position, dir, out hit, Mathf.Infinity, mapLayer))
        {
            targetDist = Mathf.Min(hit.distance * hit.distance, optimalDist);
        }
        else
        {
            targetDist = optimalDist;
        }

        if (targetDist > cameraObject.localPosition.sqrMagnitude)
        {
            cameraObject.position += dir * camZoomSpeed * Time.deltaTime;
        }
        if (targetDist < cameraObject.localPosition.sqrMagnitude)
        {
            cameraObject.position -= dir * camZoomSpeed * Time.deltaTime;
        }
    }
}
