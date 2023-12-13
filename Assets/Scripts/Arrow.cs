using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Arrow : MonoBehaviour
{
    private static readonly List<Arrow> Arrows = new();

    [FormerlySerializedAs("_lineRenderer")] [SerializeField]
    private LineRenderer lineRenderer;

    [FormerlySerializedAs("_colorGradient")] [Space(5f)] [SerializeField]
    private Gradient colorGradient;

    private int _rank;

    public void Init(Transform origin, Transform destination, int rank)
    {
        _rank = rank;

        var originPos = origin.position;
        var destinationPos = destination.position;

        var dir = destinationPos - originPos;
        const float offset = 0.6f;
        var dist = dir.magnitude - 2 * offset;
        dir = dir.normalized;

        originPos.y = 0.35f;

        lineRenderer.SetPosition(0, originPos + offset * dir);
        lineRenderer.SetPosition(1, originPos + (offset + dist * 0.89f) * dir);
        lineRenderer.SetPosition(2, originPos + (offset + dist * 0.9f) * dir);
        lineRenderer.SetPosition(3, originPos + (offset + dist) * dir);
        lineRenderer.startColor = colorGradient.Evaluate((float)_rank / 8);
        lineRenderer.endColor = colorGradient.Evaluate((float)_rank / 8);

        Arrows.Add(this);

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        lineRenderer.enabled = active;

        if (!active) return;
        lineRenderer.startColor = colorGradient.Evaluate((float)_rank / 8);
        lineRenderer.endColor = colorGradient.Evaluate((float)_rank / 8);
    }

    public void SetActive(bool active, int pathRank)
    {
        lineRenderer.enabled = active;

        if (!active) return;
        lineRenderer.startColor = colorGradient.Evaluate((float)pathRank / 8);
        lineRenderer.endColor = colorGradient.Evaluate((float)pathRank / 8);
    }

    public static void CleanArrows()
    {
        if (Arrows == null || Arrows.Count == 0) return;

        foreach (var arrow in Arrows.Where(arrow => arrow != null))
            Destroy(arrow.gameObject);
    }
}