using UnityEngine;

namespace Bizier.Utils.Core {
    public static class MathHelperCore {
        public static bool GetIsPointForvard(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor, Vector3 point) {
            var factorPoint = BizierUtilityCore.GetBuizierPoint(p0, p1, p2, p3, factor);
            var directionPoint = BizierUtilityCore.GetBuizierFirstDerivative(
                p0, p1, p2, p3, factor);
            var dist = GetDistanceToNormal(directionPoint, factorPoint, point);
            return dist >= 0;
        }

        public static float GetDistanceToNormal(
        Vector3 planeNormal, Vector3 planePos, Vector3 pointPos) {
            var c = Mathf.Sqrt(Mathf.Pow(planeNormal.x, 2)
                + Mathf.Pow(planeNormal.y, 2) + Mathf.Pow(planeNormal.z, 2));
            var b = planeNormal.x * planePos.x
                + planeNormal.y * planePos.y + planeNormal.z * planePos.z;
            var a = planeNormal.x * pointPos.x
                + planeNormal.y * pointPos.y + planeNormal.z * pointPos.z;
            var dist = ((a - b) / c);
            return dist;
        }

        public static float GetAndgle(Vector3 origin, Vector3 up, Vector3 right,
        Vector3 pos, float radius, bool clocWise = true) {
            var yDist = GetDistanceToNormal(up, origin, pos) / radius;
            var xDist = GetDistanceToNormal(right, origin, pos) / radius;

            var baseAngle = 0f;
            var angle = Mathf.Acos(yDist) * Mathf.Rad2Deg;
            if (!float.IsNaN(angle)) {
                if (xDist >= 0) {
                    baseAngle = angle;
                } else {
                    baseAngle = 360 - angle;
                }
            }

            return baseAngle;
        }

        public static Vector3 GetNormalHandlePosition(
        Vector3 pos, Vector3 upDir, Vector3 rightDir, float radius, float angle) {
            var radAngle = Mathf.Deg2Rad * angle;
            var newPos = pos + (radius * rightDir * Mathf.Sin(radAngle))
                + (radius * upDir * Mathf.Cos(radAngle));
            return newPos;
        }

        public static Vector3 GetSnapPosition(
        Vector3 normalOrigin, Vector3 normalDir, Vector3 pos, float radius) {
            var dist = GetDistanceToNormal(normalDir, normalOrigin, pos);
            var newPos = pos + normalDir * dist;
            var direction = (newPos - normalOrigin).normalized;
            return normalOrigin + direction * radius;
        }

        public static float GetAngleDiff(float starAngle, float endAngle) {
            var diff = (endAngle - starAngle) % 360;
            if (Mathf.Abs(diff) > 180) {
                var isClockWise = diff > 0;
                diff = ((endAngle + 180 * (isClockWise ? -1 : 1))
                    - (starAngle + 180 * (isClockWise ? 1 : -1))) % 360;
            }

            return diff;
        }

        public static float Loop01(float t) {
            return t - Mathf.Floor(t);
        }
    }
}
