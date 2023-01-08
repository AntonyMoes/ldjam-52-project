using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Model;
using GeneralUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.FlowerInfo {
    public class PlantInfoPanel : UIElement {
        [SerializeField] private TextMeshProUGUI _plantName;

        [SerializeField] private GridLayoutGroup _rangeParent;
        [SerializeField] private RangeItem _rangeItemPrefab;
        private readonly List<RangeItem> _rangeItems = new List<RangeItem>();

        public void Load(Plant plant) {
            Clear();
            _plantName.text = plant.Name;
            
            LoadRange(plant.Range);
            // LoadResourceGroup
            // LoadResourceGroup
        }

        private void LoadRange(Vector2Int[] range) {
            var actualRange = range.Length != 0 ? range : new[] { Vector2Int.zero };

            var minX = Mathf.Min(actualRange.Min(offset => offset.x), 0);
            var minY = Mathf.Min(actualRange.Min(offset => offset.y), 0);
            var maxX = Mathf.Max(actualRange.Max(offset => offset.x), 0);
            var maxY = Mathf.Max(actualRange.Max(offset => offset.y), 0);

            var min = new Vector2Int(minX, minY);
            var max = new Vector2Int(maxX, maxY);
            var size = max - min + Vector2Int.one;

            const int spacing = 10;
            const int maxCellSize = 100;
            var gridTransform = (RectTransform) _rangeParent.transform;
            var gridSize = gridTransform.rect.size;
            var maybeCellSizes = (gridSize - spacing * (size - Vector2Int.one)) / size;
            var cellSize = Mathf.Min(Mathf.FloorToInt(Mathf.Min(maybeCellSizes.x, maybeCellSizes.y)), maxCellSize);

            var neededGridSize = cellSize * size + spacing * (size - Vector2Int.one);
            var padding = gridSize - neededGridSize;
            _rangeParent.spacing = spacing * Vector2.one ;
            _rangeParent.padding.left = Mathf.FloorToInt(padding.y / 2);
            _rangeParent.padding.bottom = Mathf.FloorToInt(padding.x / 2);
            _rangeParent.cellSize = cellSize * Vector2.one;
            _rangeParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _rangeParent.constraintCount = size.x;

            foreach (var position in size.Iterate()) {
                var rangePosition = position + min;

                RangeItem.State state;
                if (rangePosition == Vector2Int.zero) {
                    state = RangeItem.State.Plant;
                } else if (range.Contains(rangePosition)) {
                    state = RangeItem.State.Affected;
                } else {
                    state = RangeItem.State.Empty;
                }

                var rangeItem = Instantiate(_rangeItemPrefab,_rangeParent.transform);
                rangeItem.Load(state);
                _rangeItems.Add(rangeItem);
            }
        }

        private void LoadResourceGroup() {
            
        }

        public override void Clear() {
            foreach (var rangeItem in _rangeItems) {
                Destroy(rangeItem.gameObject);
            }

            _rangeItems.Clear();
        }
    }
}