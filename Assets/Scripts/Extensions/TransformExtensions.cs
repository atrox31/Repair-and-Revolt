using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static IInteractable GetTargetAsInteractable(this Transform source, float range)
        {
            var ray = new Ray(source.position, source.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, range)
                && hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                return interactable;

            return null;
        }
    }
}
