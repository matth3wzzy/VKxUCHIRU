using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText; 

    void Start() // Используем Start вместо OnEnable
{
    if (TaskService.Instance != null)
    {
        TaskService.Instance.OnScoreChanged += UpdateScoreText;
        TaskService.Instance.LoadProgress(null, null);
    }
    else 
    {
        // Если попали сюда — значит TaskService еще не создался
        Debug.LogError("Критическая ошибка: TaskService еще не существует!");
    }
}

    void OnDisable()
    {
        // ОБЯЗАТЕЛЬНО отписываемся при выключении, чтобы не было ошибок «MissingReference»
        if (TaskService.Instance != null)
        {
            TaskService.Instance.OnScoreChanged -= UpdateScoreText;
        }
    }

    // Этот метод будет вызываться автоматически самим TaskService
    void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Очки: {newScore}";
            Debug.Log($"[ScoreDisplay] Текст обновлен: {newScore}");
        }
    }

    IEnumerator InitialLoadRoutine()
    {
        // Ждем кадр, чтобы Singleton TaskService успел проинициализироваться
        yield return null; 

        if (TaskService.Instance != null)
        {
            // Просто просим загрузить прогресс. 
            // Колбэки тут можно не писать, так как сработает наше событие OnScoreChanged
            TaskService.Instance.LoadProgress(null, null);
        }
        else
        {
            Debug.LogError("[ScoreDisplay] TaskService не найден!");
        }
    }
}
