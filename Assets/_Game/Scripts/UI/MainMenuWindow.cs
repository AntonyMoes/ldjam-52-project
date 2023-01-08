using System;
using GeneralUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class MainMenuWindow : UIElement {
        [SerializeField] private BaseButton _levelSelectButton;
        [SerializeField] private BaseButton _settingsButton;

        private Action<int> _startLevel;

        protected override void Init() {
            _levelSelectButton.OnClick.Subscribe(OnLevelSelectClick);
            _settingsButton.OnClick.Subscribe(OnSettingsClick);
        }

        public void Load(Action<int> startLevel) {
            _startLevel = startLevel;
        }

        private void OnLevelSelectClick() {
            Hide(() => UIController.Instance.ShowLevelSelectWindow(_startLevel));
        }

        private void OnSettingsClick() {
            // TODO
        }
    }
}