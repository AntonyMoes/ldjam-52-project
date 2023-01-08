using System;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelItem : MonoBehaviour {
        [SerializeField] private BaseButton _button;
        [SerializeField] private TextMeshProUGUI _label;

        public void Load(int index, Action<int> onClick) {
            _label.text = index.ToString();
            _button.OnClick.Subscribe(() => onClick?.Invoke(index));
        }
    }
}