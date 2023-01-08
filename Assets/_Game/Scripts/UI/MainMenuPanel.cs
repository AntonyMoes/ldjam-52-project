using System;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class MainMenuPanel : UIElement {
        [SerializeField] private BaseButton _button;

        private Action _endLevel;

        protected override void Init() {
            _button.OnClick.Subscribe(OnButtonClick);
        }

        public void Load(Action endLevel) {
            _endLevel = endLevel;
        }

        private void OnButtonClick() {
            _endLevel?.Invoke();
        }
    }
}