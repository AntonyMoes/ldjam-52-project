﻿using System;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.UI {
    public class LevelItem : MonoBehaviour {
        [SerializeField] private BaseButton _button;
        [SerializeField] private TextMeshProUGUI _label;

        public void Load(int index, Action<int> onClick, bool enabled) {
            _label.text = (index + 1).ToString();
            _button.Enabled = enabled;
            _button.OnClick.Subscribe(() => onClick?.Invoke(index));
        }
    }
}