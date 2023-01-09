using _Game.Scripts.UI;
using _Game.Scripts.View;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private TutorialController _tutorialController;

        private LevelRunner _currentRunner;
        private int _lastLevelIndex;

        private void Start() {
            DataStorage.Instance.Init();
            ArtStorage.Instance.Init();

            UIController.Instance.ShowMainMenuWindow(StartLevel);

#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
            SaveManager.SetInt(SaveManager.IntData.CompletedLevels, 300);
#endif
        }

        private void StartLevel(int levelIndex) {
            _lastLevelIndex = levelIndex;
            _currentRunner = new LevelRunner();
            _currentRunner.StartLevel(DataStorage.Instance.Levels[levelIndex], OnWin,
                _cameraController.FocusOnBoard, _tutorialController, () => TryStartTutorial(levelIndex));
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
            UIController.Instance.ShowLevelWinPanel(nextLevelIndex, EndLevel, StartLevel);
        }

        private void Update() {
            _currentRunner?.ProcessFrame(Time.deltaTime);
        }
    }
}