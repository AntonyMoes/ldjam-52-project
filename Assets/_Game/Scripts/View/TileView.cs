using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using UnityEngine;

namespace _Game.Scripts.View {
    public class TileView : MonoBehaviour {
        public Vector2Int Position { get; private set; }
        public IDictionary<Resource, int> Resources => _field.GetAt(Position).Resources;

        private Field _field;

        public void Load(Field field, Vector2Int position) {
            _field = field;
            Position = position;
        }

        public bool TryPlant(Plant plant) {
            return _field.PlantAt(plant, Position);
        }
    }
}