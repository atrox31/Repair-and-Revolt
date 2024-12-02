using Assets.Scripts.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    public abstract class InventoryItem : MonoBehaviour, IInventoryItem
    {
        private bool _isAnimationRunning;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _time;
        private const float TimeDuration = 10f;

        private void Update()
        {
            if (_isAnimationRunning)
            {
                _time += TimeDuration * Time.deltaTime;

                if (_time > 1)
                {
                    DestroyGameObject();
                    return;
                }

                transform.position = Vector3.Lerp(_startPosition, _endPosition, _time);
            }
        }

        public virtual void Pick()
        {
            if (!_isAnimationRunning)
            {
                GetComponent<BoxCollider>().enabled = false;
                _time = 0;
                _isAnimationRunning = true;
                _startPosition = gameObject.transform.position;
            }
        }

        public virtual void Interact(Transform source)
        {
            _endPosition = source.position - new Vector3(0, source.position.y / 2, 0);
            Pick();
        }

        public abstract void Use();

        private void DestroyGameObject()
        {
            if (!gameObject.IsDestroyed())
                Destroy(gameObject);
        }
    }
}