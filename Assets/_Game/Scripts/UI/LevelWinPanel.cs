using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelWinPanel : UIElement {
        [SerializeField] private BaseButton _nextLevelButton;
        [SerializeField] private BaseButton _mainMenuButton;

        protected override void Init() {
            _nextLevelButton.OnClick.Subscribe(OnNextLevelClick);
            _mainMenuButton.OnClick.Subscribe(OnMainMenuClick);
        }

        private void OnNextLevelClick() {
            
        }

        private void OnMainMenuClick() {
            
        }
    }
}