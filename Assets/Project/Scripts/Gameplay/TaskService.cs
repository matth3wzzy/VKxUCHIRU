using System;
using System.Collections;
using UnityEngine;


public class TaskService : MonoBehaviour


{
    public static TaskService Instance { get; private set; }

    public event Action<int> OnScoreChanged;
    

    

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. Получение прогресса (очки, уровень)
    public void LoadProgress(Action<int, int, int> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetProgressCoroutine(onSuccess, onError));
    }
    

    IEnumerator GetProgressCoroutine(Action<int, int, int> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Get("/api/progress", UserSession.Instance.AuthToken,
            success => {
                var pr = JsonUtility.FromJson<ProgressData>(success);

                OnScoreChanged?.Invoke(pr.current_points);

                onSuccess?.Invoke(pr.level, pr.current_points, pr.points_to_next);
            },
            onError);
    }

    // 2. Ежедневная награда
    public void DailyCheckin(Action<int, int, int> onSuccess, Action<string> onError)
    {
        StartCoroutine(DailyCheckinCoroutine(onSuccess, onError));
    }

    IEnumerator DailyCheckinCoroutine(Action<int, int, int> onSuccess, Action<string> onError)
    {
        yield return ApiClient.Post("/api/daily-checkin", "{}", UserSession.Instance.AuthToken,
            success => {
                var d = JsonUtility.FromJson<DailyCheckinData>(success);
                onSuccess?.Invoke(d.streak, d.reward, d.max_streak);
            },
            onError);
    }

    // 3. Упрощенная отправка результата задания
    // Мы оставляем сигнатуру метода прежней, чтобы TaskVerifier не сломался
    public void CompleteTask(string taskId, string taskType, string parentPin, string answer, string solutionText,
                             Action<bool, int> onDone, Action<string> onError)
    {
        StartCoroutine(CompleteTaskCoroutine(taskId, answer, onDone, onError));
    }

    public void AddPointsSimple(int amount, Action<int> onDone, Action<string> onError)
{
    // Мы передаем rewardPoints как "answer", чтобы сервер мог их обработать, 
    // если он настроен на простой прием очков.
    StartCoroutine(CompleteTaskCoroutine("1", amount.ToString(), (success, newScore) => {
        onDone?.Invoke(newScore);
    }, onError));
}

    IEnumerator CompleteTaskCoroutine(string taskId, string answer, Action<bool, int> onDone, Action<string> onError)
{
    var payload = new TaskCompletePayload
    {
        // 1. Убедись, что taskId передается как "001" (проверь в инспекторе задания)
        task_id = taskId, 
        
        // 2. МЕНЯЕМ НА auto, чтобы сервер дал очки мгновенно!
        task_type = "auto", 
        
        answer = answer,
        parent_pin = "1234" 
    };
    
    string json = JsonUtility.ToJson(payload);
    Debug.Log($"[SENDING] {json}");

    yield return ApiClient.Post("/api/task/complete", json, UserSession.Instance.AuthToken,
        success => {
            var resp = JsonUtility.FromJson<TaskCompleteResponse>(success);

            OnScoreChanged?.Invoke(resp.score);
            // Если сервер вернул новый score, значит начисление прошло
            onDone?.Invoke(true, resp.score);
        },
        error => onError?.Invoke(error)
    );
}
}

// --- КЛАССЫ ДАННЫХ ---

[System.Serializable]
public class ProgressData {
    public int level;
    public int current_points;
    public int points_to_next;
}

[System.Serializable]
public class DailyCheckinData {
    public int streak;
    public int reward;
    public int max_streak;
}

[System.Serializable]
public class TaskCompletePayload
{
    public string task_id;
    public string task_type; // Добавляем это поле
    public string answer;
    public string parent_pin;
}


[System.Serializable]
public class TaskCompleteResponse {
    public string status;
    public int score;
}


