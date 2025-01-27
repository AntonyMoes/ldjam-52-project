﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class BaseButton : MonoBehaviour {
        [SerializeField] private Button _button;

        private readonly Action _onClick;
        public readonly GeneralUtils.Event OnClick;

        public bool Enabled {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public BaseButton() {
            OnClick = new GeneralUtils.Event(out _onClick);
        }

        private void Awake() {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick() {
            SoundController.Instance.PlaySound(SoundController.Instance.ButtonPressClip, 0.3f);
            _onClick();
        }
    }
}