using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.View {
    public class ResourceView : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _addGroup;
        [SerializeField] private GameObject _removeGroup;
        
        public void Load(Color color) {
            _image.color = color;
            SetState(State.Disabled);
        }

        public void SetState(State state) {
            _image.enabled = state == State.Enabled || state == State.Removed;
            _addGroup.SetActive(state == State.Added);
            _removeGroup.SetActive(state == State.Removed);
        }
        
        public enum State {
            Disabled,
            Enabled,
            Added,
            Removed
        }
    }
}