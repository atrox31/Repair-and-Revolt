namespace Assets.Scripts.Interfaces
{
    public interface IInventoryItem : IInteractable
    {
        void Pick();
        void Use();
    }
}