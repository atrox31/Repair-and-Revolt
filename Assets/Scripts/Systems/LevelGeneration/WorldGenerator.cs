using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems.LevelGeneration
{

    public class WorldGenerator : MonoBehaviour, ISingleton<WorldGenerator>
    {
        [Header("World structure")]
        [SerializeField] List<WorldStructure> worldStructureList = new List<WorldStructure>(5);

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
        [SerializeField] int DeadEndsCount;

        [Header("Prefabs")]
        [SerializeField] GameObject PrefabValley;
        [SerializeField] GameObject PrefabRoom;
        [SerializeField] RoomTemplate PrefabElevator;

        [Header("Debug")]
        [SerializeField] GameObject PlayerPrefab;
        [SerializeField] bool GeneratePlayer = true;

        public static float WorldScaleX { get; } = 10f;
        public static float WorldScaleY { get; } = 10f;

        public NavMeshSurface navMeshSurface;
        private List<RoomList> RoomsToPlace = new List<RoomList>();

        private World[] worlds = new World[6];
        private List<Room> roomsGenerated = new List<Room>();

        private static int _level = 0;

        class RoomList
        {
            public int X { get; }
            public int Y { get; }
            public RoomTemplate Room { get; }
            public MPoint GetXY()
            {
                return new MPoint(X, Y);
            }
            public RoomList(int x, int y, int width, int height, RoomTemplate room)
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

        void CreateWorld(int level, MPoint elevatorXY)
        {
            RoomsToPlace.Clear();
            WorldStructure current_world_template = worldStructureList[level];
            worlds[level] = World.CreateWorld(WorldSizeWidth, WorldSizeHeight);

            TryToPlaceRoom(PrefabElevator, worlds[level], elevatorXY); // elevator

            // rooms generate
            for (int i = 0; i < current_world_template.RoomsCount(); i++)
            {
                TryToPlaceRoom(current_world_template.rooms[i], worlds[level]);
            }
        }

        void TryToPlaceRoom(RoomTemplate room, World world, MPoint point = null)
        {
            // try to place room
            int room_width;
            int room_height;
            Room.SetDimensions(out room_width, out room_height, room);

            int attempts = 0;
            int attempts_max = 24;
            while (true)
            {
                ++attempts;
                if (attempts > attempts_max)
                {
                    world.ExpandWorld(1);
                    Debug.Log("GenerateWorld error, max attemps reached! (rooms). Try to extend world");
                    attempts = 0;
                }

                int selectedX;
                int selectedY;
                if(point is null)
                {
                    selectedX = Random.Range(1, world.Width - room_width - 2);
                    selectedY = Random.Range(1, world.Height - room_height - 2);
                }
                else
                {
                    selectedX = point.x; 
                    selectedY = point.y;
                }

                selectedX = Mathf.FloorToInt(selectedX / room_width) * room_width;
                selectedY = Mathf.FloorToInt(selectedY / room_height) * room_height;

                bool error = false;
                for (int x = selectedX - 1; x < selectedX + room_width + 1; x++)
                {
                    for (int y = selectedY - 1; y < selectedY + room_height + 1; y++)
                    {
                        if (!world.IsInBoundry(x, y))
                        {
                            error = true;
                        }
                        else
                        if (world.Get(x, y) != World.WorldTitle.EMPTY)
                        {
                            error = true;
                        }
                    }
                }
                if (error) continue;
                // room fits
                for (int x = selectedX; x < selectedX + room_width; x++)
                {
                    for (int y = selectedY; y < selectedY + room_height; y++)
                    {
                        world.Set(x, y, World.WorldTitle.ROOM);
                    }
                }

                RoomsToPlace.Add(new RoomList(selectedX, selectedY, room_width, room_height, room));
                Debug.Log("Room added");
                break;
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

        void PlaceValleyPath(MPath path, World world)
        {
            if (path != null)
            {
                foreach (MPoint p in path)
                {
                    if (world.Get(p.x, p.y) == World.WorldTitle.VALLEY) continue;
                    if (world.Get(p.x, p.y) == World.WorldTitle.ROOM) continue;
                    world.Set(p.x, p.y, World.WorldTitle.VALLEY);
                }
            }
            else
            {
                Debug.Log("Cannot place path. Morron. void PlaceValleyPath(null)");
            }
        }

        void PlaceWorld(int level)
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
                    worlds[level].GetIntData(),
                    worlds[level].GetDimensions(),
                    new[] { (int)World.WorldTitle.ROOM, (int)World.WorldTitle.DEAD_END });

                PlaceValleyPath(path, worlds[level]);

                roomsConnected.Add(roomToConnect);

                if (!roomsConnected.Any()) roomsConnected.Add(nearestRoom);
            }

            // place rooms
            foreach (RoomList room in RoomsToPlace)
            {
                Room placed_room = Instantiate(PrefabRoom, new Vector3(room.X * WorldScaleX, -level * 10.0f, room.Y * WorldScaleY), Quaternion.identity, this.transform).GetComponent<Room>();
                placed_room.GenerateRoom(room.X, room.Y, room.Width, room.Height, room.Room, worlds[level]);
                roomsGenerated.Add(placed_room);
                Debug.Log("Placing room (" + room.X + "," + room.Y);
            }

            // set valley type
            // valleyList.Add(Instantiate(ValleyPrefabs.GetRandomElement(), new Vector3(p.x * _worldScaleX, 0, p.y * _worldScaleY), new Quaternion(0f, 0f, 0f, 1)));
            for (int x = 0; x < worlds[level].Width; x++)
            {
                for (int y = 0; y < worlds[level].Height; y++)
                {
                    if (worlds[level].Get(x, y) != World.WorldTitle.VALLEY) continue;

                    bool upValley = false;
                    bool downValley = false;
                    bool rightValley = false;
                    bool leftValley = false;

                    if (worlds[level].IsInBoundry(x - 1, y) && worlds[level].Get(x - 1, y) == World.WorldTitle.VALLEY) leftValley = true;
                    if (worlds[level].IsInBoundry(x + 1, y) && worlds[level].Get(x + 1, y) == World.WorldTitle.VALLEY) rightValley = true;
                    if (worlds[level].IsInBoundry(x, y - 1) && worlds[level].Get(x, y - 1) == World.WorldTitle.VALLEY) upValley = true;
                    if (worlds[level].IsInBoundry(x, y + 1) && worlds[level].Get(x, y + 1) == World.WorldTitle.VALLEY) downValley = true;

                    if (worlds[level].IsInBoundry(x - 1, y) && worlds[level].Get(x - 1, y) == World.WorldTitle.ROOM) leftValley = true;
                    if (worlds[level].IsInBoundry(x + 1, y) && worlds[level].Get(x + 1, y) == World.WorldTitle.ROOM) rightValley = true;
                    if (worlds[level].IsInBoundry(x, y - 1) && worlds[level].Get(x, y - 1) == World.WorldTitle.ROOM) upValley = true;
                    if (worlds[level].IsInBoundry(x, y + 1) && worlds[level].Get(x, y + 1) == World.WorldTitle.ROOM) downValley = true;

                    if (upValley && downValley && !rightValley && !leftValley) CreateValley(x, y, level, ValleyVertical);
                    if (upValley && downValley && rightValley && !leftValley) CreateValley(x, y, level, ValleyVerticalRight);
                    if (upValley && downValley && !rightValley && leftValley) CreateValley(x, y, level, ValleyVerticalLeft);

                    if (!upValley && !downValley && rightValley && leftValley) CreateValley(x, y, level, ValleyHorizontal);
                    if (upValley && !downValley && rightValley && leftValley) CreateValley(x, y, level, ValleyHorizontalUp);
                    if (!upValley && downValley && rightValley && leftValley) CreateValley(x, y, level, ValleyHorizontalDown);

                    if (upValley && downValley && rightValley && leftValley) CreateValley(x, y, level, Valley4Siedes);

                    if (upValley && !downValley && !rightValley && leftValley) CreateValley(x, y, level, ValleyCornerUpLeft);
                    if (upValley && !downValley && rightValley && !leftValley) CreateValley(x, y, level, ValleyCornerUpRight);
                    if (!upValley && downValley && !rightValley && leftValley) CreateValley(x, y, level, ValleyCornerDownLeft);
                    if (!upValley && downValley && rightValley && !leftValley) CreateValley(x, y, level, ValleyCornerDownRight);

                    if (!upValley && downValley && !rightValley && !leftValley) CreateValley(x, y, level, ValleyDeadEndDown);
                    if (upValley && !downValley && !rightValley && !leftValley) CreateValley(x, y, level, ValleyDeadEndUp);
                    if (!upValley && !downValley && !rightValley && leftValley) CreateValley(x, y, level, ValleyDeadEndRight);
                    if (!upValley && !downValley && rightValley && !leftValley) CreateValley(x, y, level, ValleyDeadEndLeft);
                }
            }
        }

        void CreateValley(int x, int y, int z, List<GameObject> obj)
        {
            GameObject selectedObject = obj.GetRandomElement();
            Instantiate(selectedObject, new Vector3(x * WorldScaleX - 5f, -z * 10.0f, y * WorldScaleY - 5f), selectedObject.transform.rotation, this.transform);
        }

        private void Awake()
        {
            MPoint elevatorXY = new(Mathf.FloorToInt(WorldSizeWidth / 2f), Mathf.FloorToInt(WorldSizeHeight / 2f));
            _level++;

            GenerateWorld(WorldSizeWidth, WorldSizeHeight, DeadEndsCount, elevatorXY, ((GeneratePlayer == true) && (_level == 1)));
        }
        public bool GenerateNavMesh()
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                Debug.LogError("NavMeshSurface not assigned!");
                return false;
            }

            // Clear any existing NavMesh
            navMeshSurface.RemoveData();

            // Bake the NavMesh
            navMeshSurface.BuildNavMesh();

            Debug.Log("NavMesh successfully generated!");
            return true;
        }

        void CreateLevel(int level, MPoint elevator)
        {
            CreateWorld(level, elevator);
            PlaceWorld(level);
        }

        public void GenerateWorld(int SizeWidth, int SizeHeight, int DeadEnds, MPoint elevatorXY, bool debug_GeneratePlayer)
        {
            WorldSizeWidth = SizeWidth;
            WorldSizeHeight = SizeHeight;
            DeadEndsCount = DeadEnds;

            CreateLevel(_level, elevatorXY);

            if (!GenerateNavMesh()) return;

            if (debug_GeneratePlayer)
            {
                Instantiate(PlayerPrefab, new Vector3(roomsGenerated[0].gameObject.transform.position.x, roomsGenerated[0].gameObject.transform.position.y, roomsGenerated[0].gameObject.transform.position.z), Quaternion.identity);
            }
        }
    }
}
