using System.Collections.Generic;
using UnityEngine;

public class statusFases : MonoBehaviour
{
    public class Fase {
        public int id;
        public bool passada;
        public bool status;

        public Fase()
        {
            this.passada = false;
            this.status = false;
        }
    }
    public static List<Fase> fases = new List<Fase>();
        void Start(){
        
        if (fases == null)
        {
            fases = new List<Fase>();
            for (int i = 0; i < 5; i++)
            {
                fases.Add(new Fase());
            }
        }

    }

    public static void ResetarFases()
    {
            foreach (Fase fase in fases)
            {
                fase.passada = false;
                fase.status = false;
            }
    }

    public static void passoufase(int fase, bool status)
    {
        if (fase >= 0 && fase < fases.Count)
        {
            fases[fase].passada = true;
            fases[fase].status = status;
        }
        else
        {
            Debug.LogError("Fase inválida: " + fase);
        }
    }

    public Fase GetStatusFase(int fase)
    {
        if (fase >= 0 && fase < fases.Count)
        {
            return fases[fase];
        }
        else
        {
            Debug.LogError("Fase inválida: " + fase);
            return null;
        }
    }
}
