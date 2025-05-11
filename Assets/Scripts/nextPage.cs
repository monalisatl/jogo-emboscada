using UnityEngine;

public class NextPage : MonoBehaviour
{
    public void NextPageButton()
    {
        if (MainManager.main != null)
            MainManager.main.ProximoCanvas();
    }

    public void PreviousPageButton()
    {
        if (MainManager.main != null)
            MainManager.main.CanvasAnterior();
    }

    public void GoToPageButton(int id)
    {
        if (MainManager.main != null)
            MainManager.main.IrParaCanvas(id);
    }
}