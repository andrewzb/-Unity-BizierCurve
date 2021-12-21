using System;
using UnityEngine;

namespace Bizier.Structures {
    [Serializable]
    public struct PathCreatoreSettings {
        public PathCreatoreSettings(
            bool isDisplayAnchorPoints,
            bool isDisplayControlPoints,
            bool isDisplayAnchoreNumber,
            bool isShowPathBounds,
            bool isShowPerSegmentBounds,
            bool isShowPerSegmentColisionBound,
            bool isShowNormalsHandles,
            bool isShowNormals) {
            this.isDisplayAnchorPoints = isDisplayAnchorPoints;
            this.isDisplayControlPoints = isDisplayControlPoints;
            this.isDisplayAnchoreNumber = isDisplayAnchoreNumber;
            this.isShowPathBounds = isShowPathBounds;
            this.isShowPerSegmentBounds = isShowPerSegmentBounds;
            this.isShowPerSegmentColisionBound = isShowPerSegmentColisionBound;
            this.isShowNormalsHandles = isShowNormalsHandles;
            this.isShowNormals = isShowNormals;
        }

        [SerializeField] private bool isDisplayAnchorPoints;
        public bool IsDisplayAnchorPoints => isDisplayAnchorPoints;

        [SerializeField] private bool isDisplayControlPoints;
        public bool IsDisplayControlPoints => isDisplayControlPoints;

        [SerializeField] private bool isDisplayAnchoreNumber;
        public bool IsDisplayAnchoreNumber => isDisplayAnchoreNumber;

        [SerializeField] private bool isShowPathBounds;
        public bool IsShowPathBounds => isShowPathBounds;

        [SerializeField] private bool isShowPerSegmentBounds;
        public bool IsShowPerSegmentBounds => isShowPerSegmentBounds;

        [SerializeField] private bool isShowPerSegmentColisionBound;
        public bool IsShowPerSegmentColisionBound => isShowPerSegmentColisionBound;

        [SerializeField] private bool isShowNormalsHandles;
        public bool IsShowNormalsHandles => isShowNormalsHandles;

        [SerializeField] private bool isShowNormals;
        public bool IsShowNormals => isShowNormals;
    }
}
