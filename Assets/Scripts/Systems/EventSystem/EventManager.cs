using Assets.Scripts.Interfaces;
using Assets.Scripts.Systems.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Systems.EventSystem
{
    public static class EventManager
    {
        public static readonly PlayerEvents Player = new();

        public class PlayerEvents
        {
            public UnityAction<GameObject, IInventoryItem> OnItemPicked;
            public UnityAction OnItemUse;
            public UnityAction<Item> OnItemUsed;
        }
    }
}
