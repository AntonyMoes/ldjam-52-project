using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using GeneralUtils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Game.Scripts.View {
    public class FieldView : SingletonBehaviour<FieldView> {
        [SerializeField] private Vector3Int _initialPosition;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tile _tile;

        [SerializeField] private Tilemap _availableMap;
        [SerializeField] private Tilemap _affectedMap;
        [SerializeField] private Tile _availableTile;
        [SerializeField] private Tile _affectedTile;
        [SerializeField] private Tile _currentTile;

        [SerializeField] private Vector3 _tileViewOffset;
        [SerializeField] private TileView _tileViewPrefab;
        [SerializeField] private Transform _tileViewsParent;
        private readonly List<TileView> _tileViews = new List<TileView>();

        private Field _field;

        private Vector3Int FieldToMapPosition(Vector2Int position) {
            return _initialPosition + new Vector3Int(position.y, position.x, 0);
        }
        
        private Vector2Int MapToFieldPosition(Vector3Int position) {
            var intermediatePosition = position - _initialPosition;
            return new Vector2Int(intermediatePosition.y, intermediatePosition.x);
        }

        public void Load(Field field) {
            Clear();
            _field = field;

            foreach (var position in _field.Iterate()) {
                // here wee can basically modify axis directions
                var tilePosition = FieldToMapPosition(position);
                _tilemap.SetTile(tilePosition, _tile);

                var tileView = Instantiate(_tileViewPrefab, _tileViewsParent);
                tileView.transform.position = _tilemap.CellToWorld(tilePosition) + _tileViewOffset;
                tileView.Load(_field, position);
                _tileViews.Add(tileView);
            }
        }
        
        public void ShowAvailableTiles(Plant plant) {
            foreach (var position in _field.Iterate()) {
                if (_field.CanPlantAt(plant, position)) {
                    _availableMap.SetTile(FieldToMapPosition(position), _availableTile);
                }
            }
        }

        public void HideAvailableTiles() {
            _availableMap.ClearAllTiles();
        }

        // public TileView GetCurrentTile() {
        //     // var worldPos = DragComponent.MouseWorldPoint - _tileViewOffset;
        //     // worldPos.z = 0f;
        //     // var mapPosition = _tilemap.WorldToCell(worldPos);
        //     // var position = MapToFieldPosition(mapPosition);
        //     // Debug.Log(position);
        //     // return _tileViews.FirstOrDefault(view => view.Position == position);
        // }

        public void ShowAffectedTiles(Plant plant, TileView tileView) {
            var updates = _field.GetUpdates(plant, tileView.Position);
            foreach (var (offset, update) in updates) {
                var position = tileView.Position + offset;
                var view = _tileViews.First(view => view.Position == position);
                view.ShowResources(update);

                _affectedMap.SetTile(FieldToMapPosition(position), _affectedTile);
            }
        }

        public void HideAffectedTiles() {
            _affectedMap.ClearAllTiles();
        }

        public void Clear() {
            _tilemap.ClearAllTiles();

            HideAvailableTiles();

            foreach (var tileView in _tileViews) {
                Destroy(tileView.gameObject);
            }

            _tileViews.Clear();
        }
    }
}