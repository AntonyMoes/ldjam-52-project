using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Model;
using DG.Tweening;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Game.Scripts.View {
    public class TileView : MonoBehaviour {
        [SerializeField] private ResourceGroup[] _resourceGroups;
        [SerializeField] private SpriteRenderer _frame;
        [SerializeField] private SpriteRenderer _plant;
        [SerializeField] private SpriteRenderer _tempTile;
        [SerializeField] private SortingGroup _group;

        [Header("Main plant")]
        [SerializeField] private Sprite _initialMainSprite;
        [SerializeField] private Sprite[] _mainSprites;
        [SerializeField] private SpriteRenderer _layerPrefab;
        [SerializeField] private Transform _layerParent;
        [SerializeField] private float _layerOffset;

        public Vector2Int Position { get; private set; }
        public IDictionary<Resource, int> Resources => _field.GetAt(Position).Resources;

        private Field _field;
        private Func<Vector2Int, Vector2Int[], Process> _animatePlant;

        public void Load(Field field, Vector2Int position, Sprite tileSprite, Func<Vector2Int, Vector2Int[], Process> animatePlant) {
            _field = field;
            Position = position;

            _animatePlant = animatePlant;

            var tile = _field.GetAt(Position);
            // tile.OnTileUpdate.Subscribe(OnTileUpdate);

            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Load();
                resourceGroup.UpdateResources(tile.Resources);
            }

            HideResources();

            _tempTile.sprite = tileSprite;
            _group.sortingOrder = -Position.x - Position.y;
            ToggleTempTile(false);
        }

        public void ToggleTempTile(bool enabled) {
            _tempTile.enabled = enabled;
        }

        public bool CanPlant(Plant plant) {
            return _field.CanPlantAt(plant, Position);
        }

        public bool PlantAt(Plant plant, out Process plantProcess, bool mainPlant = false) {
            if (!_field.PlantAt(plant, Position)) {
                plantProcess = new DummyProcess();
                return false;
            }

            _plant.enabled = true;
            _plant.sprite = !mainPlant ? plant.Sprite : _initialMainSprite;
            _plant.color = _plant.color.WithAlpha(1f);

            if (mainPlant) {
                SoundController.Instance.PlaySound(SoundController.Instance.TreePlaceClip, 0.6f);
            }

            HideResources();

            var process = _animatePlant?.Invoke(Position, plant.Range);

            if (!mainPlant) {
                plantProcess = process;
                return true;
            }

            var fullProcess = new SerialProcess();
            fullProcess.Add(process);
            fullProcess.Add(AnimatePlantMain());
            plantProcess = fullProcess;
            return true;
        }

        private Process AnimatePlantMain() {
            const float duration = 0.1f;
            const float delay = 0.05f;
            const float maxColorOffset = 0.1f;

            var count = 30;  // TODO

            var initialOffset = _layerOffset * count;
            var rng = new Rng(Rng.RandomSeed);

            var clip = SoundController.Instance.TreeFallClip;
            AudioSource fallSource = null;

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => fallSource = SoundController.Instance.PlaySound(clip, 0.6f));

            for (var i = 0; i < count; i++) {
                var sprite = rng.NextChoice(_mainSprites);
                var layer = Instantiate(_layerPrefab, _layerParent);
                layer.sprite = sprite;
                layer.sortingOrder = i + 1;
                layer.color -= new Color(1f, 1f, 1f, 0f) * rng.NextFloat(0, maxColorOffset);

                var initialPosition = layer.transform.localPosition;
                initialPosition.y = initialOffset;
                layer.transform.localPosition = initialPosition;

                sequence.Insert(i * delay, layer.transform.DOLocalMoveY(_layerOffset * i, duration));
            }

            sequence.AppendCallback(() => {
                if (fallSource.clip == clip) {
                    fallSource.Stop();
                }
            });

            return new TweenProcess(sequence);
        }

        public void PreviewPlant(Plant plant, bool mainPlant = false) {
            _plant.enabled = true;
            _plant.sprite = !mainPlant ? plant.Sprite : _initialMainSprite;
            _plant.color = _plant.color.WithAlpha(0.5f);
        }

        public void RemovePreview() {
            _plant.enabled = false;
        }

        public void ShowResources(IDictionary<Resource,int> update = null) {
            if (_field.GetAt(Position).HasPlant) {
                return;
            }

            _frame.enabled = true;
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Show(update);
            }
        }

        public void HideResources() {
            _frame.enabled = false;
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.Hide();
            }
        }

        public void OnTileUpdate() {
            var tile = _field.GetAt(Position);
            foreach (var resourceGroup in _resourceGroups) {
                resourceGroup.UpdateResources(tile.Resources);
                if (_frame.enabled) {
                    resourceGroup.Show();
                }
            }
        }

        private void OnDestroy() {
            // _field.GetAt(Position).OnTileUpdate.Unsubscribe(OnTileUpdate);
        }
    }
}