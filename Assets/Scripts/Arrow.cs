using System.Collections;
using System.Collections.Generic;
using System.Xml;
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

        arrows.Add(this);
    }


    public static List<Arrow> arrows = new List<Arrow>();

    public static void CleanArrows()
    {
        if (arrows == null || arrows.Count == 0) return;

        foreach (Arrow arrow in arrows)
        {
            if (arrow != null)
            {
                Destroy(arrow.gameObject);
            }
        }
    }
}
