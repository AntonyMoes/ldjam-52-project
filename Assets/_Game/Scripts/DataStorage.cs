using System;
using System.Linq;
using _Game.Scripts.Data;
using GeneralUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Scripts {
    public class DataStorage : SingletonBehaviour<DataStorage> {
        [SerializeField] private TextAsset _plants;
        [SerializeField] private TextAsset _levels;

        public PlantData[] Plants { get; private set; }
        public LevelData[] Levels { get; private set; }

        public void Init() {
            Plants = LoadRecords<PlantData>(_plants);
            Levels = LoadRecords<LevelData>(_levels);
        }

        private static T[] LoadRecords<T>(TextAsset asset) where T : IData {
            var records = JsonConvert.DeserializeObject<Records<T>>(asset.text)!.records;
            foreach (var record in records) {
                record.Load();
            }

            return records;
        }

        [Serializable]
        private class Records<T> {
            public T[] records;
        }
    }

    public static class DataStorageHelper {
        public static PlantData WithName(this PlantData[] plants, string name) {
            return plants.FirstOrDefault(data => data.name == name);
        }
    }
}