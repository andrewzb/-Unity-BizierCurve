using System;
using UnityEngine;
using System.Collections.Generic;

namespace Bizier.Examples{
    //[ExecuteAlways]
    public class BizierDrawer : MonoBehaviour {
        [SerializeField] private bool isDrawCurve;
        [SerializeField] private bool isApplyLoop;
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments;

        [SerializeField] [Range(0, 1)] private float start;
        [SerializeField] [Range(0, 1)] private float end;

        [SerializeField] private float t;
        [SerializeField] private float length;
        [SerializeField] private int seg;

        /*
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
        */

        /*
        public void Draw() {
            var pointList = new List<Vector3>();
            var fraction = (end - start) / segments;
            for (int i = 0; i < segments; i++) {
                var t = start + fraction * i;
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
        */

        private void FixedUpdate() {
            Draw();
        }

        public void Draw() {
            var pointsData = pathCreator.GetBzizierPointsData(t, length, seg);
            lineRenderer.positionCount = pointsData.Count;
            for (int i = 0; i < pointsData.Count; i++) {
                lineRenderer.SetPosition(i, pointsData[i].Position);
            }
        }

        public void Clear() {
            lineRenderer.positionCount = 0;
        }

        private void OnEnable() {
            ToggleSubscription(true);
        }

        private void OnDisable() {
            ToggleSubscription(false);
        }

        private void OnDestroy() {
            ToggleSubscription(false);
        }

        public void ToggleSubscription(bool isSubscribe) {
            /*
            pathCreator.PathUpdateAction -= Draw;
            if (isSubscribe) {
                pathCreator.PathUpdateAction += Draw;
            }
            */
        }



        /*
        public void OnValidate() {
            if (isDrawCurve) {
                //Draw(start, end, segments);
            } else {
                Clear();
            }
        }
        */
    }
}