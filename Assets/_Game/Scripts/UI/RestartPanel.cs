using System;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class RestartPanel : UIElement {
        [SerializeField] private BaseButton _button;

        private Action _restart;

        protected override void Init() {
            _button.OnClick.Subscribe(OnButtonClick);
        }

        public void Load(Action restart) {
            _restart = restart;
        }

        private void OnButtonClick() {
            _restart?.Invoke();
        }

        private void Update() {
            if (State == EState.Shown && Input.GetKeyDown(KeyCode.R) && _button.Enabled) {
                OnButtonClick();
            }
        }

        public void SetButtons(bool enabled) {
            _button.Enabled = enabled;
        }
    }
}