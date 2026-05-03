using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentService : MonoBehaviour
{
    public static ParentService Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GetPendingTasks(Action<List<PendingTask>> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetPendingCoroutine(onSuccess, onError));
    }

    IEnumerator GetPendingCoroutine(Action<List<PendingTask>> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Get("/api/parent/pending-tasks", UserSession.Instance.AuthToken,
            success =>
            {
                var wrapper = JsonUtility.FromJson<PendingTaskList>(success);
                onSuccess?.Invoke(new List<PendingTask>(wrapper.tasks));
            },
            error => onError?.Invoke(error));
    }

    public void ReviewTask(int taskId, string action, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(ReviewCoroutine(taskId, action, onSuccess, onError));
    }

    IEnumerator ReviewCoroutine(int taskId, string action, Action onSuccess, Action<string> onError)
    {
        var payload = new ReviewPayload { task_id = taskId, action = action };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/parent/review", json, UserSession.Instance.AuthToken,
            success => onSuccess?.Invoke(),
            error => onError?.Invoke(error));
    }

    public void GetDashboard(Action<string, int, int> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetDashboardCoroutine(onSuccess, onError));
    }

    IEnumerator GetDashboardCoroutine(Action<string, int, int> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Get("/api/parent/dashboard", UserSession.Instance.AuthToken,
            success =>
            {
                var d = JsonUtility.FromJson<ParentDashboardData>(success);
                onSuccess?.Invoke(d.nickname, d.score, d.tasks_done);
            },
            error => onError?.Invoke(error));
    }
}

[System.Serializable]
public class ParentDashboardData
{
    public string nickname;
    public int score;
    public int tasks_done;
    public string[] tasks_list;
}

[System.Serializable]
public class PendingTask
{
    public int id;
    public string task_id;
    public string solution_text;
    public string solution_image;
}

[System.Serializable]
public class PendingTaskList
{
    public PendingTask[] tasks;
}

[System.Serializable]
public class ReviewPayload
{
    public int task_id;
    public string action;
}