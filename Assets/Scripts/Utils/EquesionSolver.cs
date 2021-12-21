using System;
using UnityEngine;
using Bizier.Enums;

namespace Bizier.Uttils {
    public static class EquesionSolver {

        public static WorkFlowResult TrySolveQuadraticEquesion(
            float a, float b, float c, out Tuple<float, float> result) {
            var disc = b * b - 4 * a * c;
            if (disc < 0 || a == 0) {
                result = null;
                return WorkFlowResult.Failure;
            } else {
                return SolveSimple(a, b, disc, out result);
            }
        }

        public static WorkFlowResult SolveSimple(
            float a, float b, float disc, out Tuple<float, float> result) {
            var rootDisc = Mathf.Sqrt(disc);
            result = Tuple.Create(
                (-b + rootDisc) / (2 * a),
                (-b - rootDisc) / (2 * a)
            );
            return WorkFlowResult.Success;
        }
    }
}