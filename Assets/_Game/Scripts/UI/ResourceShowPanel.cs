using _Game.Scripts.View;
using GeneralUtils.UI;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class ResourceShowPanel : UIElement {
        [SerializeField] private BaseButton _button;

        protected override void Init() {
            _button.OnClick.Subscribe(() => FieldView.Instance.ToggleResources());
        }
    }
}