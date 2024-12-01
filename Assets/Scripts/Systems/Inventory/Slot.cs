using System;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Systems.Inventory
{
    public class Slot : VisualElement
    {
        public Image Icon;
        public Label IndexLabel;
        public Sprite BaseSprite;

        public Guid ItemId { get; private set; } = Guid.Empty;
        public int Index => parent.IndexOf(this);

        public Slot()
        {
            Icon = this.CreateChild<Image>("slotIcon");
            IndexLabel = this.CreateChild<Label>("slotIndex");
        }

        public void Start()
        {
            IndexLabel.text = $"{Index + 1}";
        }

        public void SetActive()
        {
            var slots = parent.Query<Slot>().ToList();

            foreach (var slot in slots)
            {
                slot.RemoveFromClassList("slotActive");
            }

            this.AddClass("slotActive");
        }

        public void Set(Guid id, Sprite icon)
        {
            ItemId = id;
            BaseSprite = icon;

            Icon.image = BaseSprite != null ? icon.texture : null;
        }
    }
}
