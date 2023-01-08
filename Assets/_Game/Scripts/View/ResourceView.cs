using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.View {
    public class ResourceView : MonoBehaviour {
        [SerializeField] private SpriteRenderer _resource;
        [SerializeField] private SpriteRenderer _cross;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Sprite _addSprite;

        public void Load(Color color, Color borderColor) {
            _resource.color = color;
            _cross.color = borderColor;
            SetState(State.Disabled);
        }

        public void SetState(State state) {
            _resource.enabled = state != State.Disabled;
            _resource.color = _resource.color.WithAlpha(state == State.Removed ? 0.7f : 1f);

            _resource.sprite = state == State.Added ? _addSprite : _sprite;
            _cross.enabled = state == State.Removed;
        }
        
        public enum State {
            Disabled,
            Enabled,
            Added,
            Removed
        }
    }
}