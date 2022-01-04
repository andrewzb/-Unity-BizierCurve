using UnityEngine;

namespace Bizier.Structures {
    public struct CurvePointData {
        public CurvePointData(Vector3 pos, Vector3 dir, Vector3 normal, Vector3 right,
        Vector3 angleDirection) {
            this.pos = pos;
            this.dir = dir;
            this.right = right;
            this.normal = normal;
            this.angleDirection = angleDirection;
        }

        private Vector3 pos;
        public Vector3 Position => pos;

        private Vector3 dir;
        public Vector3 Direction => dir;

        private Vector3 normal;
        public Vector3 Normal => normal;

        private Vector3 right;
        public Vector3 Right => right;

        private Vector3 angleDirection;
        public Vector3 AngleDirection => angleDirection;
    }
}