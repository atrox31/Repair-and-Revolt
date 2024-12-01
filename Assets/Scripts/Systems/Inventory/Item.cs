using System;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    public class Item
    {
        public readonly Guid Id;
        public readonly string Name;
        public readonly Sprite Icon;

        public Item(string name, Sprite icon)
        {
            Id = Guid.NewGuid();
            Name = name;
            Icon = icon;
        }
    }
}
