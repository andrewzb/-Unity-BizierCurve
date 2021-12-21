using UnityEngine;

namespace Bizier.Structures {
    public struct CurvePointData {
        public CurvePointData(Vector3 pos, Vector3 dir, Vector3 normalDir) {
            this.pos = pos;
            this.dir = dir;
            this.normalDir = normalDir;
        }

        private Vector3 pos;
        public Vector3 Position => pos;

        private Vector3 dir;
        public Vector3 Direction => dir;

        private Vector3 normalDir;
        public Vector3 NormalDir => normalDir;
    }
}