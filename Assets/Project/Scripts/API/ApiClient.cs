using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiClient
{
    public static string BaseUrl =
#if UNITY_EDITOR
        "https://gamejem-production.up.railway.app";   // ���� Railway-������
#else
        "";
#endif

    public static IEnumerator Get(string path, string token, Action<string> onSuccess, Action<string> onError)
    {
        using (var req = UnityWebRequest.Get(BaseUrl + path))
        {
            if (!string.IsNullOrEmpty(token))
                req.SetRequestHeader("Authorization", "Bearer " + token);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }
    }

    public static IEnumerator Post(string path, string jsonBody, string token, Action<string> onSuccess, Action<string> onError)
    {
        using (var req = new UnityWebRequest(BaseUrl + path, "POST"))
        {
            byte[] body = Encoding.UTF8.GetBytes(jsonBody ?? "{}");
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(token))
                req.SetRequestHeader("Authorization", "Bearer " + token);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
            
                Debug.LogError("СЕРВЕР ПИШЕТ: " + req.downloadHandler.text); // ЭТО РЕШИТ ВСЁ
                onError?.Invoke(req.error);
        }
    }

    public static IEnumerator PostForm(string path, WWWForm form, string token, Action<string> onSuccess, Action<string> onError)
    {
        using (var req = UnityWebRequest.Post(BaseUrl + path, form))
        {
            if (!string.IsNullOrEmpty(token))
                req.SetRequestHeader("Authorization", "Bearer " + token);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }
    }
}