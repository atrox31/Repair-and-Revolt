using System.Collections;

namespace Assets.Scripts.Systems.Inventory
{
    public class InventoryController
    {
        private readonly InventoryView _view;
        private readonly int _capacity;

        public InventoryController(InventoryView view, int capacity)
        {
            _view = view;
            _capacity = capacity;

            view.StartCoroutine(Initialize());
        }

        public void SwitchInventorySlot(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _capacity)
                _view.Slots[slotIndex].SetActive();
        }

        private IEnumerator Initialize()
        {
            yield return _view.InitializeView(_capacity);
        }
    }
}
