using System;
using UnityEngine;

namespace Bizier.Structures {
    [Serializable]
    public struct BizierBound {
        public BizierBound(Vector3 center, Vector3 size) {
            this.center = center;
            this.size = size;
        }

        [SerializeField] private Vector3 center;
        public Vector3 Center => center;

        [SerializeField] private Vector3 size;
        public Vector3 Size => size;
    }
}

