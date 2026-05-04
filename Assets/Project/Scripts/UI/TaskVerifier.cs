using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskVerifier : MonoBehaviour
{
    [Header("UI элементы")]
    public Text taskText;
    public InputField answerInput;
    public Text feedbackText;
    public Button submitButton;
    public Text scoreText;

    [Header("Настройки задания")]
    public TaskData currentTask; 
    
    [Header("Настройки визуализации")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private bool isCompleted = false;

    void Start()
    {
        if (currentTask != null && taskText != null)
            taskText.text = currentTask.taskDescription;

        submitButton.onClick.AddListener(CheckAnswer);
        UpdateScoreDisplay();
    }

    public void CheckAnswer()
{
    if (isCompleted || currentTask == null) return;

    string userAnswer = answerInput.text.Trim();
    if (string.IsNullOrEmpty(userAnswer)) return;

    // Если задание АВТОМАТИЧЕСКОЕ (как было раньше)
    if (currentTask.taskType == "auto") 
    {
        if (userAnswer.ToLower() == currentTask.correctAnswer.ToLower())
            StartCoroutine(AddPointsRoutine());
        else
            ShowFeedback(false, "Неверно!");
    }
    // Если задание РУЧНОЕ (новое)
    else if (currentTask.taskType == "manual")
    {
        StartCoroutine(SendToParentRoutine(userAnswer));
    }
}

    IEnumerator SendToParentRoutine(string solution)
{
    ShowFeedback(true, "Отправка на проверку...");
    submitButton.interactable = false;

    Image btnImage = submitButton.GetComponent<Image>();
    Shadow btnShadow = submitButton.GetComponent<Shadow>();

    // Используем task_id (или то имя, которое ты дал в TaskData)
    TaskService.Instance.SendManualTaskForVerification(currentTask.taskId, solution, () => {
        isCompleted = true;
        ShowFeedback(true, "Отправлено родителю!");
        
        if (btnImage != null) btnImage.color = Color.gray;
        if (btnShadow != null) btnShadow.enabled = false;
        submitButton.enabled = false; 
    }, (err) => {
        Debug.LogError("Ошибка: " + err);
        ShowFeedback(false, "Ошибка сети");
        // Возвращаем кнопку в рабочее состояние, если не улетело
        submitButton.interactable = true; 
    });

    yield break;
}

    IEnumerator AddPointsRoutine()
{
    ShowFeedback(true, "Правильно!");
    submitButton.interactable = false;

    

    // --- НОВОЕ: Визуальное изменение кнопки ---
    // 1. Делаем кнопку серой через компонент Image
    Image btnImage = submitButton.GetComponent<Image>();
    if (btnImage != null) btnImage.color = Color.gray;

    // 2. Убираем тень (Shadow), если она есть
    Shadow btnShadow = submitButton.GetComponent<Shadow>();
    if (btnShadow != null) btnShadow.enabled = false;

    // 3. Удаляем или выключаем компонент Button, чтобы она больше не реагировала
    // Лучше просто выключить, чтобы не ломать логику ссылок
    submitButton.enabled = false; 

    TaskService.Instance.AddPointsSimple(currentTask.rewardPoints, (newScore) => {
        isCompleted = true;
        if (scoreText != null) scoreText.text = $"Очки: {newScore}";
        ShowFeedback(true, $"+{currentTask.rewardPoints} очков!");
    }, (err) => {
        Debug.LogError("Ошибка: " + err);
        submitButton.interactable = false;
        submitButton.enabled = false;
        if (btnImage != null) btnImage.color = Color.gray;
        if (btnShadow != null) btnShadow.enabled = false;
    });
    yield break;
}


    void ShowFeedback(bool isCorrect, string message)
    {
        if (feedbackText == null) return;
        feedbackText.text = message;
        feedbackText.color = isCorrect ? correctColor : wrongColor;
    }

    void UpdateScoreDisplay()
    {
        TaskService.Instance.LoadProgress((lvl, points, next) => {
            if (scoreText != null) scoreText.text = $"Очки: {points}";
        }, null);
    }
}
