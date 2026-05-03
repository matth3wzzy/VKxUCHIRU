using System;
using System.Collections;
using UnityEngine;

public class TaskService : MonoBehaviour
{
    public static TaskService Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadProgress(Action<int, int, int> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetProgressCoroutine(onSuccess, onError));
    }

    IEnumerator GetProgressCoroutine(Action<int, int, int> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Get("/api/progress", UserSession.Instance.AuthToken,
            success =>
            {
                var pr = JsonUtility.FromJson<ProgressData>(success);
                onSuccess?.Invoke(pr.level, pr.current_points, pr.points_to_next);
            },
            error => onError?.Invoke(error));
    }

    public void DailyCheckin(Action<int, int, int> onSuccess, Action<string> onError)
    {
        StartCoroutine(DailyCheckinCoroutine(onSuccess, onError));
    }

    IEnumerator DailyCheckinCoroutine(Action<int, int, int> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Post("/api/daily-checkin", "{}", UserSession.Instance.AuthToken,
            success =>
            {
                var d = JsonUtility.FromJson<DailyCheckinData>(success);
                onSuccess?.Invoke(d.streak, d.reward, d.max_streak);
            },
            error => onError?.Invoke(error));
    }

    public void CompleteTask(string taskId, string taskType, string parentPin,
        string answer, string solutionText,
        Action<bool, int> onDone, Action<string> onError)
    {
        StartCoroutine(CompleteTaskCoroutine(taskId, taskType, parentPin, answer, solutionText, onDone, onError));
    }

    IEnumerator CompleteTaskCoroutine(string taskId, string taskType, string parentPin,
        string answer, string solutionText,
        Action<bool, int> onDone, Action<string> onError)
    {
        var payload = new TaskCompletePayload
        {
            task_id = taskId,
            task_type = taskType,
            parent_pin = parentPin,
            answer = answer,
            solution_text = solutionText,
            solution_image = null
        };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/task/complete", json, UserSession.Instance.AuthToken,
            success =>
            {
                var resp = JsonUtility.FromJson<TaskCompleteResponse>(success);
                bool completed = resp.status == "completed";
                onDone?.Invoke(completed, resp.score);
            },
            error => onError?.Invoke(error));
    }
}

[System.Serializable]
public class ProgressData
{
    public int level;
    public int current_points;
    public int points_to_next;
    public int total_points;
}

[System.Serializable]
public class DailyCheckinData
{
    public int streak;
    public int reward;
    public int max_streak;
}

[System.Serializable]
public class TaskCompletePayload
{
    public string task_id;
    public string task_type;
    public string parent_pin;
    public string answer;
    public string solution_text;
    public string solution_image;
}

[System.Serializable]
public class TaskCompleteResponse
{
    public string status;
    public int score;
    public string message;
}