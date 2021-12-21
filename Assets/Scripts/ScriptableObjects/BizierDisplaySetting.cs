using UnityEngine;
namespace Bizier.Assets {
    [CreateAssetMenu(fileName = "BizierDisplaySetting", menuName = "ScriptableObjects/BizierDisplaySetting", order = 1)]
    public class BizierDisplaySetting :ScriptableObject {
        public float anchorSize;
        public float controlSize;
        public float normalSize;

        public Color anchoreFree;
        public Color anchoreSymetric;
        public Color anchoreSymetricDirection;
        public Color anchoreLabelNumber;

        public Color control;
        public Color controlLine;

        public Color bezierPath;

        public Color commonBox;
        public Color separetBox;
        public Color colisionBox;

        public Color normals;
        public Color normalHandle;
        public Color normalLabel;

        [Range(14, 98)] public int anchoreNumberLabelSize;
        [Range(14, 98)] public int LabelFontSize;
        [Range(0, 1)] public float normalsLength;
        [Range(1, 50)] public int normalSegmentPerUnit;

#if UNITY_EDITOR
        public static BizierDisplaySetting Load() {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BizierDisplaySetting");
            if (guids.Length == 0) {
                Debug.LogWarning("Could not find DisplaySettings asset. Will use default settings instead.");
                return ScriptableObject.CreateInstance<BizierDisplaySetting>();
            } else {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<BizierDisplaySetting>(path);
            }
        }
#endif
    }
}
