using UnityEngine;

namespace Bizier.Examples {
    [ExecuteAlways]
    public class PathUpdateSubsciption : MonoBehaviour {
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private bool isSubscribe;

        private void OnPathUpdate() => Debug.Log("path updated");

        private void OnValidate() {
            if (isSubscribe) {
                pathCreator.PathUpdateAction += OnPathUpdate;
            } else {
                pathCreator.PathUpdateAction -= OnPathUpdate;

            }
        }
    }
}
