using _Game.Scripts.UI;
using _Game.Scripts.View;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        [SerializeField] private CameraController _cameraController;

        private LevelRunner _currentRunner;
        private int _lastLevelIndex;

        private void Start() {
            DataStorage.Instance.Init();
            ArtStorage.Instance.Init();

            UIController.Instance.ShowMainMenuWindow(StartLevel);
        }

        private void StartLevel(int levelIndex) {
            _lastLevelIndex = levelIndex;
            _currentRunner = new LevelRunner();
            _currentRunner.StartLevel(DataStorage.Instance.Levels[levelIndex], OnWin, _cameraController.FocusOnBoard);
        }

        private void EndLevel() {
            // NO-OP
        }

        private void OnWin() {
            _currentRunner = null;
            // TODO
            Debug.LogError("WIIIIIN!");

            var nextLevelIndex = _lastLevelIndex + 1 >= DataStorage.Instance.Levels.Length
                ? (int?) null
                : _lastLevelIndex + 1;
            UIController.Instance.ShowLevelWinPanel(nextLevelIndex, EndLevel, StartLevel);
        }

        private void Update() {
            _currentRunner?.ProcessFrame(Time.deltaTime);
        }
    }
}