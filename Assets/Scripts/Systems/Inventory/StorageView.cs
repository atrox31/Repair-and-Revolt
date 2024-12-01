using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Systems.Inventory
{
    public abstract class StorageView : MonoBehaviour
    {
        public Slot[] Slots;

        protected static VisualElement GhostIcon;

        [SerializeField] protected UIDocument Document;
        [SerializeField] protected StyleSheet StyleSheet;

        protected VisualElement Root;
        protected VisualElement Container;

        private IEnumerator Start()
        {
            yield return StartCoroutine(InitializeView());
        }

        public abstract IEnumerator InitializeView(int size = 6);
    }
}
