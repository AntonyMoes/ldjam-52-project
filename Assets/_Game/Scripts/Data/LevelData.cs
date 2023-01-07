using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Scripts.Data {
    [Serializable]
    public class LevelData : IData {
        [JsonProperty] private string field;
        public Dictionary<Resource, int>[][] Field { get; private set; }
        public string targetPlant;
        public AvailablePlant[] availablePlants;
        public Dictionary<string, int> AvailablePlants { get; private set; }

        [Serializable]
        public struct AvailablePlant {
            public string name;
            public int count;
        }

        public void Load() {
            Field = FieldFromCsv(field);
            AvailablePlants = availablePlants.ToDictionary(plant => plant.name, plant => plant.count);
        }

        private static Dictionary<Resource, int>[][] FieldFromCsv(string csv) {
            return csv
                .Replace("\\n", "\n")
                .Replace(" ", "")
                .Split('\n')
                .Reverse()
                .Select(row => row
                    .Split(',')
                    .Select(resourceData => resourceData.ToResources())
                    .ToArray())
                .ToArray();
        }
    }
}