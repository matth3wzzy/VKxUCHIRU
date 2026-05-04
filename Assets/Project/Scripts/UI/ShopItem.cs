using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour
{
    [Header("Настройки товара из БД")]
    public int itemId;      
    public int price;       

    [Header("UI элементы")]
    public Text statusText;     
    public Text feedbackText;   
    public Button buyButton;

    private int quantity = 0;
    private int lastCurrentPoints; 

    void Start()
    {
        buyButton.onClick.AddListener(TryPurchase);
        
        if (TaskService.Instance != null)
        {
            TaskService.Instance.OnScoreChanged += UpdateLocalScore;
            TaskService.Instance.LoadProgress((lvl, points, next) => {
                lastCurrentPoints = points;
            }, null);
        }
    }

    void OnDestroy()
    {
        if (TaskService.Instance != null)
            TaskService.Instance.OnScoreChanged -= UpdateLocalScore;
    }

    void UpdateLocalScore(int p)
    {
        lastCurrentPoints = p;
    }

    public void TryPurchase()
    {
        if (lastCurrentPoints < price)
        {
            statusText.text = "Не хватает очков!";
            statusText.color = Color.red;
            // Запускаем очистку даже для сообщения о нехватке очков
            StopAllCoroutines(); 
            StartCoroutine(ClearFeedbackRoutine());
            return;
        }

        buyButton.interactable = false;
        
        TaskService.Instance.BuyItem(itemId, (newScore) => {
            // УСПЕХ
            quantity++;
            statusText.text = $"Вы купили x{quantity}";
            statusText.color = Color.green;

            feedbackText.text = $"-{price} очков";
            feedbackText.color = Color.red;

            buyButton.interactable = true;
            StopAllCoroutines(); // Останавливаем старые таймеры, если они были
            StartCoroutine(ClearFeedbackRoutine());
        }, 
        (err) => {
            // ОШИБКА (временно выводим успех по твоему коду)
            Debug.LogError($"[Shop] Ошибка: {err}");
            statusText.text = "Товар куплен успешно";
            statusText.color = Color.green;
            
            buyButton.interactable = true;
            StopAllCoroutines();
            StartCoroutine(ClearFeedbackRoutine());
        });
    }

    // Общая корутина для очистки всех надписей через 5 секунд
    IEnumerator ClearFeedbackRoutine()
    {
        yield return new WaitForSeconds(5f);
        
        if (feedbackText != null) feedbackText.text = "";
        if (statusText != null) statusText.text = "";
    }
}
