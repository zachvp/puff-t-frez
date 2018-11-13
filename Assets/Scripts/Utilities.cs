using UnityEngine;

public static class Utilities
{
    public static bool EqualVectors(Vector2 lhs, Vector2 rhs)
    {
        return Vector2.Dot(lhs, rhs) > Constants.Thresholds.DOT_THRESHOLD_MAX;
    }
}
