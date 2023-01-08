using System;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelWinPanel : UIElement {
        [SerializeField] private BaseButton _nextLevelButton;
        [SerializeField] private BaseButton _mainMenuButton;

        private Action _startLevel;
        private Action _endLevel;

        protected override void Init() {
            _nextLevelButton.OnClick.Subscribe(OnNextLevelClick);
            _mainMenuButton.OnClick.Subscribe(OnMainMenuClick);
        }

        public void Load(int? nextLevelIndex, Action endLevel, Action<int> startLevel) {
            _endLevel = endLevel;
            
            if (nextLevelIndex is { } index) {
                _nextLevelButton.gameObject.SetActive(true);
                _startLevel = () => startLevel?.Invoke(index);
            } else {
                _nextLevelButton.gameObject.SetActive(false);
                _startLevel = null;
            }
        }

        private void OnNextLevelClick() {
            Hide(() => _startLevel?.Invoke());
        }

        private void OnMainMenuClick() {
            _endLevel?.Invoke();
            Hide(() => UIController.Instance.ShowMainMenuWindow());
        }
    }
}