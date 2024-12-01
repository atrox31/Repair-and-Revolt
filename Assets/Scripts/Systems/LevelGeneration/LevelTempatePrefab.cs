using System.Collections.Generic;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public class LevelTempatePrefab : MonoBehaviour
    {
        [SerializeField] List<GameObject> EntrySocket;
        [SerializeField] public int SizeWidth = 1;
        [SerializeField] public int SizeHeight = 1;

        public List<GameObject> GetSocketList()
        {
            return EntrySocket;
        }
        public GameObject GetRandomSocket() { 
            return EntrySocket.GetRandomElement();
        }

        private void Start()
        {
            foreach (GameObject socket in EntrySocket)
            {
                // destroy all socket so they visibled only in editor
                socket.SetActive(false);
            }
        }
    }
}
