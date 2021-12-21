using UnityEngine;

namespace Bizier.Examples {
    public class AnimControl :MonoBehaviour {
        [SerializeField] private Animator animatorSlice;
        [SerializeField] private string animFlatName;

        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private Transform tracable;
        [SerializeField] private Transform snaper;
        [SerializeField] private float treshold;

        private float currentT;

        private void Update() {
            if (pathCreator.TryGetClosest(snaper.position, out var t)) {
                var pointData = pathCreator.GetCurvePointData(t);

                if (t > currentT && t < currentT + treshold) {
                    tracable.position = pointData.Position;
                    //tracable.forward = pointData.Direction;
                    tracable.up = pointData.NormalDir;
                    currentT = t;

                    animatorSlice.SetFloat(animFlatName, t);
                }
            }
        }

        [ContextMenu("Reset")]
        private void ResetSnap() {
            currentT = 0;
        }
    }
}
