using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts {
    public class ArtStorage : SingletonBehaviour<ArtStorage> {
        [SerializeField] private ResourceColor[] _resourceColors;
        public IDictionary<Resource, Color> ResourceColors { get; private set; }

        public void Init() {
            ResourceColors = _resourceColors.ToDictionary(rc => rc.resource, rc => rc.color);
        }

        public Sprite GetSprite(string spriteName) {
            return Resources.Load<Sprite>(spriteName);
        }

        [Serializable]
        private struct ResourceColor {
            public Resource resource;
            public Color color;
        }
    }
}