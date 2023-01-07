using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using _Game.Scripts.View;
using GeneralUtils;
using GeneralUtils.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.UI {
    public class PlantsPanel : UIElement {
        [SerializeField] private Transform _plantItemsRoot;
        [SerializeField] private PlantItem _plantItemPrefab;
        [SerializeField] private PlantItem _targetPlantItem;
        [SerializeField] private DraggedPlant _draggedPlantPrefab;

        private readonly List<PlantItem> _plantItems = new List<PlantItem>();
        private Action _onTargetPlanted;

        private Action _stopDrag;

        public void Load(Dictionary<Plant, int> plants, Plant targetPlant, Action onTargetPlanted) {
            foreach (var (plant, count) in plants) {
                var plantItem = Instantiate(_plantItemPrefab, _plantItemsRoot);
                plantItem.Load(plant, count, OnStartDrag, OnStopDrag);
                _plantItems.Add(plantItem);
            }

            _targetPlantItem.Load(targetPlant, 1, OnStartDrag, OnStopDrag);
            _onTargetPlanted = onTargetPlanted;
        }

        private void OnStartDrag(Plant plant, PointerEventData eventData) {
            var draggedPlant = Instantiate(_draggedPlantPrefab);
            var drag = new Drag(() => DragComponent.MouseWorldPoint, out _stopDrag);
            draggedPlant.Load(plant, drag, OnDrop);
        }

        private void OnStopDrag() {
            _stopDrag();
            _stopDrag = null;
        }

        private void OnDrop(Plant plant, DragComponent dragComponent, DropComponent dropComponent) {
            Destroy(dragComponent.gameObject);
            if (dropComponent == null || !dropComponent.TryGetComponent<TileView>(out var tileView)) {
                Debug.LogWarning("NOWHERE TO PLANT");
                return;
            }

            if (tileView.TryPlant(plant)) {
                Debug.LogWarning($"PLANTED AT {tileView.Position}");
                var item = _plantItems.FirstOrDefault(item => item.Plant == plant);
                if (item != null) {
                    if (--item.Count == 0) {
                        Destroy(item.gameObject);
                    }
                } else if (_targetPlantItem.Plant == plant) {
                    // TODO WIN
                    _onTargetPlanted?.Invoke();
                }
            } else {
                Debug.LogWarning($"COULD NOT PLANT AT {tileView.Position}! Requirements: {plant.Requirements.Serialize()}; Resources: {tileView.Resources.Serialize()}");
            }
        }

        public override void Clear() {
            foreach (var plantItem in _plantItems) {
                Destroy(plantItem.gameObject);
            }

            _plantItems.Clear();
        }
    }
}