using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using GeneralUtils;

namespace _Game.Scripts.Model {
    public class FieldTile {
        private Dictionary<Resource, int> _resources;
        public Dictionary<Resource, int> Resources {
            get => _resources;
            set {
                _resources = value;
                _onTileUpdate();
            }
        }

        private bool _hasPlant;
        public bool HasPlant {
            get => _hasPlant;
            set {
                _hasPlant = value;
                _onTileUpdate();
            }
        }

        private readonly Action _onTileUpdate;
        public readonly Event OnTileUpdate;

        public FieldTile(Dictionary<Resource, int> resources) {
            _resources = resources;
            OnTileUpdate = new Event(out _onTileUpdate);
        }
    }
}