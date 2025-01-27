﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Model;
using DG.Tweening;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Game.Scripts.View {
    public class FieldView : SingletonBehaviour<FieldView> {
        [SerializeField] private Vector3Int _initialPosition;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        [SerializeField] private Tile[] _tiles;

        [SerializeField] private Tilemap _availableMap;
        [SerializeField] private Tilemap _affectedMap;
        [SerializeField] private Tile _availableTile;
        [SerializeField] private Tile _affectedTile;
        [SerializeField] private Tile _currentTile;

        [SerializeField] private Vector3 _tileViewOffset;
        [SerializeField] private TileView _tileViewPrefab;
        [SerializeField] private Transform _tileViewsParent;
        private readonly List<TileView> _tileViews = new List<TileView>();

        private Field _field;
        private bool _showResources;
        private Tween _tween; 

        private Vector3Int FieldToMapPosition(Vector2Int position) {
            return _initialPosition + new Vector3Int(position.y, position.x, 0);
        }

        public void Load(Field field, bool showResources = true) {
            Clear();

            _field = field;
            _showResources = false;

            var rng = new Rng(Rng.RandomSeed);

            foreach (var position in _field.Size.Iterate()) {
                // here wee can basically modify axis directions
                var tilePosition = FieldToMapPosition(position);
                var tile = rng.NextChoice(_tiles);
                _tilemap.SetTile(tilePosition, tile);

                var tileView = Instantiate(_tileViewPrefab, _tileViewsParent);
                tileView.transform.position = _tilemap.CellToWorld(tilePosition) + _tileViewOffset;
                tileView.Load(_field, position, tile.sprite, AnimatePlant);
                _tileViews.Add(tileView);
            }

            ToggleResources(showResources);
        }

        public void ToggleResources(bool? show = null) {
            var showResources = show ?? !_showResources;
            if (showResources == _showResources) {
                return;
            }

            foreach (var tileView in _tileViews) {
                if (showResources) {
                    tileView.ShowResources();
                } else {
                    tileView.HideResources();
                }
            }

            _showResources = showResources;
        }
        
        public void ShowAvailableTiles(Plant plant) {
            foreach (var position in _field.Size.Iterate()) {
                if (_field.CanPlantAt(plant, position)) {
                    _availableMap.SetTile(FieldToMapPosition(position), _availableTile);
                }
            }
        }

        public void HideAvailableTiles() {
            _availableMap.ClearAllTiles();
        }

        public void ShowAffectedTiles(Plant plant, TileView tileView) {
            var updates = _field.GetUpdates(plant, tileView.Position);
            foreach (var (offset, update) in updates) {
                var position = tileView.Position + offset;
                var view = _tileViews.First(view => view.Position == position);
                view.ShowResources(update);

                _affectedMap.SetTile(FieldToMapPosition(position), _affectedTile);
            }
        }

        public void HideAffectedTiles() {
            _affectedMap.ClearAllTiles();

            foreach (var tileView in _tileViews) {
                if (_showResources) {
                    tileView.ShowResources();
                } else {
                    tileView.HideResources();
                }
            }
        }

        public void Clear() {
            _tween?.Complete(true);

            _tilemap.ClearAllTiles();

            HideAvailableTiles();
            HideAffectedTiles();

            foreach (var tileView in _tileViews) {
                Destroy(tileView.gameObject);
            }

            _tileViews.Clear();
        }

        // ------------------------------ ANIMATIONS ------------------------------ //

        public void Show(Action onDone = null) {
            _tilemapRenderer.enabled = false;
            foreach (var tileView in _tileViews) {
                tileView.ToggleTempTile(true);
            }

            const float fallDuration = 0.5f;  // 0.5f
            const float punchDuration = 0.4f;  // 0.3f
            const float delay = 0.05f;

            const float startHeight = 16f;
            const float punchHeight = 0.4f;  // 0.5f

            var sequence = DOTween.Sequence();

            var rng = new Rng(Rng.RandomSeed);
            var shuffled = rng.NextShuffle(_tileViews);

            for (var i = 0; i < shuffled.Count; i++) {
                var tileTransform = shuffled[i].transform;
                var initialPosition = tileTransform.position;
                tileTransform.position = initialPosition + Vector3.up * startHeight;

                sequence.Insert(i * delay, tileTransform.DOMoveY(initialPosition.y, fallDuration).SetEase(Ease.OutBack, 1.2f));
                sequence.Insert(i * delay + fallDuration, tileTransform.DOPunchPosition(Vector3.up * punchHeight, punchDuration, 0, 0));
            }

            sequence.OnComplete(() => {
                foreach (var tileView in _tileViews) {
                    tileView.ToggleTempTile(false);
                }

                _tilemapRenderer.enabled = true;
                onDone?.Invoke();
            });

            _tween = sequence;
            _tween.Play();
        }

        public void Hide(Action onDone = null) {
            _tween?.Complete(true);

            _tilemap.ClearAllTiles();
            HideAvailableTiles();
            HideAffectedTiles();

            foreach (var tileView in _tileViews) {
                tileView.ToggleTempTile(true);
            }

            const float moveDuration = 0.35f;
            const float delay = 0.05f;

            const float endHeight = 16f;

            var sequence = DOTween.Sequence();

            var rng = new Rng(Rng.RandomSeed);
            var shuffled = rng.NextShuffle(_tileViews);

            for (var i = 0; i < shuffled.Count; i++) {
                var tileTransform = shuffled[i].transform;
                var initialPosition = tileTransform.position;
                sequence.Insert(i * delay, tileTransform.DOMoveY(initialPosition.y + endHeight, moveDuration).SetEase(Ease.InSine));
            }

            sequence.OnComplete(() => {
                foreach (var tileView in _tileViews) {
                    tileView.ToggleTempTile(false);
                }

                onDone?.Invoke();
            });

            _tween = sequence;
            _tween.Play();
        }
        
        private Process AnimatePlant(Vector2Int origin, Vector2Int[] range) {
            _tilemapRenderer.enabled = false;
            foreach (var tileView in _tileViews) {
                tileView.ToggleTempTile(true);
            }

            const float punchDuration = 0.3f;
            const float delay = 0.1f;

            const float punchHeight = -0.25f;

            var sequence = DOTween.Sequence();
            var fullRange = range.Append(Vector2Int.zero).ToHashSet();

            // AudioClip clip;
            // var magnitudesCount = fullRange.Select(offset => offset.magnitude).ToHashSet().Count;
            // if (fullRange.Count >= 5 && magnitudesCount >= 4) {
            //     clip = SoundController.Instance.PlantPlace3Clip;
            // } else if (fullRange.Count >= 3 && magnitudesCount >= 3) {
            //     clip = SoundController.Instance.PlantPlace2Clip;
            // } else if (fullRange.Count >= 2) {
            //     clip = SoundController.Instance.PlantPlace1Clip;
            // } else {
            //     clip = null;
            // }

            // if (clip != null) {
            //     SoundController.Instance.PlaySound(clip);
            // }

            var delays = new HashSet<float>();
            foreach (var tileView in _tileViews.Where(view => fullRange.Contains(view.Position - origin))) {
                var diffVector = (tileView.Position - origin);
                var multiplier = Mathf.Abs(diffVector.x) + Mathf.Abs(diffVector.y);

                sequence.InsertCallback(multiplier * delay, () => tileView.OnTileUpdate());
                sequence.Insert(multiplier * delay, tileView.transform.DOPunchPosition(Vector3.up * punchHeight, punchDuration, 0, 0));
                delays.Add(multiplier * delay);
            }

            if (!_field.Fake) {
                foreach (var concreteDelay in delays) {
                    sequence.InsertCallback(concreteDelay, () => {
                        var pitch = 1f + (concreteDelay / delay - 1) * 0.05f;
                        SoundController.Instance.PlaySound(SoundController.Instance.PlantPlace1Clip, volume: 0.5f, pitch: pitch);
                    });
                }
            }

            sequence.AppendCallback(() => {
                foreach (var tileView in _tileViews) {
                    tileView.ToggleTempTile(false);
                }

                _tilemapRenderer.enabled = true;
            });

            _tween = sequence;
            return new TweenProcess(_tween);
        }

        public TileView At(Vector2Int position) => _tileViews.FirstOrDefault(view => view.Position == position);
    }
}