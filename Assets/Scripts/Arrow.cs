using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [Space(5f)]
    [SerializeField] private Gradient _colorGradient;

    private int rank;
    public void Init(Transform origin, Transform destination, int _rank)
    {
        rank = _rank;

        Vector3 originPos = origin.position;
        Vector3 destinationPos = destination.position;

        Vector3 dir = destinationPos - originPos;
        float offset = 0.6f;
        float dist = dir.magnitude - 2 * offset;
        dir = dir.normalized;

        originPos.y = 0.35f;

        _lineRenderer.SetPosition(0, originPos + (offset * dir));
        _lineRenderer.SetPosition(1, originPos + ((offset + dist * 0.89f) * dir));
        _lineRenderer.SetPosition(2, originPos + ((offset + dist * 0.9f) * dir));
        _lineRenderer.SetPosition(3, originPos + ((offset + dist) * dir));
        _lineRenderer.startColor = _colorGradient.Evaluate((float)rank / 8);
        _lineRenderer.endColor = _colorGradient.Evaluate((float)rank / 8);

        arrows.Add(this);

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        _lineRenderer.enabled = active;

        if (active)
        {
            _lineRenderer.startColor = _colorGradient.Evaluate((float)rank / 8);
            _lineRenderer.endColor = _colorGradient.Evaluate((float)rank / 8);
        }
    }
    public void SetActive(bool active, int pathRank)
    {
        _lineRenderer.enabled = active;

        if (active)
        {
            _lineRenderer.startColor = _colorGradient.Evaluate((float)pathRank / 8);
            _lineRenderer.endColor = _colorGradient.Evaluate((float)pathRank / 8);
        }
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
