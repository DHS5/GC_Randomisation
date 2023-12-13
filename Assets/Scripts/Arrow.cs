using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private static readonly List<Arrow> Arrows = new();
    [SerializeField] private LineRenderer _lineRenderer;

    [Space(5f)] [SerializeField] private Gradient _colorGradient;

    private int _rank;

    public void Init(Transform origin, Transform destination, int _rank)
    {
        this._rank = _rank;

        var originPos = origin.position;
        var destinationPos = destination.position;

        var dir = destinationPos - originPos;
        const float offset = 0.6f;
        var dist = dir.magnitude - 2 * offset;
        dir = dir.normalized;

        originPos.y = 0.35f;

        _lineRenderer.SetPosition(0, originPos + offset * dir);
        _lineRenderer.SetPosition(1, originPos + (offset + dist * 0.89f) * dir);
        _lineRenderer.SetPosition(2, originPos + (offset + dist * 0.9f) * dir);
        _lineRenderer.SetPosition(3, originPos + (offset + dist) * dir);
        _lineRenderer.startColor = _colorGradient.Evaluate((float)this._rank / 8);
        _lineRenderer.endColor = _colorGradient.Evaluate((float)this._rank / 8);

        Arrows.Add(this);

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        _lineRenderer.enabled = active;

        if (!active) return;
        _lineRenderer.startColor = _colorGradient.Evaluate((float)_rank / 8);
        _lineRenderer.endColor = _colorGradient.Evaluate((float)_rank / 8);
    }

    public void SetActive(bool active, int pathRank)
    {
        _lineRenderer.enabled = active;

        if (!active) return;
        _lineRenderer.startColor = _colorGradient.Evaluate((float)pathRank / 8);
        _lineRenderer.endColor = _colorGradient.Evaluate((float)pathRank / 8);
    }

    public static void CleanArrows()
    {
        if (Arrows == null || Arrows.Count == 0) return;

        foreach (var arrow in Arrows.Where(arrow => arrow != null))
            Destroy(arrow.gameObject);
    }
}