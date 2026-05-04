using UnityEngine;
using UnityEngine.UI;

public class ParentTaskItem : MonoBehaviour
{
    public Text infoText;
    private TaskVerifier childTask; // Ссылка на скрипт ребенка

    public void SetupLocal(TaskVerifier task, string desc)
    {
        childTask = task;
        infoText.text = desc;
    }

    public void OnApprove() // На кнопку "Подтвердить"
    {
        // Начисляем очки через твой сервис (чтобы в базу улетело)
        TaskService.Instance.AddPointsSimple(50, (newScore) => {
            Destroy(gameObject); // Убираем плашку
        }, null);
    }

    public void OnReject() // На кнопку "Доделать"
    {
        childTask.ResetTask(); // Возвращаем кнопку ребенку в рабочее состояние
        Destroy(gameObject);   // Убираем плашку
    }
}

