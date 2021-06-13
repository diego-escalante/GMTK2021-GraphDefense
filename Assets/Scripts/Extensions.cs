using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static void SetColor(this LineRenderer lineRenderer, Color color) {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public static float Length(this LineRenderer lineRenderer) {
        return Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
    }

}
