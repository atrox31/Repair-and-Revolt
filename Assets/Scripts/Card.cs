using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class Card : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("It is working!");
        }
    }
}
