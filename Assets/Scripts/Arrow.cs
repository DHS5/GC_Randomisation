using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    public void Init(Transform origin, Transform destination)
    {
        Vector3 originPos = origin.position;
        Vector3 destinationPos = destination.position;

        _lineRenderer.SetPosition(0, new Vector3(originPos.x, 0.1f, originPos.z));
        _lineRenderer.SetPosition(1, new Vector3(destinationPos.x, 0.1f, destinationPos.z));
    }
}
