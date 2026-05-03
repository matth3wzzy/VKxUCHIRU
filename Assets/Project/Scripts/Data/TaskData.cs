using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "Game/Task")]
public class TaskData : ScriptableObject
{
    public string taskId;
    public string taskDescription;
    public int rewardPoints = 10;
    public Sprite taskIcon;
    public string taskType = "manual"; // "auto" или "manual"
}