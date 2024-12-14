using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Systems.LevelGeneration
{
    [CreateAssetMenu(fileName = "WorldStructure", menuName = "Room/WorldStructure")]
    public class WorldStructure : ScriptableObject
    {
        public List<RoomTemplate> rooms;
        public int RoomsCount() { return rooms.Count; }
    }
}