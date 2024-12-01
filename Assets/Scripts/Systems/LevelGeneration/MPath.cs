using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class MPath : IEnumerable
    {
        private int _length;
        private readonly List<MPoint> _path = new();
        private int _currentPoint;

        public static MPath FindPath(
            MPoint from,
            MPoint to,
            int[] mapData2D,
            MPoint mapData2Dimensions,
            int[] obstaclesValues,
            bool allowDiagonal = false)
        {
            // Create map grid and populate with obstacles
            var mapData = new int[mapData2D.Length];
            for (int i = 0; i < mapData2D.Length; i++)
            {
                mapData[i] = obstaclesValues.Contains(mapData2D[i]) ? -1 : 0;
            }

            // Ensure start and end points are not obstacles
            mapData[GetIndex(from.x, from.y, mapData2Dimensions)] = 0;
            mapData[GetIndex(to.x, to.y, mapData2Dimensions)] = 0;

            // Initialize search lists
            var openList = new List<MPoint> { from };
            var closedList = new HashSet<MPoint>();
            var cameFrom = new Dictionary<MPoint, MPoint>();

            // Define movement directions
            var directions = allowDiagonal
                ? new MPoint[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, -1), new(1, 1), new(1, -1), new(-1, 1) }
                : new MPoint[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

            while (openList.Count > 0)
            {
                var current = openList[0];
                openList.RemoveAt(0);

                if (current == to)
                {
                    return ReconstructPath(cameFrom, current);
                }

                closedList.Add(current);

                foreach (var direction in directions)
                {
                    var neighbor = current + direction;
                    if (!IsInBoundary(neighbor, mapData2Dimensions) || closedList.Contains(neighbor) || mapData[GetIndex(neighbor, mapData2Dimensions)] == -1)
                    {
                        continue;
                    }

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            Debug.Log($"Path not found! From: ({from.x},{from.y}) To: ({to.x},{to.y})");
            return null;
        }

        private static int GetIndex(int x, int y, MPoint mapData2Dimensions)
        {
            return x + y * mapData2Dimensions.x;
        }

        private static int GetIndex(MPoint p, MPoint mapData2Dimensions)
        {
            return p.x + p.y * mapData2Dimensions.x;
        }

        private static bool IsInBoundary(int x, int y, MPoint mapData2Dimensions)
        {
            return !(x < 0 || y < 0 ||
                     x >= mapData2Dimensions.x || y >= mapData2Dimensions.y);
        }

        private static bool IsInBoundary(MPoint p, MPoint mapData2Dimensions)
        {
            return IsInBoundary(p.x, p.y, mapData2Dimensions);
        }

        private static MPath ReconstructPath(Dictionary<MPoint, MPoint> cameFrom, MPoint current)
        {
            var path = new MPath { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();

            return path;
        }

        public void Add(MPoint p)
        {
            _path.Add(p);
            _length++;
        }

        public MPoint Next()
        {
            if (_currentPoint < _length)
            {
                _currentPoint++;
                return _path[_currentPoint];
            }

            return null;
        }

        public MPoint Last()
        {
            return _path.Last();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _length) { return; }
            if (_currentPoint > index)
            {
                _currentPoint--;
                if (_currentPoint < 0) _currentPoint = 0;
            }

            _length--;
            _path.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        private void Reverse()
        {
            _path.Reverse();
        }
    }
}
