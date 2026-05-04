using System;
using System.Collections;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    public string AuthToken { get; private set; }
    public string ChildId { get; private set; }
    public string Nickname { get; private set; }
    public string UserEmail { get; set; }
    public string Role { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);
    public string parent_pin; // Добавь эту строку там, где лежат Nickname и AuthToken


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterChild(string email, string password, string parentPin, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterCoroutine(email, password, parentPin, onSuccess, onError));
    }

    IEnumerator RegisterCoroutine(string email, string password, string parentPin, Action onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(parentPin)) parentPin = "1234";
        var payload = new ChildRegisterPayload
        {
            nickname = null,
            email = email,
            password = password,
            parent_pin = parentPin
        };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/register", json, null,
            success =>
            {
                TokenResponse resp = JsonUtility.FromJson<TokenResponse>(success);
                AuthToken = resp.access_token;
                ChildId = resp.child_id;
                Nickname = resp.nickname;
                Role = "child";
                onSuccess?.Invoke();
            },
            error =>
            {
                onError?.Invoke(error);
            });
    }

    public void LoginChild(string email, string password, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginChildCoroutine(email, password, onSuccess, onError));
    }

    IEnumerator LoginChildCoroutine(string email, string password, Action onSuccess, Action<string> onError)
    {
        var payload = new LoginPayload { email = email, password = password };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/login/json", json, null,
            success =>
            {
                TokenResponse resp = JsonUtility.FromJson<TokenResponse>(success);
                AuthToken = resp.access_token;
                ChildId = resp.child_id;
                Nickname = resp.nickname;
                Role = "child";
                onSuccess?.Invoke();
            },
            error =>
            {
                onError?.Invoke(error);
            });
    }

    // �����: ���� ������ �� ��������
    public void LoginChildByNickname(string nickname, string password, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginChildByNicknameCoroutine(nickname, password, onSuccess, onError));
    }

    IEnumerator LoginChildByNicknameCoroutine(string nickname, string password, Action onSuccess, Action<string> onError)
    {
        var payload = new NicknameLoginPayload { nickname = nickname, password = password };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/login/nickname", json, null,
            success =>
            {
                TokenResponse resp = JsonUtility.FromJson<TokenResponse>(success);
                AuthToken = resp.access_token;
                ChildId = resp.child_id;
                Nickname = resp.nickname;
                Role = "child";
                onSuccess?.Invoke();
            },
            error => onError?.Invoke(error));
    }

    public void LoginParent(string email, string pin, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginParentCoroutine(email, pin, onSuccess, onError));
    }

    IEnumerator LoginParentCoroutine(string email, string pin, Action onSuccess, Action<string> onError)
    {
        var form = new WWWForm();
        form.AddField("username", email);
        form.AddField("pin", pin);
        yield return ApiClient.PostForm("/api/parent/login", form, null,
            success =>
            {
                TokenResponse resp = JsonUtility.FromJson<TokenResponse>(success);
                AuthToken = resp.access_token;
                ChildId = resp.child_id;
                Nickname = resp.nickname;
                Role = "parent";
                onSuccess?.Invoke();
            },
            error =>
            {
                onError?.Invoke(error);
            });
    }

    // �����: ���� �������� �� �������� ������
    public void LoginParentByNickname(string nickname, string pin, Action onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginParentByNicknameCoroutine(nickname, pin, onSuccess, onError));
    }

    IEnumerator LoginParentByNicknameCoroutine(string nickname, string pin, Action onSuccess, Action<string> onError)
    {
        var payload = new ParentNicknameLoginPayload { nickname = nickname, pin = pin };
        string json = JsonUtility.ToJson(payload);
        yield return ApiClient.Post("/api/parent/login/nickname", json, null,
            success =>
            {
                TokenResponse resp = JsonUtility.FromJson<TokenResponse>(success);
                AuthToken = resp.access_token;
                ChildId = resp.child_id;
                Nickname = resp.nickname;
                Role = "parent";
                onSuccess?.Invoke();
            },
            error => onError?.Invoke(error));
    }

    public void GetProfile(Action<string, string> onSuccess, Action<string> onError)
{
    StartCoroutine(GetProfileCoroutine(onSuccess, onError));
}

IEnumerator GetProfileCoroutine(Action<string, string> onSuccess, Action<string> onError)
{
    yield return ApiClient.Get("/api/profile", AuthToken,
        success =>
        {
            ProfileResponse resp = JsonUtility.FromJson<ProfileResponse>(success);
            UserEmail = resp.email;  // сохраняем email
            Nickname = resp.nickname;
            onSuccess?.Invoke(resp.email, resp.nickname);
        },
        error => onError?.Invoke(error));
}

[System.Serializable]
public class ProfileResponse
{
    public string email;
    public string nickname;
}

    public void Logout()
    {
        AuthToken = null;
        ChildId = null;
        Nickname = null;
        Role = null;
    }
}

// ---- ������ JSON ----
[System.Serializable]
public class ChildRegisterPayload
{
    public string nickname;
    public string email;
    public string password;
    public string parent_pin;
}

[System.Serializable]
public class LoginPayload
{
    public string email;
    public string password;
}

[System.Serializable]
public class NicknameLoginPayload
{
    public string nickname;
    public string password;
}

[System.Serializable]
public class ParentNicknameLoginPayload
{
    public string nickname;
    public string pin;
}

[System.Serializable]
public class TokenResponse
{
    public string access_token;
    public string token_type;
    public string child_id;
    public string nickname;
}