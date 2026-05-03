using UnityEngine;

public class CanvasSorter : MonoBehaviour
{
    public Canvas mainPagePC;
    public Canvas mainPagePhone;

    void Start()
    {
        // Сбрасываем всё в 0 (на случай, если в инспекторе стоят другие значения)
        mainPagePC.sortingOrder = 0;
        mainPagePhone.sortingOrder = 0;

        // Проверяем соотношение сторон или ширину. 
        // Обычно компьютер — это когда ширина больше высоты (landscape)
        if (Screen.width > Screen.height)
        {
            mainPagePC.sortingOrder = 1;
        }
        else
        {
            mainPagePhone.sortingOrder = 1;
        }
    }
}
