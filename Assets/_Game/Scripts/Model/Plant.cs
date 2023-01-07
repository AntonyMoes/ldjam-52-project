using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.Data;
using UnityEngine;

namespace _Game.Scripts.Model {
    public class Plant {
        private readonly PlantData _data;
        public Dictionary<Resource, int> Requirements => _data.Requirements;
        public Sprite Sprite => ArtStorage.Instance.GetSprite(_data.sprite);

        public Plant(PlantData data) {
            _data = data;
        }

        public (Vector2Int offset, Dictionary<Resource, int> update)[] GetUpdates(Func<Vector2Int, IDictionary<Resource, int>> getAtOffset) {
            // only basic updates as of right now
            return _data.Range
                .Select(offset => (offset, update: getAtOffset(offset)?.CombineWith(_data.Effect)))
                .Where(update => update.update != null)
                .ToArray();
        }
    }
}