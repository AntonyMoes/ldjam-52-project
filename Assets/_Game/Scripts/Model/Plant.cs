using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using GeneralUtils;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Model {
    public class Plant {
        private readonly PlantData _data;
        public Dictionary<Resource, int> Requirements => _data.Requirements;
        public Dictionary<Resource, int> Effect => _data.Effect;
        public Vector2Int[] Range => _data.Range;
        public string Name => _data.displayName;
        public Sprite Sprite => ArtStorage.Instance.GetSprite(_data.sprite);

        public Plant(PlantData data) {
            _data = data;
        }

        public (Vector2Int offset, IDictionary<Resource, int> update)[] GetUpdates(Func<Vector2Int, IDictionary<Resource, int>> getAtOffset) {
            // only basic updates as of right now
            var updates = _data.Range
                .Select(offset => (offset, update: getAtOffset(offset)?.CombineWith(_data.Effect, clamp: true)))
                .Where(update => update.update != null)
                .ToArray();

            var selfDelta = Requirements;
            var selfOffset = Vector2Int.zero;
            var selfUpdateIdx = updates.IndexOf(update => update.offset == selfOffset);
            if (selfUpdateIdx == -1) {
                var selfUpdate = getAtOffset(selfOffset).CombineWith(selfDelta, add: false, clamp: true);
                return updates.Append((selfOffset, selfUpdate)).ToArray();
            }

            updates[selfUpdateIdx].update = updates[selfUpdateIdx].update.CombineWith(selfDelta, add: false, clamp: true);
            return updates;
        }
    }
}