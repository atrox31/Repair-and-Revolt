using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.LevelGeneration
{
    public enum RoomType
    {
        /* 0 */
        Empty, Elevator,
        /* 1 */
        StorageRoom, BreakRoom, UtilityRoom, Dormitoriy, ReceptionArea,
        /* 2 */
        LabRoom, Workshop, ServiceRoom, MeetingRoom, DataStorage,
        /* 3 */
        AICoreAnalysisRoom, CyberneticLab, SecurityCheckpoint, RobotMaintenanceBay, HighLevelTerminals,
        /* 4 */
        SurveillanceControlRoom, CommandCenter, ContainmentChambers, BackupGenerator, QuarantineLab,
        /* 5 */
        ARIS_ControlCore, BiometricVault, EliteGuardStation, SecureDataArchive, EmergencyShitdownZone,
        /* 6 */
        OfficeDrHart, OfficeDrWilliams, OfficeDrKline, OfficeDrThorne, OfficeDrSinclair, OfficeDeJenson,

    }
    [CreateAssetMenu(fileName = "New RoomTemplate", menuName = "Room/RoomTemplate")]
    public class RoomTemplate : ScriptableObject
    {
        [Header("Dimensions")]
        [SerializeField] public int RoomMinWidth = 2;
        [SerializeField] public int RoomMaxWidth = 5;
        [SerializeField] public int RoomMinHeight = 2;
        [SerializeField] public int RoomMaxHeight = 5;
        [SerializeField] public int RoomChanceToBeSquare = 50;

        [Header("Values")]
        [SerializeField] public int SecurityLevel = 0;
        [Header("Values")]
        [SerializeField] public RoomType Type = 0;

        [Header("Interior")]
        [SerializeField] public List<RoomElement> RoomObjects = new List<RoomElement>();

        [Header("Exterior")]
        [SerializeField] public List<GameObject> Wall0Exit;
        [SerializeField] public List<GameObject> Wall1Exit;

        [SerializeField] public List<GameObject> Corner0Exit;
        [SerializeField] public List<GameObject> Corner1ExitV1;
        [SerializeField] public List<GameObject> Corner1ExitV2;

        [SerializeField] public List<GameObject> Corner2Exit;

        [SerializeField] public List<GameObject> CenterRoom;

    }
}