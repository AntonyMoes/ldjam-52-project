using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data {
    public enum Resource {
        R,
        Y,
        B,
    }

    public static class ResourceHelper {
        private const string ResourceMatcher = "-?\\d";
        private const string NotNegativeResourceMatcher = "\\d";

        private const int MinResourceValue = 0;
        private const int MaxResourceValue = 5;

        private static readonly Resource[] Resources = (Resource[]) Enum.GetValues(typeof(Resource));

        public static Dictionary<Resource, int> ToResources(this string resourceData, bool allowNegative = false) {
            var groupString = allowNegative ? ResourceMatcher : NotNegativeResourceMatcher;
            var groupsString = string.Join("", Enumerable.Range(0, Resources.Length).Select(_ => $"({groupString})"));
            var matcher = new Regex($"^{groupsString}$");
            var match = matcher.Match(resourceData);

            if (!match.Success) {
                throw new ArgumentException(
                    $"Could not parse resource data \"{resourceData}\", allowNegative = {allowNegative}");
            }

            return Enumerable
                .Range(0, Resources.Length)
                .ToDictionary(i => Resources[i],
                    i => Mathf.Clamp(int.Parse(match.Groups[i + 1].Value), MinResourceValue, MaxResourceValue));
        }

        public static Dictionary<Resource, int> CombineWith(this IDictionary<Resource, int> resources,
            IDictionary<Resource, int> otherResources, bool add = true, bool clamp = false) {
            var multiplier = add ? 1 : -1;

            var result = new Dictionary<Resource, int>();
            foreach (var resource in resources.Keys) {
                var resultValue = resources[resource] + multiplier * otherResources[resource];
                result[resource] = clamp ? Mathf.Clamp(resultValue, MinResourceValue, MaxResourceValue) : resultValue;
            }

            return result;
        }

        public static string Serialize(this IDictionary<Resource, int> resources) {
            return string.Join(";", resources.Select(pair => $"{pair.Key}:{pair.Value}"));
        }
    }
}