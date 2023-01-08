using System;
using _Game.Scripts.Data;
using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        private LevelRunner _currentRunner;

        private void Start() {
            DataStorage.Instance.Init();
            ArtStorage.Instance.Init();

            UIController.Instance.ShowMainMenu(StartLevel);
        }

        private void StartLevel(int levelIndex) {
            _currentRunner = new LevelRunner();
            _currentRunner.StartLevel(DataStorage.Instance.Levels[levelIndex], OnWin);
        }

        private void OnWin() {
            _currentRunner = null;
            // TODO
            Debug.LogError("WIIIIIN!");
        }

        private void Update() {
            _currentRunner?.ProcessFrame(Time.deltaTime);
        }
    }
}