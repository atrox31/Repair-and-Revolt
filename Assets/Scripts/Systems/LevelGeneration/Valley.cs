using System.Collections.Generic;
using UnityEngine;

public class Valley
{
    [SerializeField] List<GameObject> ValleyVertical;
    [SerializeField] List<GameObject> ValleyVerticalLeft;
    [SerializeField] List<GameObject> ValleyVerticalRight;

    [SerializeField] List<GameObject> ValleyHorizontal;
    [SerializeField] List<GameObject> ValleyHorizontalUp;
    [SerializeField] List<GameObject> ValleyHorizontalDown;

    [SerializeField] List<GameObject> Valley4Siedes;

    [SerializeField] List<GameObject> ValleyDeadEndUp;
    [SerializeField] List<GameObject> ValleyDeadEndDown;
    [SerializeField] List<GameObject> ValleyDeadEndLeft;
    [SerializeField] List<GameObject> ValleyDeadEndRight;

    [SerializeField] List<GameObject> ValleyCornerUpRight;
    [SerializeField] List<GameObject> ValleyCornerUpLeft;
    [SerializeField] List<GameObject> ValleyCornerDownRight;
    [SerializeField] List<GameObject> ValleyCornerDownLeft;
}
