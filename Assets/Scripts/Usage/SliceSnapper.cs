using UnityEngine;

namespace Bizier.Examples {
    public class SliceSnapper :MonoBehaviour {
        [SerializeField] private BizierDrawer bizierDrawer;

        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private Transform tracable;
        [SerializeField] private Transform snaper;
        [SerializeField] private float treshold;

        [SerializeField] private float drawGap;

        private float currentT;

        private void Update() {
            bizierDrawer.Draw(Mathf.Clamp01(currentT - 0.5f * drawGap), Mathf.Clamp01(currentT + 0.5f * drawGap), 5);

            if (pathCreator.TryGetClosest(snaper.position, out var t)) {
                var pointData = pathCreator.GetCurvePointData(t);

                if (t > currentT && t < currentT + treshold) {
                    //tracable.position = pointData.Position;
                    //tracable.forward = pointData.Direction;
                    //tracable.up = pointData.NormalDir;
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
