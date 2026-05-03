using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameNetworkManager : MonoBehaviour
{
    [Header("Панели (перетащи сюда нужные GameObject)")]
    public GameObject mainpage;
    public GameObject student_page;
    public GameObject parent_page;
    public GameObject task_page;
    public GameObject shop_page;
    public GameObject achieve_page;
    public GameObject parent_checktask;
    public GameObject Child_Logn;
    public GameObject Child_reg;
    public GameObject Parent_login;
    public GameObject Parent_reg;

    [Header("Поля ввода и тексты")]
    public InputField childLoginEmail;      // теперь используется как поле для никнейма
    public InputField childLoginPassword;
    public InputField childRegEmail;
    public InputField childRegPassword;
    public InputField childRegParentPin;
    public InputField parentLoginEmail;     // для никнейма
    public InputField parentLoginPin;
    public InputField parentRegEmail;
    public InputField parentRegPassword;
    public InputField parentRegPin;
    public Text taskText;
    public InputField answerInput;
    public InputField solutionInput;
    public InputField pinInput;
    public Text errorText;
    public Text scoreText;

    private string currentTaskId;
    private string currentTaskType;

    void Awake()
    {
        AdjustCanvasScaler();
        if (mainpage != null)
            ShowOnly(mainpage);
        else
            Debug.LogError("Главная панель (mainpage) не назначена в инспекторе!");
    }

    void AdjustCanvasScaler()
    {
        CanvasScaler scaler = GetComponentInParent<CanvasScaler>() ?? FindObjectOfType<CanvasScaler>();
        if (scaler == null) return;

        float aspect = (float)Screen.width / Screen.height;

        if (aspect >= 1.7f) scaler.matchWidthOrHeight = 0.0f;
        else if (aspect >= 1.2f) scaler.matchWidthOrHeight = 0.3f;
        else if (aspect >= 0.9f) scaler.matchWidthOrHeight = 0.5f;
        else scaler.matchWidthOrHeight = 1.0f;
    }

    void ShowOnly(GameObject panel)
    {
        mainpage?.SetActive(false);
        student_page?.SetActive(false);
        parent_page?.SetActive(false);
        task_page?.SetActive(false);
        shop_page?.SetActive(false);
        achieve_page?.SetActive(false);
        parent_checktask?.SetActive(false);
        Child_Logn?.SetActive(false);
        Child_reg?.SetActive(false);
        Parent_login?.SetActive(false);
        Parent_reg?.SetActive(false);
        if (panel != null) panel.SetActive(true);
    }

    // ================== НАВИГАЦИЯ ==================
    public void NavMainMenu() => ShowOnly(mainpage);
    public void NavStudentPage() => ShowOnly(student_page);
    public void NavParentPage() => ShowOnly(parent_page);
    public void NavTaskPage() => ShowOnly(task_page);
    public void NavShopPage() => ShowOnly(shop_page);
    public void NavAchievePage() => ShowOnly(achieve_page);
    public void NavParentCheckTask() => ShowOnly(parent_checktask);
    public void NavChildLogin() => ShowOnly(Child_Logn);
    public void NavChildRegister() => ShowOnly(Child_reg);
    public void NavParentLogin() => ShowOnly(Parent_login);
    public void NavParentRegister() => ShowOnly(Parent_reg);

    // ================== СЕРВЕРНЫЕ МЕТОДЫ ==================

    public void OnChildLogin()
    {
        if (childLoginEmail == null || childLoginPassword == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля для входа ребёнка (никнейм/пароль) или отсутствует UserSession");
            return;
        }
        string nickname = childLoginEmail.text.Trim();
        string pass = childLoginPassword.text;

        UserSession.Instance.LoginChildByNickname(nickname, pass,
            () => { NavTaskPage(); StartCoroutine(StartGameAfterLogin()); },
            err => { if (errorText) errorText.text = "Неверный никнейм или пароль"; }
        );
    }

    public void OnRegister()
    {
        if (childRegEmail == null || childRegPassword == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля для регистрации (email/пароль) или отсутствует UserSession");
            return;
        }
        string email = childRegEmail.text.Trim();
        string pass = childRegPassword.text;
        string pin = (childRegParentPin != null) ? childRegParentPin.text.Trim() : "";
        if (string.IsNullOrEmpty(pin)) pin = "1234";

        UserSession.Instance.RegisterChild(email, pass, pin,
            () => { NavTaskPage(); StartCoroutine(StartGameAfterLogin()); },
            err => { if (errorText) errorText.text = "Ошибка регистрации: " + err; }
        );
    }

    public void OnParentLogin()
    {
        if (parentLoginEmail == null || parentLoginPin == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля для входа родителя (никнейм/PIN) или отсутствует UserSession");
            return;
        }
        string nickname = parentLoginEmail.text.Trim();
        string pin = parentLoginPin.text.Trim();

        UserSession.Instance.LoginParentByNickname(nickname, pin,
            () => { NavParentCheckTask(); LoadParentData(); },
            err => { if (errorText) errorText.text = "Неверный никнейм или PIN"; }
        );
    }

    public void OnParentRegister()
    {
        if (parentRegEmail == null || parentRegPassword == null || UserSession.Instance == null)
        {
            Debug.LogError("Не назначены поля для регистрации родителя (email/пароль) или отсутствует UserSession");
            return;
        }
        string email = parentRegEmail.text.Trim();
        string pass = parentRegPassword.text;
        string pin = (parentRegPin != null) ? parentRegPin.text.Trim() : "";
        if (string.IsNullOrEmpty(pin)) pin = "1234";

        UserSession.Instance.RegisterChild(email, pass, pin,
            () => { NavParentCheckTask(); LoadParentData(); },
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
            Debug.LogError("Не назначены поля для ответа или отсутствует TaskService");
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
                if (scoreText) scoreText.text = "Очки: " + newScore;
                GiveNextTask();
            },
            err => { if (errorText) errorText.text = "Ошибка: " + err; }
        );
    }

    public void OnSubmitManualSolution()
    {
        if (solutionInput == null || pinInput == null || TaskService.Instance == null)
        {
            Debug.LogError("Не назначены поля для решения или отсутствует TaskService");
            return;
        }
        string solution = solutionInput.text.Trim();
        string pin = pinInput.text.Trim();
        if (string.IsNullOrEmpty(solution) || string.IsNullOrEmpty(pin))
        {
            if (errorText) errorText.text = "Опишите решение и введите PIN";
            return;
        }

        TaskService.Instance.CompleteTask(currentTaskId, currentTaskType, pin, null, solution,
            (completed, newScore) =>
            {
                if (scoreText) scoreText.text = "Очки: " + newScore;
                GiveNextTask();
            },
            err => { if (errorText) errorText.text = "Ошибка: " + err; }
        );
    }

    public void Logout()
    {
        if (UserSession.Instance != null)
            UserSession.Instance.Logout();
        NavMainMenu();
    }
}