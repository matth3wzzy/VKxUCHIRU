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

    [Header("Новое: Связь с Родителем")]
    public GameObject parentTaskPrefab; // Префаб с 2 кнопками
    public Transform parentContent;     // Контейнер (Content) на канвасе родителя

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

        Debug.Log("Клик по кнопке зафиксирован! Тип задания: " + (currentTask != null ? currentTask.taskType : "NULL"));
    
    if (isCompleted || currentTask == null) return;
        if (isCompleted || currentTask == null) return;

        // --- НОВАЯ ЛОГИКА ДЛЯ MANUAL (БЕЗ СЕРВЕРА) ---
        if (currentTask.taskType == "manual")
        {
            SendToParentLocal();
            return;
        }

        // --- СТАРАЯ ЛОГИКА AUTO (НИЧЕГО НЕ ТРОНУТО) ---
        if (answerInput == null) return; 

        string userAnswer = answerInput.text.Trim();
        if (string.IsNullOrEmpty(userAnswer)) return;

        if (userAnswer.ToLower() == currentTask.correctAnswer.ToLower())
            StartCoroutine(AddPointsRoutine());
        else
            ShowFeedback(false, "Неверно!");
    }

    // Метод для мгновенного создания префаба у родителя
    private void SendToParentLocal()
    {
        if (parentTaskPrefab == null || parentContent == null)
        {
            Debug.LogError("Не назначены ссылки на префаб или контейнер родителя!");
            return;
        }

        // 1. Создаем плашку на экране родителя
        GameObject go = Instantiate(parentTaskPrefab, parentContent);
        
        // 2. Настраиваем плашку через её скрипт
        ParentTaskItem item = go.GetComponent<ParentTaskItem>();
        if (item != null)
        {
            string msg = (answerInput != null && !string.IsNullOrEmpty(answerInput.text)) 
                         ? answerInput.text.Trim() 
                         : "Задание выполнено!";
            item.SetupLocal(this, msg);
        }

        // 3. Визуал кнопки ребенка (как в твоем AddPoin tsRoutine)
        isCompleted = true;
        ShowFeedback(true, "Отправлено родителю!");
        submitButton.interactable = false;

        Image btnImage = submitButton.GetComponent<Image>();
        if (btnImage != null) btnImage.color = Color.gray;

        Shadow btnShadow = submitButton.GetComponent<Shadow>();
        if (btnShadow != null) btnShadow.enabled = false;

        submitButton.enabled = false; 
    }

    // Метод для кнопки "Отказ" со стороны родителя
    public void ResetTask()
    {
        isCompleted = false;
        submitButton.interactable = true;
        submitButton.enabled = true;
        
        if (submitButton.TryGetComponent(out Image img)) img.color = Color.white; // Или твой стандартный цвет
        ShowFeedback(false, "Нужно переделать!");
    }

    // --- ТВОЙ ОРИГИНАЛЬНЫЙ AddPointsRoutine (БЕЗ ИЗМЕНЕНИЙ) ---
    IEnumerator AddPointsRoutine()
    {
        ShowFeedback(true, "Правильно!");
        submitButton.interactable = false;

        Image btnImage = submitButton.GetComponent<Image>();
        if (btnImage != null) btnImage.color = Color.gray;

        Shadow btnShadow = submitButton.GetComponent<Shadow>();
        if (btnShadow != null) btnShadow.enabled = false;

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
