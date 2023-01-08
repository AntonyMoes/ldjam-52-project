using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts {
    public static class Extensions {
        public static IEnumerable<Vector2Int> Iterate(this Vector2Int vector) {
            return Enumerable
                .Range(0, vector.x)
                .Select(row => Enumerable
                    .Range(0, vector.y)
                    .Select(column => new Vector2Int(row, column)))
                .SelectMany(pos => pos);
        }
    }
}