using System;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class World : IDisposable
    {
        private static World _singleton = null;
        public static World Instance { get { return _singleton; } }
        public enum WorldTitle
        {
            ERROR, EMPTY, VALLEY, ROOM, DEAD_END
        }
        public int Width { get; }
        public int Height { get; }
        WorldTitle[] _data;
        private World(int width, int height)
        {
            Width = width;
            Height = height;
            _data = new WorldTitle[Width * Height];
            for (int i = 0; i < width * height; i++)
            {
                _data[i] = WorldTitle.EMPTY;
            }

        }
        public void Dispose()
        {
            _singleton = null;
            GC.SuppressFinalize(this);
        }

        public static void CreateWorld(int width, int height)
        {
            _singleton?.Dispose();
            _singleton = new World(width, height);
        }

        public MPoint GetDimensions()
        {
            return new MPoint(Width, Height);
        }
        public int[] GetIntData()
        {
            return Array.ConvertAll(_data, value => (int)value);
        }
        public WorldTitle Get(int x, int y)
        {
            return _data[x + y * Width];
        }
        public WorldTitle Get(MPoint p)
        {
            return _data[p.x + p.y * Width];
        }
        public WorldTitle GetData(int v)
        {
            if (v < 0 || v > _data.Length) return WorldTitle.ERROR;
            else return _data[v];
        }
        public void Set(int x, int y, WorldTitle value)
        {
            _data[x + y * Width] = value;
        }
        public int GetWorldIndex(int x, int y)
        {
            return x + y * Width;
        }
        public int GetWorldIndex(MPoint p)
        {
            return p.x + p.y * Width;
        }
        public bool IsInBoundry(int x, int y)
        {
            if (
                x < 0 || y < 0 ||
                x >= Width || y >= Height
            ) return false;
            return true;
        }
        public bool IsInBoundry(MPoint p)
        {
            if (
                p.x < 0 || p.y < 0 ||
                p.x >= Width || p.y >= Height
            ) return false;
            return true;
        }
    }
}
