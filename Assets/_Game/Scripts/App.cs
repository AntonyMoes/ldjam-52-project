using System;
using _Game.Scripts.Data;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        private LevelRunner _currentRunner;

        private void Start() {
            DataStorage.Instance.Init();
            ArtStorage.Instance.Init();

            // TODO
            _currentRunner = new LevelRunner();
            _currentRunner.StartLevel(DataStorage.Instance.Levels[0], OnWin);
        }

        private void OnWin() {
            // TODO
            Debug.LogError("WIIIIIN!");
        }

        private void Update() {
            _currentRunner?.ProcessFrame(Time.deltaTime);
        }
    }
}