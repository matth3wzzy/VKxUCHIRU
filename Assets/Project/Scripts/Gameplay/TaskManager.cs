using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public TaskData[] allTasks;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public TaskData GetRandomTask()
    {
        if (allTasks.Length == 0) return null;
        return allTasks[Random.Range(0, allTasks.Length)];
    }
}