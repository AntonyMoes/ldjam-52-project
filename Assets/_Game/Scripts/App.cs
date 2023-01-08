using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
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
            _currentRunner.StartLevel(DataStorage.Instance.Levels[levelIndex], OnWin);
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