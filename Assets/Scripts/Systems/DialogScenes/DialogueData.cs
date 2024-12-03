using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.DialogScenes
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueLine> lines;
    }
}