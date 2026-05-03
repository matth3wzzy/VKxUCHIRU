using UnityEngine;

public class CanvasSorter : MonoBehaviour
{
    // ТВОИ РЕАЛЬНЫЕ КАНВАСЫ
    public Canvas NEW_login_PC;      // ← добавить
    public Canvas NEW_login_PHONE;   // ← добавить
    public Canvas NEW_reg_PC;        // ← добавить (если есть)
    public Canvas NEW_reg_PHONE;     // ← добавить (если есть)
    public Canvas studentPagePC;     // ← страница ученика ПК
    public Canvas studentPagePhone;  // ← страница ученика Телефон
    
    private Canvas currentActiveCanvas;
    private bool isPhoneDevice;

    void Start()
    {
        isPhoneDevice = !(Screen.width > Screen.height);
        
        // Обнуляем ВСЕ канвасы
        SetAllOrder(0);
        
        // Показываем страницу логина в зависимости от платформы
        if (isPhoneDevice)
            ShowCanvas(NEW_login_PHONE);
        else
            ShowCanvas(NEW_login_PC);
    }
    
    public void ShowCanvas(Canvas canvasToShow)
    {
        if (canvasToShow == null)
        {
            Debug.LogError($"Canvas {canvasToShow?.name} не привязан!");
            return;
        }
        
        // Обнуляем текущий активный канвас
        if (currentActiveCanvas != null)
        {
            Debug.Log($"Обнуляем старый: {currentActiveCanvas.name}");
            currentActiveCanvas.sortingOrder = 0;
        }
        
        // Показываем новый
        canvasToShow.sortingOrder = 1;
        currentActiveCanvas = canvasToShow;
        
        Debug.Log($"Активен: {canvasToShow.name} (sortingOrder = 1)");
    }
    
    public void NavStudentPage()
{
    Debug.Log("Переход на страницу ученика");
    
    // Скрываем ВСЕ панели авторизации
    HideAllAuthPanels();
    
    // Показываем страницу ученика
    if (isPhoneDevice)
        ShowCanvas(studentPagePhone);
    else
        ShowCanvas(studentPagePC);
}
    
    public void NavLoginPage()
    {
        Debug.Log("Переход на страницу логина");
        
        if (isPhoneDevice)
            ShowCanvas(NEW_login_PHONE);
        else
            ShowCanvas(NEW_login_PC);
    }
    public void HideAllAuthPanels()
{
    Debug.Log("Скрываем ВСЕ панели авторизации");
    
    if (NEW_login_PC != null) NEW_login_PC.sortingOrder = 0;
    if (NEW_login_PHONE != null) NEW_login_PHONE.sortingOrder = 0;
    if (NEW_reg_PC != null) NEW_reg_PC.sortingOrder = 0;
    if (NEW_reg_PHONE != null) NEW_reg_PHONE.sortingOrder = 0;
}
    public void NavRegisterPage()
{
    Debug.Log("Переход на страницу регистрации");
    
    if (isPhoneDevice)
        ShowCanvas(NEW_reg_PHONE);
    else
        ShowCanvas(NEW_reg_PC);
}

    
    private void SetAllOrder(int order)
    {
        if (NEW_login_PC != null) NEW_login_PC.sortingOrder = order;
        if (NEW_login_PHONE != null) NEW_login_PHONE.sortingOrder = order;
        if (NEW_reg_PC != null) NEW_reg_PC.sortingOrder = order;
        if (NEW_reg_PHONE != null) NEW_reg_PHONE.sortingOrder = order;
        if (studentPagePC != null) studentPagePC.sortingOrder = order;
        if (studentPagePhone != null) studentPagePhone.sortingOrder = order;
        
        currentActiveCanvas = null;
    }
}