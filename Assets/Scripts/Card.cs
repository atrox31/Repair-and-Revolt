using Assets.QuickOutline.Scripts;
using Assets.Scripts.Systems.Inventory;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Outline))]
    public class Card : InventoryItem
    {
        private Outline _outline;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
        }

        public override void Use()
        {
            Debug.Log("Card item used!");
        }
    }
}
