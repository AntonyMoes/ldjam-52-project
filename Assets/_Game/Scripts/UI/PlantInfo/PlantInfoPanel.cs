using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Model;
using GeneralUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.Scripts.UI.PlantInfo {
    public class PlantInfoPanel : UIElement {
        [SerializeField] private TextMeshProUGUI _plantName;

        [SerializeField] private GridLayoutGroup _rangeParent;
        [SerializeField] private PlantInfoRangeItem _plantInfoRangeItemPrefab;

        [SerializeField] private PlantInfoResourceGroup _requiresGroup;
        [SerializeField] private PlantInfoResourceGroup _affectsGroup;

        private readonly List<PlantInfoRangeItem> _rangeItems = new List<PlantInfoRangeItem>();

        // shit
        private bool _autoShown;

        public bool AutoShown {
            get => _autoShown;
            set {
                _autoShown = value;
                if (_autoShown) {
                    _wannaHide = true;
                } if (_autoShown == false && _wannaHide) {
                    Hide();
                }
            }
        }

        private bool _wannaHide;

        public void Load(Plant plant) {
            Clear();
            _plantName.text = plant.Name;
            
            LoadRange(plant.Range);
            _requiresGroup.Load(plant.Requirements, false);
            _affectsGroup.Load(plant.Effect, true);
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

                PlantInfoRangeItem.State state;
                if (rangePosition == Vector2Int.zero) {
                    state = PlantInfoRangeItem.State.Plant;
                } else if (range.Contains(rangePosition)) {
                    state = PlantInfoRangeItem.State.Affected;
                } else {
                    state = PlantInfoRangeItem.State.Empty;
                }

                var rangeItem = Instantiate(_plantInfoRangeItemPrefab,_rangeParent.transform);
                rangeItem.Load(state);
                _rangeItems.Add(rangeItem);
            }
        }

        protected override void PerformShow(Action onDone = null) {
            _wannaHide = false;
            base.PerformShow(onDone);
        }

        public void TryHide() {
            if (AutoShown) {
                _wannaHide = true;
            } else {
                Hide();
            }
        }

        protected override void PerformHide(Action onDone = null) {
            _wannaHide = false;
            base.PerformHide(onDone);
        }

        public override void Clear() {
            _requiresGroup.Clear();
            _affectsGroup.Clear();

            foreach (var rangeItem in _rangeItems) {
                Destroy(rangeItem.gameObject);
            }

            _rangeItems.Clear();
        }
    }
}