using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    [CreateAssetMenu(menuName = "Inventory item")]
    public class Item : ScriptableObject
    {
        public string Name;
        public Texture2D Icon;
    }
}
