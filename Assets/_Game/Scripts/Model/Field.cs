using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using UnityEngine;

namespace _Game.Scripts.Model {
    public class Field {
        public readonly bool Fake;
        private readonly FieldTile[][] _field;

        public Vector2Int Size => new Vector2Int(_field.Length, _field[0].Length);

        public Field(Dictionary<Resource, int>[][] fieldData, bool fake = false) {
            Fake = fake;
            _field = fieldData
                .Select(row => row
                    .Select(resourceData => new FieldTile(resourceData))
                    .ToArray())
                .ToArray();
        }

        public bool CanPlantAt(Plant plant, Vector2Int position) {
            return GetAt(position) is { HasPlant: false } tile 
                   && (Fake
                       || !tile.HasPlant
                       && tile.Resources.CombineWith(plant.Requirements, false).Values.All(count => count >= 0));
        }

        public bool PlantAt(Plant plant, Vector2Int position) {
            if (!CanPlantAt(plant, position)) {
                return false;
            }

            GetAt(position).HasPlant = true;
            var updates = GetUpdates(plant, position);
            foreach (var (offset, update) in updates) {
                if (GetAt(offset + position) is { } tile) {
                    tile.Resources = update;
                }
            }

            return true;
        }

        public (Vector2Int offset, IDictionary<Resource, int> update)[] GetUpdates(Plant plant, Vector2Int position) {
            return plant.GetUpdates(offset => GetAt(offset + position)?.Resources);
        }

        public FieldTile GetAt(Vector2Int position) {
            if (position.x < 0 || position.x >= _field.Length) {
                return null;
            }

            if (position.y < 0 || position.y >= _field[0].Length) {
                return null;
            }

            return _field[position.x][position.y];
        }
    }
}