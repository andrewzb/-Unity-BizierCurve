using UnityEngine;
using Bizier.Utils.Core;

namespace Bizier.Utils {
    public static class MathHelper {
        public static bool GetIsPointForvard(Vector3[] points, float factor, Vector3 point) {
            return MathHelperCore.GetIsPointForvard(
                points[0], points[1], points[2], points[3], factor, point);
        }

        public static bool GetIsPointForvard(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor, Vector3 point) {
            return MathHelperCore.GetIsPointForvard(p0, p1, p2, p3, factor, point);
        }

        public static float GetDistanceToNormal(
        Vector3 planeNormal, Vector3 planePos, Vector3 pointPos) {
            return MathHelperCore.GetDistanceToNormal(planeNormal, planePos, pointPos);
        }

        public static float GetAndgle(Vector3 origin, Vector3 up, Vector3 right,
        Vector3 pos, float radius, bool clocWise = true) {
            return MathHelperCore.GetAndgle(origin, up, right, pos, radius, clocWise);
        }

        public static Vector3 GetNormalHandlePosition(
        Vector3 pos, Vector3 upDir, Vector3 rightDir, float radius, float angle) {
            return MathHelperCore.GetNormalHandlePosition(pos, upDir, rightDir, radius, angle);
        }

        public static Vector3 GetSnapPosition(
        Vector3 normalOrigin, Vector3 normalDir, Vector3 pos, float radius) {
            return MathHelperCore.GetSnapPosition(normalOrigin, normalDir, pos, radius);
        }

        public static float GetAngleDiff(float starAngle, float endAngle) {
            return MathHelperCore.GetAngleDiff(starAngle, endAngle);
        }

        public static float Loop01(float t) {
            return MathHelperCore.Loop01(t);
        }
    }
}
