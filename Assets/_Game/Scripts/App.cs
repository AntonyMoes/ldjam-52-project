using System;
using _Game.Scripts.Data;
using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        private void Start() {
            DataStorage.Instance.Init();
            // TODO
            new LevelRunner().StartLevel(DataStorage.Instance.Levels[0], OnWin);
        }

        private void OnWin() {
            Debug.LogError("WIIIIIN!");
        }
    }
}