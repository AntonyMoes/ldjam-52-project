using System;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using _Game.Scripts.UI;
using _Game.Scripts.View;

namespace _Game.Scripts {
    public class LevelRunner {
        private Field _field;

        // private Action _onLevelComplete;

        public void StartLevel(LevelData data, Action onComplete) {
            _field = new Field(data.Field);
            FieldView.Instance.Load(_field);

            var targetPlant = GetPlantByName(data.targetPlant);
            var plants = data.availablePlants.ToDictionary(plant => GetPlantByName(plant.name), plant => plant.count);
            UIController.Instance.ShowPlantsPanel(plants, targetPlant, onComplete);

            // _onLevelComplete = onComplete;
            // TODO

            Plant GetPlantByName(string name) => new Plant(DataStorage.Instance.Plants.WithName(name));
        }
    }
}