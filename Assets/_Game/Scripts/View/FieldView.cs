using System.Collections.Generic;
using _Game.Scripts.Model;
using GeneralUtils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Game.Scripts.View {
    public class FieldView : SingletonBehaviour<FieldView> {
        [SerializeField] private Vector3Int _initialPosition;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tile _tile;

        [SerializeField] private Vector3 _tileViewOffset;
        [SerializeField] private TileView _tileViewPrefab;
        [SerializeField] private Transform _tileViewsParent;
        private readonly List<TileView> _tileViews = new List<TileView>();

        public void Load(Field field) {
            Clear();

            var size = field.Size;
            for (var row = 0; row < size.x; row++) {
                for (var column = 0; column < size.y; column++) {
                    // here wee can basically modify axis directions
                    var tilePosition = _initialPosition + new Vector3Int(column, row, 0);
                    _tilemap.SetTile(tilePosition, _tile);

                    var tileView = Instantiate(_tileViewPrefab, _tileViewsParent);
                    tileView.transform.position = _tilemap.CellToWorld(tilePosition) + _tileViewOffset;
                    tileView.Load(field, new Vector2Int(row, column));
                    _tileViews.Add(tileView);
                }
            }
        }

        public void Clear() {
            _tilemap.ClearAllTiles();

            foreach (var tileView in _tileViews) {
                Destroy(tileView.gameObject);
            }

            _tileViews.Clear();
        }
    }
}