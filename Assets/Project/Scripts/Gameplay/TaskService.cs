using System;
using System.Collections;
using UnityEngine;

public class TaskService : MonoBehaviour
{
    public static TaskService Instance { get; private set; }

    // Событие для мгновенного обновления всех счетчиков очков в игре
    public event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- 1. ЛОГИКА МАГАЗИНА (НОВАЯ) ---

    // Покупка товара по его ID из базы (POST /api/shop/buy)
    public void BuyItem(int itemId, Action<int> onSuccess, Action<string> onError)
    {
        StartCoroutine(BuyItemCoroutine(itemId, onSuccess, onError));
    }

    IEnumerator BuyItemCoroutine(int itemId, Action<int> onSuccess, Action<string> onError)
{
    string json = "{\"item_id\":" + itemId + "}"; 
    
    yield return ApiClient.Post("/api/shop/buy", json, UserSession.Instance.AuthToken,
        success => {
            var resp = JsonUtility.FromJson<TaskCompleteResponse>(success);
            OnScoreChanged?.Invoke(resp.score); // Если в JSON есть score, обновим всё
            onSuccess?.Invoke(resp.score);
        },
        error => {
            // Больше никаких LoadProgress внутри! Просто сообщаем об ошибке.
            onError?.Invoke(error);
        }
    );
}



    // Получение списка товаров (GET /api/shop/items)
    public void GetShopItems(Action<ShopItemsList> onSuccess, Action<string> onError)
    {
        StartCoroutine(ApiClient.Get("/api/shop/items", UserSession.Instance.AuthToken,
            success => onSuccess?.Invoke(JsonUtility.FromJson<ShopItemsList>(success)),
            onError));
    }

    // --- 2. ПРОГРЕСС И ЗАДАНИЯ (СТАРОЕ) ---

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

        // Этот метод нужен для GameNetworkManager, чтобы исправить ошибку CS1061
    public void CompleteTask(string taskId, string taskType, string parentPin, string answer, string solutionText,
                             Action<bool, int> onDone, Action<string> onError)
    {
        // Создаем объект данных для отправки на сервер
        var payload = new TaskCompletePayload
        {
            task_id = taskId,
            task_type = taskType,
            answer = answer,
            parent_pin = parentPin
        };

        string json = JsonUtility.ToJson(payload);

        StartCoroutine(ApiClient.Post("/api/task/complete", json, UserSession.Instance.AuthToken,
            success => {
                var resp = JsonUtility.FromJson<TaskCompleteResponse>(success);
                OnScoreChanged?.Invoke(resp.score); // Обновляем очки в UI
                onDone?.Invoke(true, resp.score);
            },
            onError
        ));
    }


    public void AddPointsSimple(int amount, Action<int> onDone, Action<string> onError)
    {
        StartCoroutine(CompleteTaskCoroutine("1", amount.ToString(), (success, newScore) => {
            onDone?.Invoke(newScore);
        }, onError));
    }

    IEnumerator CompleteTaskCoroutine(string taskId, string answer, Action<bool, int> onDone, Action<string> onError)
    {
        var payload = new TaskCompletePayload
        {
            task_id = taskId, 
            task_type = "auto", 
            answer = answer,
            parent_pin = "1234" 
        };
        
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/task/complete", json, UserSession.Instance.AuthToken,
            success => {
                var resp = JsonUtility.FromJson<TaskCompleteResponse>(success);
                OnScoreChanged?.Invoke(resp.score);
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
public class TaskCompletePayload {
    public string task_id;
    public string task_type;
    public string answer;
    public string parent_pin;
}

[System.Serializable]
public class TaskCompleteResponse {
    public string status;
    public int score;
}

// Новые классы для магазина
[System.Serializable]
public class ShopItemInfo {
    public int id;
    public string name;
    public int price;
}

[System.Serializable]
public class ShopItemsList {
    public ShopItemInfo[] items;
}
