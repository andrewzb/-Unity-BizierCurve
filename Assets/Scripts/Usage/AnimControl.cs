using UnityEngine;

namespace Bizier.Examples {
    public class AnimControl : MonoBehaviour {
        [SerializeField] private Animator animatorSlice;
        [SerializeField] private string animFlatName;

        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private Transform tracable;
        [SerializeField] private Transform snaper;

        [SerializeField] private float treshold;
        [SerializeField] private int segmentPersistance;

        private float currentT;

        private void Update() {
            if (pathCreator.TryGetClosest(snaper.position, out var t, treshold, segmentPersistance)) {
                var pointData = pathCreator.GetCurvePointData(t);
                if (t > currentT) {
                    tracable.position = pointData.Position;
                    //tracable.forward = pointData.Direction;
                    tracable.up = pointData.NormalDir;
                    currentT = t;
                    animatorSlice.SetFloat(animFlatName, t);
                    Debug.Log($"t => {t}");
                }
            }
        }

        [ContextMenu("Reset")]
        private void ResetSnap() {
            currentT = 0;
        }
    }
}
