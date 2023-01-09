using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.DragAndDrop;
using _Game.Scripts.Model;
using _Game.Scripts.UI;
using _Game.Scripts.UI.PlantInfo;
using _Game.Scripts.View;
using GeneralUtils.Processes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts {
    public class LevelRunner {
        private LevelData _data;
        private Action _onComplete;
        private Plant _targetPlant;
        private Field _field;
        private PlantsPanel _plantsPanel;
        // private ResourceShowPanel _resourceShowPanel;
        private MainMenuPanel _mainMenuPanel;
        private RestartPanel _restartPanel;

        private Plant _draggedPlant;
        private DragComponent _dragComponent;
        private TileView _currentTile;
        private Action _tryStartTutorial;
        private PlantInfoPanel _plantInfoPanel;
        private Dictionary<Plant, int> _plants;
        private TutorialController _tutorialController;
        private AudioSource _source;

        public void StartLevel(LevelData data, Action onComplete, Action<Vector2Int> focusOnBoard, TutorialController tutorialController, Action tryStartTutorial) {
            _data = data;
            _onComplete = onComplete;

            _field = new Field(_data.Field);
            _targetPlant = GetPlantByName(_data.targetPlant);
            _plants = _data.availablePlants.ToDictionary(plant => GetPlantByName(plant.name), plant => plant.count);

            focusOnBoard?.Invoke(_field.Size);

            _tryStartTutorial = tryStartTutorial;
            _tutorialController = tutorialController;
            Show(tryStartTutorial);

            var clip = SoundController.Instance.LevelStartClip;
            if (_source != null && _source.clip == clip) {
                _source.Stop();
            }
            _source = SoundController.Instance.PlaySound(clip, 0.4f);
        }

        private void Show(Action onDone = null) {
            FieldView.Instance.Load(_field);
            FieldView.Instance.Show();
            _plantsPanel = UIController.Instance.ShowPlantsPanel(_plants, _targetPlant, OnDrag, OnDrop);
            // _resourceShowPanel = UIController.Instance.ShowResourceShowPanel();

            _mainMenuPanel = UIController.Instance.ShowMainMenuPanel(EndLevel);
            _restartPanel = UIController.Instance.ShowRestartPanel(Restart);
            
            _tutorialController.LoadActions(new Dictionary<TutorialController.TutorialAction, Func<Process>> {
                [TutorialController.TutorialAction.DisableButtons] = () => new SyncProcess(DisableButtons),
                [TutorialController.TutorialAction.EnableButtons] = () => new SyncProcess(EnableButtons),
                [TutorialController.TutorialAction.AutoShowInfo] = () => new SyncProcess(AutoShowInfo),
                [TutorialController.TutorialAction.AutoHideInfo] = () => new SyncProcess(AutoHideInfo),
            });

            onDone?.Invoke();
        }

        private void Hide(Action onDone = null) {
            // TODO add animations
            _plantsPanel.Hide();
            // _resourceShowPanel.Hide();
            _mainMenuPanel.Hide();
            _restartPanel.Hide();

            FieldView.Instance.Hide(() => {
                FieldView.Instance.Clear();
                onDone?.Invoke();
            });
        }

        private void Restart() {
            Hide();
            StartLevel(_data, _onComplete, null, _tutorialController, _tryStartTutorial);
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

            var isTargetPlant = plant == _targetPlant;
            if (tileView.PlantAt(plant, out var plantProcess, isTargetPlant)) {
                _plantsPanel.OnPlanted(plant);
                Debug.LogWarning($"PLANTED AT {tileView.Position}");

                plantProcess.Run(() => {
                    if (isTargetPlant) {
                        OnLevelWon();
                    }
                });
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

                if (_currentTile != null && _currentTile.CanPlant(plant)) {
                    _currentTile.RemovePreview();
                }

                fieldView.HideAffectedTiles();
                _currentTile = currentTile;
                if (_currentTile != null && _currentTile.CanPlant(plant)) {
                    _currentTile.PreviewPlant(plant, plant == _targetPlant);
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
        
        // --------------------------------- TUTORIAL --------------------------------- //

        private void DisableButtons() {
            _mainMenuPanel.SetButtons(false);
            _restartPanel.SetButtons(false);
        }

        private void EnableButtons() {
            _mainMenuPanel.SetButtons(true);
            _restartPanel.SetButtons(true);
        }

        private void AutoShowInfo() {
            var plantToShow = _plants.FirstOrDefault().Key ?? _targetPlant;
            _plantInfoPanel = UIController.Instance.ShowPlantInfoPanel(plantToShow, plantToShow == _targetPlant);
            _plantInfoPanel.AutoShown = true;
        }

        private void AutoHideInfo() {
            _plantInfoPanel.AutoShown = false;
        }
    }
}