using UnityEngine;

public class DebugRoomValueInfo : MonoBehaviour
{
    [SerializeField] public int direction;
    [SerializeField] public string model;
    public void SetData(int direction, string model)
    {
        this.direction = direction;
        this.model = model;
    }
}