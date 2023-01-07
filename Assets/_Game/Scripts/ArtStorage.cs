using System.IO;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Data {
    public class ArtStorage : SingletonBehaviour<ArtStorage> {
        public Sprite GetSprite(string spriteName) {
            return Resources.Load<Sprite>(spriteName);
        }
    }
}