using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    [CreateAssetMenu(fileName = "New RoomElement", menuName = "Room/RoomElement")]
    public class RoomElement : ScriptableObject
    {
        [SerializeField] public bool CanBePlacedOnEmpty;
        [SerializeField] public bool CanBePlacedOnWall;
        [SerializeField] public bool CanBePlacedOnCorner;
        [SerializeField] public bool CanBePlacedOnEntry;

        [SerializeField] public bool IsInteractable;

        [SerializeField] public List<GameObject> Objects;
    }
}