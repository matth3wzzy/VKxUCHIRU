using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentDashboard : MonoBehaviour
{
    public GameObject taskPrefab;
    public Transform container; // Сюда падают префабы

    void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        // Если контейнер пуст, ничего не делаем
        if (container == null) return;

        // 1. УМНАЯ ОЧИСТКА
        // Идем с конца списка до 4-го элемента (наши HEAD)
        for (int i = container.childCount - 1; i >= 4; i--) 
        {
            Transform child = container.GetChild(i);
            
            // ПРОВЕРКА: Если на объекте НЕТ скрипта ParentTaskItem,
            // значит это какой-то старый мусор или дубликат, удаляем его.
            // Если скрипт ЕСТЬ — это задание от ребенка, его НЕ ТРОГАЕМ.
            if (child.GetComponent<ParentTaskItem>() == null)
            {
                Destroy(child.gameObject);
            }
        }

        // 2. ЗАГРУЗКА ИЗ БАЗЫ (если нужно подтянуть то, чего нет на экране)

    }

    
}
