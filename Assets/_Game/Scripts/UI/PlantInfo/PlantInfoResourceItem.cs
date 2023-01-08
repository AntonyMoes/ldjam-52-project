using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.PlantInfo {
    public class PlantInfoResourceItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Sprite _addSprite;
        [SerializeField] private Sprite _removeSprite;
        
        
        public void Load(Color color, State state) {
            _image.color = color;
            _image.enabled = state != State.Disabled;

            _image.sprite = state switch {
                State.Disabled => null,
                State.Enabled => _sprite,
                State.Added => _addSprite,
                State.Removed => _removeSprite,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        public enum State {
            Disabled,
            Enabled,
            Added,
            Removed
        }
    }
}