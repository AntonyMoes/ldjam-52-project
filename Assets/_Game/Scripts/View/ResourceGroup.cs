using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.View {
    public class ResourceGroup : MonoBehaviour {
        [SerializeField] private Resource _resource;
        // [SerializeField] private Image[] _images;
        [SerializeField] private ResourceView[] _resourceViews;

        private IDictionary<Resource, int> _currentResources;
        
        public void Load() {
            var color = ArtStorage.Instance.ResourceColors[_resource];
            var borderColor = ArtStorage.Instance.BorderColors[_resource];

            foreach (var resourceView in _resourceViews) {
                resourceView.Load(color, borderColor);
            }
        }

        public void UpdateResources(IDictionary<Resource, int> resources) {
            _currentResources = resources;
        }

        public void Show(IDictionary<Resource, int> update = null) {
            gameObject.SetActive(true);
            var updatedValue = (update ?? _currentResources)?[_resource] ?? 0;
            var currentValue = _currentResources?[_resource] ?? 0;

            for (var i = 0; i < _resourceViews.Length; i++) {
                var state = (i < currentValue, i < updatedValue) switch {
                    (true, true) => ResourceView.State.Enabled,
                    (true, false) => ResourceView.State.Removed,
                    (false, true) => ResourceView.State.Added,
                    (false, false) => ResourceView.State.Disabled
                };

                _resourceViews[i].SetState(state);
            }
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}