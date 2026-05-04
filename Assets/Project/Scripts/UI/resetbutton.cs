using UnityEngine;
using UnityEngine.UI;

public class ButtonReset : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (button != null)
        {
            button.interactable = true;
            // На всякий случай сбрасываем состояние аниматора, 
            // если кнопка "зависла" в цвете Pressed или Disabled
            button.targetGraphic.canvasRenderer.SetAlpha(1f); 
            Debug.Log($"[CLEANUP] Кнопка {gameObject.name} принудительно активирована");
        }
    }
}
