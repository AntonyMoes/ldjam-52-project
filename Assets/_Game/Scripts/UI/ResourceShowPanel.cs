using _Game.Scripts.View;
using GeneralUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class ResourceShowPanel : UIElement {
        [SerializeField] private Button _button;

        protected override void Init() {
            _button.onClick.AddListener(() => FieldView.Instance.ToggleResources());
        }
    }
}