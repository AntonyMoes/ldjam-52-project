using System;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using _Game.Scripts.UI;
using _Game.Scripts.UI.PlantInfo;
using _Game.Scripts.View;
using log4net.Filter;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts {
    public class LevelRunner {
        private LevelData _data;
        private Action _onComplete;
        private Plant _targetPlant;
        private Field _field;
        private PlantsPanel _plantsPanel;
        private ResourceShowPanel _resourceShowPanel;
        private MainMenuPanel _mainMenuPanel;
        private RestartPanel _restartPanel;

        private Plant _draggedPlant;
        private DragComponent _dragComponent;
        private TileView _currentTile;

        public void StartLevel(LevelData data, Action onComplete) {
            _data = data;
            _onComplete = onComplete;

            _field = new Field(_data.Field);
            _targetPlant = GetPlantByName(_data.targetPlant);

            Show();
        }

        private void Show() {
            FieldView.Instance.Load(_field);

            var plants = _data.availablePlants.ToDictionary(plant => GetPlantByName(plant.name), plant => plant.count);
            _plantsPanel = UIController.Instance.ShowPlantsPanel(plants, _targetPlant, OnDrag, OnDrop);
            _resourceShowPanel = UIController.Instance.ShowResourceShowPanel();

            _mainMenuPanel = UIController.Instance.ShowMainMenuPanel(EndLevel);
            _restartPanel = UIController.Instance.ShowRestartPanel(Restart);
        }

        public void Hide(Action onDone = null) {
            // TODO add animations
            _plantsPanel.Hide();
            _resourceShowPanel.Hide();
            _mainMenuPanel.Hide();
            _restartPanel.Hide();
            
            FieldView.Instance.Clear();
            
            onDone?.Invoke();
        }

        private void Restart() {
            Hide();
            StartLevel(_data, _onComplete);
        }

        private void EndLevel() {
            Hide(() => UIController.Instance.ShowMainMenuWindow());
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
                    OnLevelWon();
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

        private void OnLevelWon() {
            Hide();
            _onComplete?.Invoke();
        }
        
        private static Plant GetPlantByName(string name) => new Plant(DataStorage.Instance.Plants.WithName(name));
    }
}