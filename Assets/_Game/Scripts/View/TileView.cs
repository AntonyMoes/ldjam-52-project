using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.View {
    public class TileView : MonoBehaviour {
        [SerializeField] private ResourceGroup[] _resourceGroups;
        [SerializeField] private SpriteRenderer _frame;
        [SerializeField] private SpriteRenderer _plant;
        [SerializeField] private SpriteRenderer _tempTile;
        
        public Vector2Int Position { get; private set; }
        public IDictionary<Resource, int> Resources => _field.GetAt(Position).Resources;

        private Field _field;

        public void Load(Field field, Vector2Int position, Sprite tileSprite) {
            _field = field;
            Position = position;

            var tile = _field.GetAt(Position);
            tile.OnTileUpdate.Subscribe(OnTileUpdate);

            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Load();
                resourceGroup.UpdateResources(tile.Resources);
            }

            HideResources();

            _tempTile.sprite = tileSprite;
            _tempTile.sortingOrder = -Position.x - Position.y;
            ToggleTempTile(false);
        }

        public void ToggleTempTile(bool enabled) {
            _tempTile.enabled = enabled;
        }

        public bool CanPlant(Plant plant) {
            return _field.CanPlantAt(plant, Position);
        }

        public bool PlantAt(Plant plant) {
            if (!_field.PlantAt(plant, Position)) {
                return false;
            }

            _plant.enabled = true;
            _plant.sprite = plant.Sprite;
            _plant.color = _plant.color.WithAlpha(1f);

            HideResources();

            return true;
        }

        public void PreviewPlant(Plant plant) {
            _plant.enabled = true;
            _plant.sprite = plant.Sprite;
            _plant.color = _plant.color.WithAlpha(0.5f);
        }

        public void RemovePreview() {
            _plant.enabled = false;
        }

        public void ShowResources(IDictionary<Resource,int> update = null) {
            if (_field.GetAt(Position).HasPlant) {
                return;
            }

            _frame.enabled = true;
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Show(update);
            }
        }

        public void HideResources() {
            _frame.enabled = false;
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