using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using UnityEngine;

namespace _Game.Scripts.View {
    public class TileView : MonoBehaviour {
        [SerializeField] private ResourceGroup[] _resourceGroups;
        
        public Vector2Int Position { get; private set; }
        public IDictionary<Resource, int> Resources => _field.GetAt(Position).Resources;

        private Field _field;

        public void Load(Field field, Vector2Int position) {
            _field = field;
            Position = position;

            var tile = _field.GetAt(Position);
            tile.OnTileUpdate.Subscribe(OnTileUpdate);

            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Load();
                resourceGroup.UpdateResources(tile.Resources);
            }
        }

        public bool CanPlant(Plant plant) {
            return _field.CanPlantAt(plant, Position);
        }

        public bool PlantAt(Plant plant) {
            return _field.PlantAt(plant, Position);
        }

        public void ShowResources(IDictionary<Resource,int> update = null) {
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Show(update);
            }
        }

        public void HideResources() {
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Hide();
            }
        }

        private void OnTileUpdate() {
            var tile = _field.GetAt(Position);
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.UpdateResources(tile.Resources);
            }
        }

        private void OnDestroy() {
            _field.GetAt(Position).OnTileUpdate.Unsubscribe(OnTileUpdate);
        }
    }
}