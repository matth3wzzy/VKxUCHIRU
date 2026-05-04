using UnityEngine;
using UnityEngine.UI;

public class PinCodeHandler : MonoBehaviour
{
    public InputField pinInputField; // Сюда перетащи поле ввода
    public Text errorText;           // Текст для вывода ошибки "Неверный ПИН"
    public GameObject parentCanvas;  // Канвас родителя, который откроем
    public GameObject currentCanvas; // Текущий канвас ПИН-кода, который закроем
    [Header("Добавь страницу профиля сюда")]
public GameObject studentProfilePage; // Ссылка на страницу ребенка

    public void CheckPin()
{
    // Очищаем от пробелов и лишних символов
    string enteredPin = pinInputField.text.Trim();

    // ВЫВОДИМ В КОНСОЛЬ ДЛЯ ПРОВЕРКИ
    Debug.Log($"[DEBUG] Введено: '{enteredPin}', Длина: {enteredPin.Length}");
    
    // Прямая проверка на "1234" (в кавычках, как строку)
    if (enteredPin == "1234")
    {
        Debug.Log("✅ ПИН совпал!");
        OpenParentPanel();
    }
    else
    {
        Debug.Log($"❌ ПИН '{enteredPin}' не равен '1234'");
        if (errorText != null) errorText.text = "Неверный ПИН!";
        pinInputField.text = ""; 
    }
}

    private void OpenParentPanel()
{
    if (parentCanvas == null) return;

    // 1. Включаем страницу родителя
    parentCanvas.SetActive(true);

    // 2. Выводим её в самый низ иерархии (в UI это значит "поверх всех")
    parentCanvas.transform.SetAsLastSibling();

    // 3. Если на объекте есть компонент Canvas, поднимаем его приоритет
    Canvas canvas = parentCanvas.GetComponent<Canvas>();
    if (canvas != null)
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 2; // Очень высокое число, чтобы быть выше профиля
    }

    // 4. Выключаем только окно ПИН-кода
    if (currentCanvas != null) currentCanvas.SetActive(false);

    // 5. Обновляем данные
    ParentDashboard dashboard = parentCanvas.GetComponentInChildren<ParentDashboard>(true);
    if (dashboard != null) dashboard.RefreshList();

    Debug.Log("✅ Переход завершен. Родитель должен быть поверх всех.");
}

public void OpenPinPanel()
{
    if (currentCanvas != null)
    {
        currentCanvas.SetActive(true);
        currentCanvas.transform.SetAsLastSibling(); // Чтобы вылез поверх профиля
        if (pinInputField != null) pinInputField.text = ""; // Очищаем старый ввод
        if (errorText != null) errorText.text = ""; // Убираем старую ошибку
    }
}

public void ShowPinPanel()
{
    if (currentCanvas == null) return;

    // 1. Включаем сам объект, если он был выключен
    currentCanvas.SetActive(true);

    // 2. Выводим его на самый передний план по иерархии
    currentCanvas.transform.SetAsLastSibling();

    // 3. Устанавливаем Sorting Order ПИН-кода выше всех остальных
    Canvas canvas = currentCanvas.GetComponent<Canvas>();
    if (canvas != null)
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 50; // Ставим 50 (чуть меньше родителя, но выше профиля)
    }

    // 4. Очищаем старые данные
    if (pinInputField != null) pinInputField.text = "";
    if (errorText != null) errorText.text = "";
}
public void EmergencyClose()
{
    // Сбрасываем сортировку, чтобы не перекрывать лобби
    Canvas canvas = parentCanvas.GetComponent<Canvas>();
    if (canvas != null) canvas.sortingOrder = 0; 
    
    parentCanvas.SetActive(false);
    currentCanvas.SetActive(false);
}
}
