using System.Collections.Generic;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class Room_backup : MonoBehaviour
    {
        [SerializeField] public int RoomMinWidth = 2;
        [SerializeField] public int RoomMaxWidth= 5;
        [SerializeField] public int RoomMinHeight  = 2;
        [SerializeField] public int RoomMaxHeight = 5;
        [SerializeField] public int RoomChanceToBeSquare = 50;

        [SerializeField] List<GameObject> Wall0ExitUp;
        [SerializeField] List<GameObject> Wall0ExitDown;
        [SerializeField] List<GameObject> Wall0ExitLeft;
        [SerializeField] List<GameObject> Wall0ExitRight;

        [SerializeField] List<GameObject> Wall1ExitUp;
        [SerializeField] List<GameObject> Wall1ExitDown;
        [SerializeField] List<GameObject> Wall1ExitLeft;
        [SerializeField] List<GameObject> Wall1ExitRight;

        [SerializeField] List<GameObject> Corner0ExitUpRight;
        [SerializeField] List<GameObject> Corner0ExitUpLeft;
        [SerializeField] List<GameObject> Corner0ExitDownRight;
        [SerializeField] List<GameObject> Corner0ExitDownLeft;

        [SerializeField] List<GameObject> Corner1ExitUpLeftVertical;
        [SerializeField] List<GameObject> Corner1ExitUpLeftHorizontal;
        [SerializeField] List<GameObject> Corner1ExitUpRightVertical;
        [SerializeField] List<GameObject> Corner1ExitUpRightHorizontal;
        [SerializeField] List<GameObject> Corner1ExitDownRightVertical;
        [SerializeField] List<GameObject> Corner1ExitDownRightHorizontal;
        [SerializeField] List<GameObject> Corner1ExitDownLeftVertical;
        [SerializeField] List<GameObject> Corner1ExitDownLeftHorizontal;

        [SerializeField] List<GameObject> Corner2ExitUpRight;
        [SerializeField] List<GameObject> Corner2ExitUpLeft;
        [SerializeField] List<GameObject> Corner2ExitDownRight;
        [SerializeField] List<GameObject> Corner2ExitDownLeft;

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
                            CreatePart(x, y, Wall0ExitUp.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Wall0ExitDown.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Wall0ExitLeft.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Wall0ExitRight.GetRandomElement());
                            continue;
                        }
                        // flat wall 1 exit
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Wall1ExitDown.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Wall1ExitUp.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Wall1ExitLeft.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Wall1ExitRight.GetRandomElement());
                            continue;
                        }
                        // corners - no valley
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner0ExitDownRight.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner0ExitDownLeft.GetRandomElement());
                            continue;

                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner0ExitUpLeft.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner0ExitUpRight.GetRandomElement());
                            continue;
                        }

                        // corners 1 valley horizontal entrance
                        if (!upValley && !downValley && !rightValley && leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitDownLeftHorizontal.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitDownRightHorizontal.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitUpRightHorizontal.GetRandomElement());
                            continue;
                        }
                        if (!upValley && !downValley && !rightValley && leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitUpLeftHorizontal.GetRandomElement());
                            continue;
                        }
                    
                        // corners 1 valley vertical entrance
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitDownLeftVertical.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && !rightValley && !leftValley
                            && !upRoom && downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitDownRightVertical.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && rightRoom && !leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitUpLeftVertical.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && !leftValley
                            && upRoom && !downRoom && !rightRoom && leftRoom)
                        {
                            CreatePart(x, y, Corner1ExitUpRightVertical.GetRandomElement());
                            continue;
                        }

                        // corners 2 valley
                        if (upValley && !downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, Corner2ExitUpLeft.GetRandomElement());
                            continue;
                        }
                        if (upValley && !downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, Corner2ExitDownLeft.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && rightValley && !leftValley)
                        {
                            CreatePart(x, y, Corner2ExitDownRight.GetRandomElement());
                            continue;
                        }
                        if (!upValley && downValley && !rightValley && leftValley)
                        {
                            CreatePart(x, y, Corner2ExitUpRight.GetRandomElement());
                            continue;
                        }
                   
                    }
                    else
                    {
                        // center
                        CreatePart(x, y, CenterRoom.GetRandomElement());
                    }
                }
            }
        }

        void CreatePart(int x, int y, GameObject room_type)
        {
            Instantiate(room_type, new Vector3(x * WorldGenerator.WorldScaleX, 0, y * WorldGenerator.WorldScaleY), Quaternion.identity, transform);
 
        }
    }
}
