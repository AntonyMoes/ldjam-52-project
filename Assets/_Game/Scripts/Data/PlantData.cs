using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Scripts.Data {
    [Serializable]
    public class PlantData : IData {
        private const char PlantPositionChar = 'o';
        private const char RangeChar = 'x';

        public string name;
        public string displayName;
        public string sprite;
        [JsonProperty] private string requirements;
        public Dictionary<Resource, int> Requirements { get; private set; }
        [JsonProperty] private string range;
        public Vector2Int[] Range { get; private set; }
        [JsonProperty] private string effect;
        public Dictionary<Resource, int> Effect { get; private set; }

        public void Load() {
            Requirements = requirements.ToResources();
            Range = ParseRange(range);
            Effect = effect.ToResources(true);
        }

        private static Vector2Int[] ParseRange(string csv) {
            var rangeData = csv
                .Replace("\\n", "\n")
                .Replace(" ", "")
                .Split('\n')
                .Reverse()
                .Select((rowData, row) => rowData
                    .Split(',')
                    .Select((rangeTileData, column) => (position: new Vector2Int(row, column), data: rangeTileData)))
                .SelectMany(pair => pair)
                .ToArray();

            var plantPosition = rangeData.First(pair => pair.data.Contains(PlantPositionChar)).position;
            return rangeData
                .Where(pair => pair.data.Contains(RangeChar))
                .Select(pair => pair.position - plantPosition)
                .ToArray();
        }
    }
}