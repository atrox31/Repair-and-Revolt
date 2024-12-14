using Assets.Scripts.Systems.Inventory;

namespace Assets.Scripts.Interfaces
{
    public interface IInventoryItem : IInteractable
    {
        public Item Item { get; }
        void Pick();
        void Use();
    }
}