using System.Collections.Generic;
using Assets.Prefabs.Level.rooms.var2;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
   
    public class Room : MonoBehaviour
    {
        public static void SetDimensions(out int width, out int height, RoomTemplate roomTemplate)
        {
            if (Random.Range(0, 100) > roomTemplate.RoomChanceToBeSquare)
            {
                int square = Random.Range((roomTemplate.RoomMinWidth + roomTemplate.RoomMinHeight) / 2, (roomTemplate.RoomMaxWidth + roomTemplate.RoomMaxHeight) / 2 + 1);
                width = square;
                height = square;
            }
            else
            {
                width = Random.Range(roomTemplate.RoomMinWidth, roomTemplate.RoomMaxWidth + 1);
                height = Random.Range(roomTemplate.RoomMinHeight, roomTemplate.RoomMaxHeight + 1);
            }
        }

        private enum RoomPlanSpace
        {
            NotSet, Empty, Wall, Corner, Entry
        }
        public RoomType Type { get; private set; } = RoomType.Empty;
        public int SecutityLevel { get; private set; }

        private int Width;
        private int Height;
        private RoomTemplate roomTemplate;

        public int GetHeight()
        {
            return Height;
        }
        public int GetWidth()
        {
            return Width;
        }

        private bool _isGenerated = false;
        public void GenerateRoom(int px, int py, int width, int height, RoomTemplate roomTemplate, World world)
        {
            if (_isGenerated)
            {
                Debug.Log("This room is generated, can not do this twice! You moron -.-");
                return;
            }
            this.roomTemplate = roomTemplate;
            Width = width;
            Height = height;
            SecutityLevel = roomTemplate.SecurityLevel;
            Type = roomTemplate.Type;

            RoomPlanSpace[] room_plan = new RoomPlanSpace[Width * Height];
            // safe guard for debug
            room_plan.Fill(RoomPlanSpace.NotSet);

            CreateFloorAndWalls(room_plan, px, py, world);
            GenerateInterior(room_plan);

            _isGenerated = true;
            Debug.Log("Generate room width(" + Width.ToString() + ") height (" + Height.ToString() + ")");
            //
        }

        void CreateFloorAndWalls(RoomPlanSpace[] room_plan, int px, int py, World world)
        {
            int index = -1;
            for (int x = px; x < px + Width; x++)
            {
                for (int y = py; y < py + Height; y++)
                {
                    index++;
                    //Debug.Log("Generate title");
                    bool upValley = false;
                    bool downValley = false;
                    bool rightValley = false;
                    bool leftValley = false;

                    bool upRoom = false;
                    bool downRoom = false;
                    bool rightRoom = false;
                    bool leftRoom = false;

                    bool center = false;

                    if (world.IsInBoundry(x - 1, y) && (world.Get(x - 1, y) == World.WorldTitle.VALLEY)) leftValley = true;
                    if (world.IsInBoundry(x + 1, y) && (world.Get(x + 1, y) == World.WorldTitle.VALLEY)) rightValley = true;
                    if (world.IsInBoundry(x, y - 1) && (world.Get(x, y - 1) == World.WorldTitle.VALLEY)) upValley = true;
                    if (world.IsInBoundry(x, y + 1) && (world.Get(x, y + 1) == World.WorldTitle.VALLEY)) downValley = true;

                    if (world.IsInBoundry(x - 1, y) && (world.Get(x - 1, y) == World.WorldTitle.ROOM)) leftRoom = true;
                    if (world.IsInBoundry(x + 1, y) && (world.Get(x + 1, y) == World.WorldTitle.ROOM)) rightRoom = true;
                    if (world.IsInBoundry(x, y - 1) && (world.Get(x, y - 1) == World.WorldTitle.ROOM)) upRoom = true;
                    if (world.IsInBoundry(x, y + 1) && (world.Get(x, y + 1) == World.WorldTitle.ROOM)) downRoom = true;

                    if (upRoom && downRoom && rightRoom && leftRoom)
                    {
                        center = true;
                        room_plan[index] = RoomPlanSpace.Empty;
                    }
                    else
                    {
                        room_plan[index] = (upValley || downValley || leftValley || rightValley) ? RoomPlanSpace.Entry : RoomPlanSpace.Wall;
                    }

                    if (!center)
                    {
                        // flat wall
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, roomTemplate.Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 2, roomTemplate.Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, roomTemplate.Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, roomTemplate.Wall0Exit.GetRandomElement());
                            continue;
                        }
                        // flat wall 1 exit
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 2, roomTemplate.Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, roomTemplate.Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, roomTemplate.Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, roomTemplate.Wall1Exit.GetRandomElement());
                            continue;
                        }
                        // corners - no valley
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, roomTemplate.Corner0Exit.GetRandomElement());
                            room_plan[index] = RoomPlanSpace.Corner;
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, roomTemplate.Corner0Exit.GetRandomElement());
                            room_plan[index] = RoomPlanSpace.Corner;
                            continue;

                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, roomTemplate.Corner0Exit.GetRandomElement());
                            room_plan[index] = RoomPlanSpace.Corner;
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, roomTemplate.Corner0Exit.GetRandomElement());
                            room_plan[index] = RoomPlanSpace.Corner;
                            continue;
                        }

                        // corners 1 valley horizontal entrance
                        if (!upValley && !downValley && !rightValley && leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, roomTemplate.Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, roomTemplate.Corner1ExitV1.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, roomTemplate.Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, roomTemplate.Corner1ExitV1.GetRandomElement());
                            continue;
                        }

                        // corners 1 valley vertical entrance
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, roomTemplate.Corner1ExitV1.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, roomTemplate.Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, roomTemplate.Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, roomTemplate.Corner1ExitV1.GetRandomElement());
                            continue;
                        }

                        // corners 2 valley
                        if (upValley && !downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, 2, roomTemplate.Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, 1, roomTemplate.Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, 0, roomTemplate.Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, 3, roomTemplate.Corner2Exit.GetRandomElement());
                            continue;
                        }

                    }
                    else
                    {
                        // center
                        CreatePart(x, y, 0, roomTemplate.CenterRoom.GetRandomElement());
                    }
                }
            }
        }

        void CreatePart(int x, int y, int rotate, GameObject room_type)
        {
            GameObject t = Instantiate(room_type,
                new Vector3(
                    x * WorldGenerator.WorldScaleX - (WorldGenerator.WorldScaleX / 2),
                    transform.position.y,
                    y * WorldGenerator.WorldScaleY - (WorldGenerator.WorldScaleY / 2)),
                Quaternion.identity,
                transform);
            t.transform.Rotate(new Vector3(0, 1, 0), rotate * 90);
 
        }

        void GenerateInterior(RoomPlanSpace[] room_plan)
        {

        }
    }
}
