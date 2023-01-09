using System;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using _Game.Scripts.UI;
using _Game.Scripts.View;
using DG.Tweening;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private TutorialController _tutorialController;

        private LevelRunner _currentRunner;
        private int _lastLevelIndex;

        private Action _onAnimationCancelled;
        private bool _playingIdleAnimation;
        private bool _shown;
        private Tween _waitTween;
        private MainMenuWindow _mainMenuWindow;

        private void Start() {
            DataStorage.Instance.Init();
            ArtStorage.Instance.Init();

            _mainMenuWindow = UIController.Instance.ShowMainMenuWindow(WaitToStartLevel);
            _mainMenuWindow.OnShown.Subscribe(PlayIdleAnimation);

            SoundController.Instance.PlayMusic(SoundController.Instance.SoundTrackClip, 0f).DOFade(0.2f, 2f);

#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
            SaveManager.SetInt(SaveManager.IntData.CompletedLevels, 10);
#endif
        }

        private void StartLevel(int levelIndex) {
            _lastLevelIndex = levelIndex;
            _currentRunner = new LevelRunner();
            _currentRunner.StartLevel(DataStorage.Instance.Levels[levelIndex], OnWin,
                size => _cameraController.FocusOnBoard(size), _tutorialController, () => TryStartTutorial(levelIndex));
        }

        private void TryStartTutorial(int levelIndex) {
            if (levelIndex == 0 && !SaveManager.GetBool(SaveManager.BoolData.Tutorial1Complete)) {
                _tutorialController.StartLevel1Tutorial(() => SaveManager.SetBool(SaveManager.BoolData.Tutorial1Complete, true));
            } else if (levelIndex == 1 && !SaveManager.GetBool(SaveManager.BoolData.Tutorial2Complete)) {
                _tutorialController.StartLevel2Tutorial(() => SaveManager.SetBool(SaveManager.BoolData.Tutorial2Complete, true));
            }
        }

        private void EndLevel() {
            // NO-OP
        }

        private void OnWin() {
            SoundController.Instance.PlaySound(SoundController.Instance.LevelWinClip);
            _currentRunner = null;
            Debug.LogWarning("WIIIIIN!");

            var completedLevels = SaveManager.GetInt(SaveManager.IntData.CompletedLevels);
            var newCompletedLevels = _lastLevelIndex + 1;
            if (newCompletedLevels > completedLevels) {
                SaveManager.SetInt(SaveManager.IntData.CompletedLevels, newCompletedLevels);
            }

            var nextLevelIndex = _lastLevelIndex + 1 >= DataStorage.Instance.Levels.Length
                ? (int?) null
                : _lastLevelIndex + 1;
            UIController.Instance.ShowLevelWinPanel(nextLevelIndex, EndLevel, WaitToStartLevel);
        }

        private void Update() {
            _currentRunner?.ProcessFrame(Time.deltaTime);
        }
        
        // --------------------------- Animations --------------------------- //

        private void WaitToStartLevel(int levelIndex) {
            if (_playingIdleAnimation) {
                _onAnimationCancelled = OnAnimationEnded;
                _waitTween?.Complete(true);
            } else {
                OnAnimationEnded();
            }

            void OnAnimationEnded() {
                _onAnimationCancelled = null;

                _playingIdleAnimation = false;
                if (_shown) {
                    FieldView.Instance.Hide(() => {
                        _shown = false;
                        StartLevel(levelIndex);
                    });
                } else {
                    StartLevel(levelIndex);
                }
            }
        }

        private void PlayIdleAnimation() {
            _playingIdleAnimation = true;

            const int maxSize = 4;
            var rng = new Rng(Rng.RandomSeed);
            var size = new Vector2Int(rng.NextInt(1, maxSize), rng.NextInt(1, maxSize));
            var flowerCount = rng.NextInt(0, Mathf.FloorToInt(Mathf.Sqrt(size.x * size.y)));

            var viablePlants = DataStorage.Instance.Plants.Where(plant => plant.sprite != "yggdrasil").ToArray();

            // var offset = Vector2.right * 0.18f;
            _cameraController.FocusOnBoard(size, _mainMenuWindow.EmptySpaceOffset);
            
            

            var allPositions = size.Iterate().ToList();
            var flowerPositions = Enumerable.Range(0, flowerCount).Select(_ => {
                var position = rng.NextChoice(allPositions, out var idx);
                allPositions.RemoveAt(idx);
                return position;
            }).ToArray();

            var flowerIndex = 0;

            var resources = ResourceHelper.CreateFake(size);
            var field = new Field(resources, true);
            FieldView.Instance.Load(field, false);
            FieldView.Instance.Show(() => {
                _shown = true;
                if (_onAnimationCancelled != null) {
                    _onAnimationCancelled();
                    return;
                }

                Pause(PlantFlower);
            });

            void PlantFlower() {
                if (flowerIndex >= flowerPositions.Length) {
                    FieldView.Instance.Hide(() => {
                        _shown = false;

                        if (_onAnimationCancelled != null) {
                            _onAnimationCancelled();
                            return;
                        }
                        Pause(PlayIdleAnimation);
                    });
                    return;
                }

                var position = flowerPositions[flowerIndex];
                var view = FieldView.Instance.At(position);
                var plant = new Plant(rng.NextChoice(viablePlants));
                view.PlantAt(plant, out var process);

                process.Run(() =>
                {
                    if (_onAnimationCancelled != null) {
                        _onAnimationCancelled();
                        return;
                    }

                    flowerIndex++;
                    Pause(PlantFlower);
                });
            }

            void Pause(Action onDone) {
                const float pause = 0.8f;
                _waitTween = DOVirtual.DelayedCall(pause, () => {
                    _waitTween = null;

                    if (_onAnimationCancelled != null) {
                        _onAnimationCancelled();
                    } else {
                        onDone?.Invoke();
                    }
                });
            }
        }
    }
}