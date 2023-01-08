using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using UnityEngine;

namespace _Game.Scripts.UI.PlantInfo {
    public class PlantInfoResourceGroup : MonoBehaviour {
        [SerializeField] private ResourceGroup[] _groups;
        [SerializeField] private PlantInfoResourceItem _itemPrefab;

        private readonly List<PlantInfoResourceItem> _items = new List<PlantInfoResourceItem>();

        public void Load(IDictionary<Resource, int> resources, bool effect) {
            foreach (var group in _groups) {
                LoadGroup(group.resource, group.parent, resources[group.resource], effect);
            }
        }

        private void LoadGroup(Resource resource, Transform parent, int count, bool effect) {
            var absCount = Mathf.Abs(count);
            var sign = Math.Sign(count);
            var color = ArtStorage.Instance.ResourceColors[resource];
            
            for (var i = 0; i < absCount; i++) {
                var state = !effect
                    ? PlantInfoResourceItem.State.Enabled
                    : sign == 1
                        ? PlantInfoResourceItem.State.Added
                        : PlantInfoResourceItem.State.Removed;

                var item = Instantiate(_itemPrefab, parent);
                item.Load(color, state);
                _items.Add(item);
            }
        }

        public void Clear() {
            foreach (var item in _items) {
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        [Serializable]
        private struct ResourceGroup {
            public Resource resource;
            public Transform parent;
        }
    }
}