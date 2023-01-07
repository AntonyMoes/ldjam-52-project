using System;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using _Game.Scripts.UI;
using _Game.Scripts.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts {
    public class LevelRunner {
        private LevelData _data;
        private Action _onComplete;
        private Plant _targetPlant;
        private Field _field;
        private PlantsPanel _plantsPanel;

        private Plant _draggedPlant;
        private DragComponent _dragComponent;
        private TileView _currentTile;

        public void StartLevel(LevelData data, Action onComplete) {
            _data = data;
            _onComplete = onComplete;

            _field = new Field(_data.Field);
            FieldView.Instance.Load(_field);

            _targetPlant = GetPlantByName(_data.targetPlant);
            var plants = _data.availablePlants.ToDictionary(plant => GetPlantByName(plant.name), plant => plant.count);
            _plantsPanel = UIController.Instance.ShowPlantsPanel(plants, _targetPlant, OnDrag, OnDrop);

            Plant GetPlantByName(string name) => new Plant(DataStorage.Instance.Plants.WithName(name));
        }

        private void OnDrag(Plant plant, DragComponent dragComponent) {
            _draggedPlant = plant;
            _dragComponent = dragComponent;
            FieldView.Instance.ShowAvailableTiles(plant);
        }
        
        private void OnDrop(Plant plant, DragComponent dragComponent, DropComponent dropComponent) {
            _draggedPlant = null;
            FieldView.Instance.HideAvailableTiles();

            Object.Destroy(dragComponent.gameObject);
            if (dropComponent == null || !dropComponent.TryGetComponent<TileView>(out var tileView)) {
                Debug.LogWarning("NOWHERE TO PLANT");
                return;
            }

            if (tileView.PlantAt(plant)) {
                _plantsPanel.OnPlanted(plant);
                Debug.LogWarning($"PLANTED AT {tileView.Position}");
                if (plant == _targetPlant) {
                    // TODO maybe also some animations
                    _onComplete?.Invoke();
                }
            } else {
                Debug.LogWarning($"COULD NOT PLANT AT {tileView.Position}! Requirements: {plant.Requirements.Serialize()}; Resources: {tileView.Resources.Serialize()}");
            }
        }

        public void ProcessFrame(float _) {
            var fieldView = FieldView.Instance;
            if (_draggedPlant is {} plant) {
                var currentTile = _dragComponent.GetDraggedOver<TileView>().FirstOrDefault();
                if (_currentTile == currentTile) {
                    return;
                }

                fieldView.HideAffectedTiles();
                _currentTile = currentTile;
                if (_currentTile != null && _currentTile.CanPlant(plant)) {
                    fieldView.ShowAffectedTiles(plant, _currentTile);
                }
            } else if (_currentTile != null) {
                fieldView.HideAffectedTiles();
                _currentTile = null;
            }
        }
    }
}