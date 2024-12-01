using System.Collections.Generic;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class Room : MonoBehaviour
    {
        [SerializeField] public int RoomMinWidth = 2;
        [SerializeField] public int RoomMaxWidth= 5;
        [SerializeField] public int RoomMinHeight  = 2;
        [SerializeField] public int RoomMaxHeight = 5;
        [SerializeField] public int RoomChanceToBeSquare = 50;

        [SerializeField] List<GameObject> Wall0Exit;
        [SerializeField] List<GameObject> Wall1Exit;

        [SerializeField] List<GameObject> Corner0Exit;
        [SerializeField] List<GameObject> Corner1ExitV1;
        [SerializeField] List<GameObject> Corner1ExitV2;

        [SerializeField] List<GameObject> Corner2Exit;

        [SerializeField] List<GameObject> CenterRoom;

        private int Width;
        private int Height;

        public int GetHeight()
        {
            return Height;
        }
        public int GetWidth()
        {
            return Width;
        }

        private void Start()
        {
        
        }
        private bool _isGenerated = false;
        public void GenerateRoom(int px, int py, int width, int height)
        {
            if (_isGenerated)
            {
                Debug.Log("This room is generated, can not do this twice! You moron -.-");
                return;
            }

            Width = width;
            Height = height;

            _isGenerated = true;
            Debug.Log("Generate room width(" + Width.ToString() + ") height (" + Height.ToString() + ")");
            //
            for (int x = px; x < px + Width; x++)
            {
                for (int y = py; y < py + Height; y++)
                {
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

                    if (World.Instance.IsInBoundry(x - 1, y) && (World.Instance.Get(x - 1, y) == World.WorldTitle.VALLEY)) leftValley = true;
                    if (World.Instance.IsInBoundry(x + 1, y) && (World.Instance.Get(x + 1, y) == World.WorldTitle.VALLEY)) rightValley = true;
                    if (World.Instance.IsInBoundry(x, y - 1) && (World.Instance.Get(x, y - 1) == World.WorldTitle.VALLEY)) upValley = true;
                    if (World.Instance.IsInBoundry(x, y + 1) && (World.Instance.Get(x, y + 1) == World.WorldTitle.VALLEY)) downValley = true;

                    if (World.Instance.IsInBoundry(x - 1, y) && (World.Instance.Get(x - 1, y) == World.WorldTitle.ROOM)) leftRoom = true;
                    if (World.Instance.IsInBoundry(x + 1, y) && (World.Instance.Get(x + 1, y) == World.WorldTitle.ROOM)) rightRoom = true;
                    if (World.Instance.IsInBoundry(x, y - 1) && (World.Instance.Get(x, y - 1) == World.WorldTitle.ROOM)) upRoom = true;
                    if (World.Instance.IsInBoundry(x, y + 1) && (World.Instance.Get(x, y + 1) == World.WorldTitle.ROOM)) downRoom = true;

                    if (upRoom && downRoom && rightRoom && leftRoom)
                    {
                        center = true;
                    }

                    if (!center)
                    {
                        // flat wall
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 2, Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, Wall0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, Wall0Exit.GetRandomElement());
                            continue;
                        }
                        // flat wall 1 exit
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 2, Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, Wall1Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, Wall1Exit.GetRandomElement());
                            continue;
                        }
                        // corners - no valley
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, Corner0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, Corner0Exit.GetRandomElement());
                            continue;

                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, Corner0Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, Corner0Exit.GetRandomElement());
                            continue;
                        }

                        // corners 1 valley horizontal entrance
                        if (!upValley && !downValley && !rightValley && leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, Corner1ExitV1.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, Corner1ExitV1.GetRandomElement());
                            continue;
                        }
                    
                        // corners 1 valley vertical entrance
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 2, Corner1ExitV1.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 1, Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, 3, Corner1ExitV2.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, 0, Corner1ExitV1.GetRandomElement());
                            continue;
                        }

                        // corners 2 valley
                        if (upValley && !downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, 2, Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, 1, Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, 0, Corner2Exit.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, 3, Corner2Exit.GetRandomElement());
                            continue;
                        }
                   
                    }
                    else
                    {
                        // center
                        CreatePart(x, y, 0, CenterRoom.GetRandomElement());
                    }
                }
            }
        }

        void CreatePart(int x, int y, int rotate, GameObject room_type)
        {
            GameObject t = Instantiate(room_type,
                new Vector3(
                    x * WorldGenerator.WorldScaleX - (WorldGenerator.WorldScaleX / 2),
                    0,
                    y * WorldGenerator.WorldScaleY - (WorldGenerator.WorldScaleY / 2)),
                Quaternion.identity,
                transform);
            t.transform.Rotate(new Vector3(0, 1, 0), rotate * 90);
            t.GetComponent<DebugRoomValueInfo>().SetData(rotate, room_type.name);
 
        }
    }
}
