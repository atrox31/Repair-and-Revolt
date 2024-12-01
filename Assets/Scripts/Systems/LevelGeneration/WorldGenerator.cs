using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class WorldGenerator : MonoBehaviour
    {
        [Header("Prefabs - room")]
        [SerializeField] List<GameObject> RoomPrefabs;

        [Header("Prefabs - valley")]
        [SerializeField] List<GameObject> ValleyVertical;
        [SerializeField] List<GameObject> ValleyVerticalLeft;
        [SerializeField] List<GameObject> ValleyVerticalRight;

        [SerializeField] List<GameObject> ValleyHorizontal;
        [SerializeField] List<GameObject> ValleyHorizontalUp;
        [SerializeField] List<GameObject> ValleyHorizontalDown;

        [SerializeField] List<GameObject> Valley4Siedes;

        [SerializeField] List<GameObject> ValleyDeadEndUp;
        [SerializeField] List<GameObject> ValleyDeadEndDown;
        [SerializeField] List<GameObject> ValleyDeadEndLeft;
        [SerializeField] List<GameObject> ValleyDeadEndRight;


        [SerializeField] List<GameObject> ValleyCornerUpRight;
        [SerializeField] List<GameObject> ValleyCornerUpLeft;
        [SerializeField] List<GameObject> ValleyCornerDownRight;
        [SerializeField] List<GameObject> ValleyCornerDownLeft;


        [Header("World size")]

        [SerializeField] int WorldSizeWidth;
        [SerializeField] int WorldSizeHeight;
        [SerializeField] int RoomCount;
        [SerializeField] int DeadEndsCount;
        List<RoomList> RoomsToPlace;

        [Header("Debug")]
        [SerializeField] GameObject PlayerPrefab;
        [SerializeField] bool GeneratePlayer = true;

        public static float WorldScaleX { get; } = 10f;
        public static float WorldScaleY { get; } = 10f;

        class RoomList
        {
            public int X { get; }
            public int Y { get; }
            public Room Room { get; }
            public MPoint GetXY()
            {
                return new MPoint(X, Y);
            }
            public RoomList(int x, int y, int width, int height, Room room)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
                this.Room = room;
            }
            public int Width { get; }
            public int Height { get; }
        }

        void CreateWorld()
        {
            World.CreateWorld(WorldSizeWidth, WorldSizeHeight);
            RoomsToPlace = new List<RoomList>();
            // rooms generate
            for (int i = 0; i < RoomCount; i++)
            {
                // try to place room
                Room selectedRoom = RoomPrefabs.GetRandomElement().GetComponent<Room>();
                int room_width, room_height;
                if (Random.Range(0, 100) > selectedRoom.RoomChanceToBeSquare)
                {
                    int square = Random.Range((selectedRoom.RoomMinWidth + selectedRoom.RoomMinHeight) / 2, (selectedRoom.RoomMaxWidth + selectedRoom.RoomMaxHeight) / 2 + 1);
                    room_width = square;
                    room_height = square;
                }
                else
                {
                    room_width = Random.Range(selectedRoom.RoomMinWidth, selectedRoom.RoomMaxWidth + 1); ;
                    room_height = Random.Range(selectedRoom.RoomMinHeight, selectedRoom.RoomMaxHeight + 1); ;
                }

                int attempts = 0;
                int attempts_max = 24;
                while (true)
                {
                    ++attempts;
                    if (attempts > attempts_max)
                    {
                        Debug.Log("GenerateWorld error, max attemps reached! (rooms)");
                        break;
                    }

                    int selectedX = Random.Range(1, World.Instance.Width - room_width - 2);
                    int selectedY = Random.Range(1, World.Instance.Height - room_height - 2);

                    selectedX = Mathf.FloorToInt(selectedX / room_width) * room_width;
                    selectedY = Mathf.FloorToInt(selectedY / room_height) * room_height;

                    bool error = false;
                    for (int x = selectedX - 1; x < selectedX + room_width + 1; x++)
                    {
                        for (int y = selectedY - 1; y < selectedY + room_height + 1; y++)
                        {
                            if (!World.Instance.IsInBoundry(x, y))
                            {
                                error = true;
                            }
                            else
                            if (World.Instance.Get(x, y) != World.WorldTitle.EMPTY)
                            {
                                error = true;
                            }
                        }
                    }
                    if (error) continue;
                    // room fits
                    int aaa = 0;
                    for (int x = selectedX; x < selectedX + room_width; x++)
                    {
                        for (int y = selectedY; y < selectedY + room_height; y++)
                        {
                            World.Instance.Set(x, y, World.WorldTitle.ROOM);
                            aaa++;
                        }
                    }

                    RoomsToPlace.Add(new RoomList(selectedX, selectedY, room_width, room_height, selectedRoom));
                    Debug.Log("Room added");
                    break;
                }
            }
            // make dead ends
            int attempts2 = 0;
            int attempts2_max = 10;
            while (true)
            {
                ++attempts2;
                if (attempts2 > attempts2_max)
                {
                    Debug.Log("GenerateWorld error, max attemps reached (dead ends)!");
                    break;
                }

                int selectedX = Random.Range(0, World.Instance.Width);
                int selectedY = Random.Range(0, World.Instance.Height);

                bool error = false;
                if (World.Instance.Get(selectedX, selectedY) != World.WorldTitle.EMPTY)
                {
                    error = true;
                }
                if (error) continue;
                World.Instance.Set(selectedX, selectedY, World.WorldTitle.DEAD_END);

            }

        }


        RoomList GetNearestRoom(List<RoomList> groupToSearch, RoomList pickedRoom)
        {
            float distance = float.MaxValue;
            RoomList answer_room = null;

            foreach (RoomList roomList in groupToSearch)
            {
                if (roomList == pickedRoom) continue;

                float current_distance = roomList.GetXY().Distance(pickedRoom.GetXY());

                if (current_distance < distance)
                {
                    distance = current_distance;
                    answer_room = roomList;
                }
            }

            return answer_room;
        }

        void PlaceValleyPath(MPath path)
        {
            if (path != null)
            {
                foreach (MPoint p in path)
                {
                    if (World.Instance.Get(p.x, p.y) == World.WorldTitle.VALLEY) continue;
                    if (World.Instance.Get(p.x, p.y) == World.WorldTitle.ROOM) continue;
                    World.Instance.Set(p.x, p.y, World.WorldTitle.VALLEY);
                }
            }
            else
            {
                Debug.Log("Cannot place path. Morron. void PlaceValleyPath(null)");
            }
        }

        void PlaceWorld()
        {
            var roomsConnected = new List<RoomList>();

            foreach (var roomToConnect in RoomsToPlace)
            {
                var roomsToSearch = roomsConnected.Any()
                    ? roomsConnected
                    : RoomsToPlace;

                var nearestRoom = GetNearestRoom(roomsToSearch, roomToConnect);

                if (nearestRoom == null)
                {
                    Debug.Log("Failed to connect room to anything, something is wrong. Moron.");
                    return;
                }

                var path = MPath.FindPath(
                    nearestRoom.GetXY(),
                    roomToConnect.GetXY(),
                    World.Instance.GetIntData(),
                    World.Instance.GetDimensions(),
                    new[] { (int)World.WorldTitle.ROOM, (int)World.WorldTitle.DEAD_END });

                PlaceValleyPath(path);

                roomsConnected.Add(roomToConnect);

                if (!roomsConnected.Any()) roomsConnected.Add(nearestRoom);
            }

            // make valley
            /*
            for (int i = 0; i < RoomsToPlace.Count - 1; ++i)
            {
                RoomList roomA = RoomsToPlace[i];
                RoomList roomB = RoomsToPlace[i + 1];
                MPath path = MPath.FindPath(
                    roomA.GetXY(), roomB.GetXY(),
                    World.Instance.GetIntData(), 
                    World.Instance.GetDimensions(), 
                    new int[] { (int)World.WorldTitle.ROOM, (int)World.WorldTitle.DEAD_END },
                    false
                );
            
                if (path != null)
                {
                    foreach (MPoint p in path)
                    {
                        if (World.Instance.Get(p.x, p.y) == World.WorldTitle.VALLEY) continue;
                        if (World.Instance.Get(p.x, p.y) == World.WorldTitle.ROOM) continue;
                        World.Instance.Set(p.x, p.y, World.WorldTitle.VALLEY);
                    }

                }
            }
            */
            // place rooms
            foreach (RoomList room in RoomsToPlace)
            {
                Room placed_room = Instantiate(room.Room, new Vector3(room.X * WorldScaleX, 0, room.Y * WorldScaleY), Quaternion.identity);
                placed_room.GenerateRoom(room.X, room.Y, room.Width, room.Height);
                Debug.Log("Placing room (" + room.X + "," + room.Y);
            }

            // set valley type
            // valleyList.Add(Instantiate(ValleyPrefabs.GetRandomElement(), new Vector3(p.x * _worldScaleX, 0, p.y * _worldScaleY), new Quaternion(0f, 0f, 0f, 1)));
            for (int x = 0; x < World.Instance.Width; x++)
            {
                for (int y = 0; y < World.Instance.Height; y++)
                {
                    if (World.Instance.Get(x, y) != World.WorldTitle.VALLEY) continue;

                    bool upValley = false;
                    bool downValley = false;
                    bool rightValley = false;
                    bool leftValley = false;

                    if (World.Instance.IsInBoundry(x - 1, y) && World.Instance.Get(x - 1, y) == World.WorldTitle.VALLEY) leftValley = true;
                    if (World.Instance.IsInBoundry(x + 1, y) && World.Instance.Get(x + 1, y) == World.WorldTitle.VALLEY) rightValley = true;
                    if (World.Instance.IsInBoundry(x, y - 1) && World.Instance.Get(x, y - 1) == World.WorldTitle.VALLEY) upValley = true;
                    if (World.Instance.IsInBoundry(x, y + 1) && World.Instance.Get(x, y + 1) == World.WorldTitle.VALLEY) downValley = true;

                    if (World.Instance.IsInBoundry(x - 1, y) && World.Instance.Get(x - 1, y) == World.WorldTitle.ROOM) leftValley = true;
                    if (World.Instance.IsInBoundry(x + 1, y) && World.Instance.Get(x + 1, y) == World.WorldTitle.ROOM) rightValley = true;
                    if (World.Instance.IsInBoundry(x, y - 1) && World.Instance.Get(x, y - 1) == World.WorldTitle.ROOM) upValley = true;
                    if (World.Instance.IsInBoundry(x, y + 1) && World.Instance.Get(x, y + 1) == World.WorldTitle.ROOM) downValley = true;

                    if (upValley && downValley && !rightValley && !leftValley) CreateValley(x, y, ValleyVertical);
                    if (upValley && downValley && rightValley && !leftValley) CreateValley(x, y, ValleyVerticalRight);
                    if (upValley && downValley && !rightValley && leftValley) CreateValley(x, y, ValleyVerticalLeft);

                    if (!upValley && !downValley && rightValley && leftValley) CreateValley(x, y, ValleyHorizontal);
                    if (upValley && !downValley && rightValley && leftValley) CreateValley(x, y, ValleyHorizontalUp);
                    if (!upValley && downValley && rightValley && leftValley) CreateValley(x, y, ValleyHorizontalDown);

                    if (upValley && downValley && rightValley && leftValley) CreateValley(x, y, Valley4Siedes);

                    if (upValley && !downValley && !rightValley && leftValley) CreateValley(x, y, ValleyCornerUpLeft);
                    if (upValley && !downValley && rightValley && !leftValley) CreateValley(x, y, ValleyCornerUpRight);
                    if (!upValley && downValley && !rightValley && leftValley) CreateValley(x, y, ValleyCornerDownLeft);
                    if (!upValley && downValley && rightValley && !leftValley) CreateValley(x, y, ValleyCornerDownRight);

                    if (!upValley && downValley && !rightValley && !leftValley) CreateValley(x, y, ValleyDeadEndDown);
                    if (upValley && !downValley && !rightValley && !leftValley) CreateValley(x, y, ValleyDeadEndUp);
                    if (!upValley && !downValley && !rightValley && leftValley) CreateValley(x, y, ValleyDeadEndRight);
                    if (!upValley && !downValley && rightValley && !leftValley) CreateValley(x, y, ValleyDeadEndLeft);
                }
            }
        }

        void CreateValley(int x, int y, List<GameObject> obj)
        {
            GameObject selectedObject = obj.GetRandomElement();
            Instantiate(selectedObject, new Vector3(x * WorldScaleX - 5f, 0, y * WorldScaleY - 5f), selectedObject.transform.rotation);
        }

        private void Start()
        {
            GenerateWorld(WorldSizeWidth, WorldSizeHeight, RoomCount, DeadEndsCount);
        }

        public void GenerateWorld(int SizeWidth, int SizeHeight, int RoomsToPlaceCount, int DeadEnds)
        {
            WorldSizeWidth = SizeWidth;
            WorldSizeHeight = SizeHeight;
            RoomCount = RoomsToPlaceCount;
            DeadEndsCount = DeadEnds;

            CreateWorld();
            PlaceWorld();

            if (GeneratePlayer)
            {
                RoomList random_room = RoomsToPlace.GetRandomElement();
                Instantiate(PlayerPrefab, new Vector3(random_room.X * WorldScaleX, 0.5f, random_room.Y * WorldScaleY), Quaternion.identity);
            }
        }
    }
}
