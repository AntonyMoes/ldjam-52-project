using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Model;
using GeneralUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class FlowerInfoPanel : UIElement {
        [SerializeField] private Image _flower;
        [SerializeField] private TextMeshProUGUI _flowerName;

        [SerializeField] private GridLayoutGroup _rangeParent;
        [SerializeField] private RangeItem _rangeItemPrefab;
        private readonly List<RangeItem> _rangeItems = new List<RangeItem>();

        public void Load(Plant plant) {
            LoadRange(plant.Range);
            // LoadResourceGroup
            // LoadResourceGroup
        }

        private void LoadRange(Vector2Int[] range) {
            var minX = range.Min(offset => offset.x);
            var minY = range.Min(offset => offset.y);
            var maxX = range.Max(offset => offset.x);
            var maxY = range.Max(offset => offset.y);

            var min = new Vector2Int(minX, minY);
            var max = new Vector2Int(maxX, maxY);
            var size = max - min + Vector2Int.one;

            const int spacing = 10;
            var gridTransform = (RectTransform) _rangeParent.transform;
            var gridSize = gridTransform.rect.size;
            var maybeCellSizes = (gridSize - spacing * (size - Vector2Int.one)) / size;
            var cellSize = Mathf.FloorToInt(Mathf.Min(maybeCellSizes.x, maybeCellSizes.y));

            var neededGridSize = cellSize * size + spacing * (size - Vector2Int.one);
            var padding = gridSize - neededGridSize;
            _rangeParent.padding.left = Mathf.FloorToInt(padding.x / 2);
            _rangeParent.padding.bottom = Mathf.FloorToInt(padding.y / 2);

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