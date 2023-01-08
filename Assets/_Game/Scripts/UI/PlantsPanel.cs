using System;
using System.Collections.Generic;
using System.Linq;
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
        private Action<Plant, DragComponent> _onDrag;
        private Action<Plant, DragComponent, DropComponent> _onDrop;

        private Action _stopDrag;

        public void Load(Dictionary<Plant, int> plants, Plant targetPlant, Action<Plant, DragComponent> onDrag, Action<Plant, DragComponent, DropComponent> onDrop) {
            foreach (var (plant, count) in plants) {
                var plantItem = Instantiate(_plantItemPrefab, _plantItemsRoot);
                plantItem.Load(plant, count, OnStartDrag, OnStopDrag);
                _plantItems.Add(plantItem);
            }

            _targetPlantItem.Load(targetPlant, null, OnStartDrag, OnStopDrag);

            _onDrag = onDrag;
            _onDrop = onDrop;
        }

        private void OnStartDrag(Plant plant, PointerEventData eventData) {
            var draggedPlant = Instantiate(_draggedPlantPrefab);
            _onDrag?.Invoke(plant, draggedPlant.DragComponent);

            var drag = new Drag(() => DragComponent.MouseWorldPoint, out _stopDrag);
            draggedPlant.Load(plant, drag, _onDrop);
        }

        private void OnStopDrag() {
            _stopDrag();
            _stopDrag = null;
        }

        public void OnPlanted(Plant plant) {
            var item = _plantItems.FirstOrDefault(item => item.Plant == plant);
            if (item != null) {
                if (--item.Count == 0) {
                    Destroy(item.gameObject);
                    _plantItems.Remove(item);
                }
            }
        }

        public override void Clear() {
            _targetPlantItem.Clear();

            foreach (var plantItem in _plantItems) {
                plantItem.Clear();
                Destroy(plantItem.gameObject);
            }

            _plantItems.Clear();
        }
    }
}