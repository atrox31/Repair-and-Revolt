using Assets.Scripts.Extensions;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Assets.Scripts.Systems.Inventory
{
    public class Slot : VisualElement
    {
        public Image Icon;
        public Label Label;
        public Item Item;

        public Slot()
        {
            Icon = this.CreateChild<Image>("slotIcon");
            Label = this.CreateChild<Label>("slotLabel");
        }

        public void Start()
        {
            Reset();
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

        public void Set([CanBeNull] Item item)
        {
            if (item is null)
            {
                Reset();
                return;
            }

            Item = item;
            Label.text = item.Name;
            Icon.image = item.Icon;
            Icon.StretchToParentSize();
        }

        private void Reset()
        {
            Item = null;
            Label.text = string.Empty;
            Icon.image = null;
        }
    }
}
