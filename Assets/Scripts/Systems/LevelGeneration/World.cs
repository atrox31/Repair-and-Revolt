using System;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class World
    {
        public enum WorldTitle
        {
            ERROR, EMPTY, VALLEY, ROOM, DEAD_END
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
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

        public void ExpandWorld(int size = 4)
        {
            int new_width = Width + size;
            int new_height = Height + size;
            WorldTitle[] new_data = new WorldTitle[new_width * new_height];
            new_data.Fill(WorldTitle.EMPTY);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    new_data[i + j * new_height] = _data[i + j * Height];
                }
            }
            Width = new_width;
            Height = new_height;
            _data = new_data;
        }

        public static World CreateWorld(int width, int height)
        {
            return new World(width, height);
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
