using UnityEngine;

namespace Bizier.Examples {
    public class SliceSnapper :MonoBehaviour {
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private BizierDrawer bizierDrawer;

        [SerializeField] private Transform snapAnchore;
        [SerializeField] [Range(0.1f, 1f)] private float treshold;
        [SerializeField] private int segmentPersistance;

        [SerializeField] private float drawGap;

        private float currentT;

        private void Update() {
            bizierDrawer.Draw(Mathf.Clamp01(currentT - 0.5f * drawGap), Mathf.Clamp01(currentT + 0.5f * drawGap), 5);

            if (pathCreator.TryGetClosest(snapAnchore.position, out var t, treshold, segmentPersistance)) {
                var pointData = pathCreator.GetCurvePointData(t);
                if (t > currentT && t < currentT + 0.1f) {
                    currentT = t;
                }
            }
        }

        [ContextMenu("Reset")]
        private void ResetSnap() {
            currentT = 0;
        }
    }
}
