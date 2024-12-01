using System.Collections;
using System.Linq;
using Assets.Scripts.Extensions;

namespace Assets.Scripts.Systems.Inventory
{
    public class InventoryView : StorageView
    {
        public override IEnumerator InitializeView(int size = 6)
        {
            Slots = new Slot[size];
            Root = Document.rootVisualElement;
            Root.Clear();
            Root.styleSheets.Add(StyleSheet);

            Container = Root.CreateChild("container");

            var inventory = Container.CreateChild("inventory");

            var slotsContainer = inventory.CreateChild("slotsContainer");

            for (var i = 0; i < size; i++)
            {
                var slot = slotsContainer.CreateChild<Slot>("slot");
                slot.Start();
                Slots[i] = slot;
            }

            Slots.First().SetActive();

            yield return null;
        }
    }
}