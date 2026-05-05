using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Логика Singleton: чтобы музыка не начиналась заново при возврате в меню
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Объект не удалится при смене сцен
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликаты, если зашли в сцену снова
        }
    }
}
