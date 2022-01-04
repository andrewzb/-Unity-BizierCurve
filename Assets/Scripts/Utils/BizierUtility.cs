using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Bizier.Enums;
using Bizier.Structures;
using Bizier.Utils.Core;

namespace Bizier.Utils {
    public static class BizierUtility {
        public static Vector3 GetBuizierPoint(Vector3[] points, float factor) {
            return BizierUtilityCore.GetBuizierPoint(
                points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierPoint(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            return BizierUtilityCore.GetBuizierPoint(
                p0, p1, p2, p3, factor);
        }


        public static Vector3 GetBuizierFirstDerivative(Vector3[] points, float factor) {
            return BizierUtilityCore.GetBuizierFirstDerivative(
                points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierFirstDerivative(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            return BizierUtilityCore.GetBuizierFirstDerivative(
                p0, p1, p2, p3, factor);
        }

        public static Vector3 GetBuizierSecondDerivative(Vector3[] points, float factor) {
            return BizierUtilityCore.GetBuizierSecondDerivative(points[0], points[1], points[2], points[3], factor);
        }

        public static Vector3 GetBuizierSecondDerivative(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor) {
            return BizierUtilityCore.GetBuizierSecondDerivative(
             p0, p1, p2, p3, factor);
        }


        public static float GetBuizierLength(Vector3[] points, int count) {
            return BizierUtilityCore.GetBuizierLength(
                points[0], points[1], points[2], points[3], count);
        }

        public static float GetBizierSegmentArchLength(Vector3[] points, int segmentCount) {
            return BizierUtilityCore.GetBizierSegmentArchLength(
                points[0], points[1], points[2], points[3], segmentCount);
        }


        public static float GetBizierSegmentArchLength(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentCount) {
            return BizierUtilityCore.GetBizierSegmentArchLength(
                p0, p1, p2, p3, segmentCount);
        }

        public static float GetBuizierLength(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int count) {
            return BizierUtilityCore.GetBuizierLength(
               p0, p1, p2, p3, count);
        }

        public static BizierBound GetBizierBound(Vector3[] points) {
            return BizierUtilityCore.GetBizierBound(points[0], points[1], points[2], points[3]);
        }

        public static BizierBound GetBizierBound(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            return  BizierUtilityCore.GetBizierBound(p0, p1, p2, p3);
        }

        public static bool GetIsColide(BizierBound bound, Vector3 point,
        CollisionErrorType collisionErrorType, float colisionErrorFactor = 1f) {
            return BizierUtilityCore.GetIsColide(
                bound, point, collisionErrorType, colisionErrorFactor);
        }

        public static BizierBound GetBound(IEnumerable<BizierBound> bounds) {
            return BizierUtilityCore.GetBound(bounds);
        }
    }
}
