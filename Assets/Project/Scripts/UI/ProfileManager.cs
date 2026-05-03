using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public Text nicknameText;
    public Text emailText;

    void Start()
    {
        Debug.Log("=== ProfileManager Start ===");
        
        if (UserSession.Instance == null)
        {
            Debug.LogError("❌ UserSession.Instance = NULL!");
            return;
        }
        
        // НЕ пытаемся показать профиль сейчас - он пустой
        // Просто ждём обновления извне
        Debug.Log("ProfileManager готов, ждём обновления после логина");
    }
    
    // ЭТОТ МЕТОД БУДЕТ ВЫЗЫВАТЬСЯ ПОСЛЕ ЛОГИНА
    public void UpdateProfileDisplay()
    {
        if (UserSession.Instance == null)
        {
            Debug.LogError("UserSession.Instance = NULL!");
            return;
        }
        
        Debug.Log($"📝 Обновляем профиль: Nickname = '{UserSession.Instance.Nickname}'");
        Debug.Log($"📧 Обновляем профиль: Email = '{UserSession.Instance.UserEmail}'");
        
        if (nicknameText != null)
        {
            if (!string.IsNullOrEmpty(UserSession.Instance.Nickname))
                nicknameText.text = UserSession.Instance.Nickname;
            else
                nicknameText.text = "Не указан";
        }
        
        if (emailText != null)
        {
            if (!string.IsNullOrEmpty(UserSession.Instance.UserEmail))
                emailText.text = UserSession.Instance.UserEmail;
            else
                emailText.text = "Не указан";
        }
    }
}