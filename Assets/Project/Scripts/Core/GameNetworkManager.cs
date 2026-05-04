using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameNetworkManager : MonoBehaviour
{
    [Header("UI Менеджеры")]
public ProfileManager profileManager;  // ← Добавить в начало класса
    [Header("Поля ввода для ЛОГИНА (PC)")]
    public InputField Login_PC_Email;
    public InputField Login_PC_Password;
    
    [Header("Поля ввода для ЛОГИНА (Phone)")]
    public InputField Login_PHONE_Email;
    public InputField Login_PHONE_Password;
    
    [Header("Поля ввода для РЕГИСТРАЦИИ (PC)")]
    public InputField Reg_PC_Email;
    public InputField Reg_PC_Password;
    
    [Header("Поля ввода для РЕГИСТРАЦИИ (Phone)")]
    public InputField Reg_PHONE_Email;
    public InputField Reg_PHONE_Password;
    
    [Header("Общие поля")]
    public InputField parentRegParentPin;
    public Text taskText;
    public InputField answerInput;
    public InputField solutionInput;
    public InputField pinInput;
    public Text errorText;
    public Text scoreText;

    private string currentTaskId;
    private string currentTaskType;
    private bool isPhoneDevice;

    void Awake()
    {
        DetectPlatform();
    }

    void DetectPlatform()
{
    // ТО ЖЕ САМОЕ УСЛОВИЕ, ЧТО И В CanvasSorter
    bool isLandscape = Screen.width > Screen.height;
    isPhoneDevice = !isLandscape;  // Если портрет → телефон
    
    Debug.Log($"Platform: {(isPhoneDevice ? "Phone" : "PC")}, Screen: {Screen.width}x{Screen.height}, Landscape: {isLandscape}");
}

    // ================== МЕТОДЫ НАВИГАЦИИ (ДЛЯ КНОПОК) ==================
    
    // Переключение между панелями (ты управляешь через sortingOrder, 
    // но эти методы нужны для вызова из OnClick)
    
    public void NavLogin()
{
    Debug.Log("NavLogin() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavLoginPage();
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}
    
    public void NavRegister()
{
    Debug.Log("NavRegister() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavRegisterPage();
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}
    
    public void NavMainMenu()
    {
        Debug.Log("NavMainMenu() вызван");
        // Твоя логика переключения на главное меню
    }
    
    public void NavStudentPage()
{
    Debug.Log("NavStudentPage() вызван");
    
    // Находим CanvasSorter на сцене и вызываем переключение
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavStudentPage();
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}
    
    
    // Эти методы нужны для совместимости со старыми кнопками
    public void NavChildLogin() => NavLogin();
    public void NavChildRegister() => NavRegister();
    public void NavParentLogin() => NavLogin();
    public void NavParentRegister() => NavRegister();
    public void NavShopPage()  // ← БЕЗ опечаток
{
    Debug.Log("NavShopPage() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavShopPage();  // ← вызываем метод из CanvasSorter
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}
    public void NavAchievePage()  // ← БЕЗ опечаток
{
    Debug.Log("NavACHIEVEPage() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavAchievePage();  // ← вызываем метод из CanvasSorter
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}
    public void NavParentCheckTask() => Debug.Log("Parent check task");

    // ================== ЛОГИКА ВХОДА ==================
    
public void OnChildLogin()
{
    Debug.Log("=== OnChildLogin() НАЧАЛО ===");
    
    // 1. Проверка платформы
    Debug.Log($"1. isPhoneDevice = {isPhoneDevice}");
    
    // 2. Получение полей
    InputField emailField = isPhoneDevice ? Login_PHONE_Email : Login_PC_Email;
    InputField passwordField = isPhoneDevice ? Login_PHONE_Password : Login_PC_Password;
    
    Debug.Log($"2. emailField = {(emailField == null ? "NULL" : emailField.name)}");
    Debug.Log($"3. passwordField = {(passwordField == null ? "NULL" : passwordField.name)}");
    
    // 3. Проверка полей
    if (emailField == null)
    {
        Debug.LogError("❌ emailField = NULL!");
        if (errorText) errorText.text = "Ошибка: поле логина не найдено";
        return;
    }
    
    if (passwordField == null)
    {
        Debug.LogError("❌ passwordField = NULL!");
        if (errorText) errorText.text = "Ошибка: поле пароля не найдено";
        return;
    }
    
    // 4. Проверка UserSession
    Debug.Log($"4. UserSession.Instance = {(UserSession.Instance == null ? "NULL" : "ЕСТЬ")}");
    
    if (UserSession.Instance == null)
    {
        Debug.LogError("❌ UserSession.Instance = NULL! Добавьте UserSession на сцену!");
        if (errorText) errorText.text = "Ошибка: сервер не доступен";
        return;
    }
    
    // 5. Получение текста из полей
    string loginInput = emailField.text.Trim();  // может быть email или никнейм
    string pass = passwordField.text;
    
    Debug.Log($"5. loginInput = '{loginInput}', длина = {loginInput.Length}");
    Debug.Log($"6. password длина = {pass.Length}");
    
    // 6. Проверка на пустоту
    if (string.IsNullOrEmpty(loginInput))
    {
        Debug.LogWarning("⚠️ Логин пустой!");
        if (errorText) errorText.text = "Введите логин";
        return;
    }
    
    if (string.IsNullOrEmpty(pass))
    {
        Debug.LogWarning("⚠️ Пароль пустой!");
        if (errorText) errorText.text = "Введите пароль";
        return;
    }
    
    // 7. Сохраняем введённый email (если это email)
    if (loginInput.Contains("@"))
    {
        UserSession.Instance.UserEmail = loginInput;
        Debug.Log($"📧 Сохранён email: {UserSession.Instance.UserEmail}");
    }
    
    // 8. Отправка запроса (используем введённое значение как никнейм)
    Debug.Log($"✅ Отправка запроса на сервер: nickname='{loginInput}', password='{new string('*', pass.Length)}'");
    
    UserSession.Instance.LoginChildByNickname(loginInput, pass,
        () => { 
            Debug.Log("🎉 УСПЕХ! Вход выполнен!");
            if (profileManager != null)
            profileManager.UpdateProfileDisplay();
            

            // Если при входе использовали email, но сервер вернул никнейм
            if (string.IsNullOrEmpty(UserSession.Instance.Nickname) == false)
            {
                Debug.Log($"👤 Никнейм с сервера: {UserSession.Instance.Nickname}");
            }
            
            
            
            NavStudentPage();
            StartCoroutine(StartGameAfterLogin());
        },
        err => { 
            Debug.LogError($"❌ Ошибка от сервера: {err}");
            if (errorText) errorText.text = $"Ошибка: {err}";
        }
    );
    
    Debug.Log("=== OnChildLogin() КОНЕЦ (запрос отправлен) ===");
}
    
    // ================== ЛОГИКА РЕГИСТРАЦИИ ==================
    
    public void OnRegister()
{
    Debug.Log("OnRegister() вызван");
    
    InputField emailField = isPhoneDevice ? Reg_PHONE_Email : Reg_PC_Email;
    InputField passwordField = isPhoneDevice ? Reg_PHONE_Password : Reg_PC_Password;
    
    if (emailField == null || passwordField == null || UserSession.Instance == null)
    {
        Debug.LogError("Не назначены поля регистрации или отсутствует UserSession");
        if (errorText) errorText.text = "Ошибка: не найдены поля регистрации";
        return;
    }
    
    string email = emailField.text.Trim();
    string pass = passwordField.text;
    string pin = (parentRegParentPin != null) ? parentRegParentPin.text.Trim() : "";
    if (string.IsNullOrEmpty(pin)) pin = "1234";
    
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
    {
        if (errorText) errorText.text = "Заполните все поля";
        return;
    }
    
    Debug.Log($"Регистрация: {email}, платформа: {(isPhoneDevice ? "Phone" : "PC")}");
    
    // Сохраняем email ДО вызова регистрации
    UserSession.Instance.UserEmail = email;
    
    UserSession.Instance.RegisterChild(email, pass, pin,
        () => { 
            Debug.Log("Регистрация успешна!");
            if (profileManager != null)
            profileManager.UpdateProfileDisplay();
            NavStudentPage();
            StartCoroutine(StartGameAfterLogin());
        },
        err => { 
            Debug.LogError("Ошибка регистрации: " + err);
            if (errorText) errorText.text = "Ошибка регистрации: " + err; 
        }
    );
}

    // ================== РОДИТЕЛЬСКИЕ МЕТОДЫ ==================
    
    public void OnParentLogin()
    {
        InputField emailField = isPhoneDevice ? Login_PHONE_Email : Login_PC_Email;
        InputField pinField = isPhoneDevice ? Login_PHONE_Password : Login_PC_Password;
        
        if (emailField == null || pinField == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля входа для родителя");
            return;
        }
        
        string nickname = emailField.text.Trim();
        string pin = pinField.text.Trim();
        
        UserSession.Instance.LoginParentByNickname(nickname, pin,
            () => { LoadParentData(); },
            err => { if (errorText) errorText.text = "Ошибка входа или PIN"; }
        );
    }
    
    public void OnParentRegister()
    {
        InputField emailField = isPhoneDevice ? Reg_PHONE_Email : Reg_PC_Email;
        InputField passwordField = isPhoneDevice ? Reg_PHONE_Password : Reg_PC_Password;
        
        if (emailField == null || passwordField == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля регистрации родителя");
            return;
        }
        
        string email = emailField.text.Trim();
        string pass = passwordField.text;
        string pin = (parentRegParentPin != null) ? parentRegParentPin.text.Trim() : "";
        if (string.IsNullOrEmpty(pin)) pin = "1234";
        
        UserSession.Instance.RegisterChild(email, pass, pin,
            () => { LoadParentData(); },
            err => { if (errorText) errorText.text = "Ошибка регистрации: " + err; }
        );
    }

    void LoadParentData()
    {
        if (ParentService.Instance == null) return;
        ParentService.Instance.GetDashboard((nick, score, tasks) => { }, err => { });
        ParentService.Instance.GetPendingTasks(list => { }, err => { });
    }

    IEnumerator StartGameAfterLogin()
    {
        if (TaskService.Instance != null)
        {
            TaskService.Instance.DailyCheckin((streak, reward, max) => { }, err => { });
            TaskService.Instance.LoadProgress((level, cur, next) => { }, err => { });
        }
        GiveNextTask();
        yield break;
    }

    void GiveNextTask()
    {
        if (TaskManager.Instance == null) return;
        TaskData t = TaskManager.Instance.GetRandomTask();
        if (t != null)
        {
            currentTaskId = t.taskId;
            currentTaskType = t.taskType;
            if (taskText) taskText.text = t.taskDescription;
        }
    }

    public void OnSubmitAutoAnswer()
    {
        if (answerInput == null || pinInput == null || TaskService.Instance == null)
        {
            Debug.LogError("Не назначены поля answerInput или pinInput");
            return;
        }
        string answer = answerInput.text.Trim();
        string pin = pinInput.text.Trim();
        if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(pin))
        {
            if (errorText) errorText.text = "Введите ответ и PIN";
            return;
        }

        TaskService.Instance.CompleteTask(currentTaskId, currentTaskType, pin, answer, null,
            (completed, newScore) =>
            {
                if (scoreText) scoreText.text = "Счёт: " + newScore;
                GiveNextTask();
            },
            err => { if (errorText) errorText.text = "Ошибка: " + err; }
        );
    }

    public void OnSubmitManualSolution()
    {
        if (solutionInput == null || pinInput == null || TaskService.Instance == null)
        {
            Debug.LogError("Не назначены поля solutionInput или pinInput");
            return;
        }
        string solution = solutionInput.text.Trim();
        string pin = pinInput.text.Trim();
        if (string.IsNullOrEmpty(solution) || string.IsNullOrEmpty(pin))
        {
            if (errorText) errorText.text = "Введите решение и PIN";
            return;
        }

        TaskService.Instance.CompleteTask(currentTaskId, currentTaskType, pin, null, solution,
            (completed, newScore) =>
            {
                if (scoreText) scoreText.text = "Счёт: " + newScore;
                GiveNextTask();
            },
            err => { if (errorText) errorText.text = "Ошибка: " + err; }
        );
    }

    
public void NavTaskPage()  // ← БЕЗ опечаток
{
    Debug.Log("NavTaskPage() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavTaskPage();  // ← вызываем метод из CanvasSorter
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}

    public void NavProfilePage()
{
    Debug.Log("NavProfilePage() вызван");
    
    CanvasSorter canvasSorter = FindObjectOfType<CanvasSorter>();
    
    if (canvasSorter != null)
    {
        canvasSorter.NavProfilePage();
    }
    else
    {
        Debug.LogError("CanvasSorter не найден на сцене!");
    }
}

    public void Logout()
    {
        if (UserSession.Instance != null)
            UserSession.Instance.Logout();
        Debug.Log("Выход выполнен");
        NavMainMenu();
    }
}