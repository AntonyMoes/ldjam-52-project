using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI {
    public class RangeItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _affectedSprite;
        [SerializeField] private Sprite _plantSprite;

        public void Load(State state) {
            _image.enabled = state != State.Empty;
            _image.sprite = state == State.Plant ? _plantSprite : _affectedSprite;
        }

        public enum State {
            Empty,
            Affected,
            Plant
        }
    }
}