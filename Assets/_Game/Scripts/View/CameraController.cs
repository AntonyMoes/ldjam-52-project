using UnityEngine;

namespace _Game.Scripts.View {
    public class CameraController : MonoBehaviour {
        [SerializeField] private Vector2 _initialPosition;
        [SerializeField] private Grid _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _bounds;

        public void FocusOnBoard(Vector2Int size) {
            var center = (Vector2) (size - Vector2Int.one) / 2f;

            var halfCellSize = (Vector2) _grid.cellSize / 2f;
            var position = _initialPosition + center.x * halfCellSize * new Vector2(-1, 1) + center.y * halfCellSize;

            _camera.transform.position = (Vector3) position + (Vector3.forward * _camera.transform.position.z);
            _camera.orthographicSize = Mathf.Max(size.x, size.y) + _bounds * 2;
        }
    }
}