using UnityEngine;

public class nextPage : MonoBehaviour
{
    public void NextPage()
    {
        if(mainManager.main != null)
        {
            mainManager.main.ProximoCanvas();
        }
    }

    public void PreviousPage()
    {
        if(mainManager.main != null)
        {
            mainManager.main.CanvasAnterior();
        }
    }

    public void GoToPage(int id)
    {
        if(mainManager.main != null)
        {
            mainManager.main.InstanciarCanva(id);
        }
    }

}
