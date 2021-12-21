using System.Collections.Generic;
using UnityEngine;

namespace Bizier.Examples{
    [ExecuteAlways]
    public class BizierDrawer :MonoBehaviour {
        [SerializeField] private bool isDrawCurve;
        [SerializeField] private bool isApplyLoop;
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments;

        public void Draw(float tStart, float tEnd, int segmentCount) {
            var pointList = new List<Vector3>();
            var fraction = (tEnd - tStart) / segmentCount;
            for (int i = 0; i < segmentCount; i++) {
                var t = tStart + fraction * i;
                var data = pathCreator.GetCurvePointData(t);
                pointList.Add(data.Position);
            }
            lineRenderer.positionCount = pointList.Count;
            for (int i = 0; i < pointList.Count; i++) {
                lineRenderer.SetPosition(i, pointList[i]);
            }
            if (isApplyLoop) {
                lineRenderer.loop = pathCreator.IsClosed;
            } else {
                lineRenderer.loop = false;
            }
        }

        public void Clear() {
            lineRenderer.positionCount = 0;
        }

        public void OnValidate() {
            if (isDrawCurve) {
                Draw(0, 1, segments);
            } else {
                Clear();
            }
        }
    }
}