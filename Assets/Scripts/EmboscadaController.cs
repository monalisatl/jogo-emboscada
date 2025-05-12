using System;
using UnityEngine;

public class EmboscadaController : MonoBehaviour
{
    [Serializable]
    public class GameData
    {
        public int currentLevel = 1;
        public int selectedCharacterId = 0;
        public string playerName = "";
        public bool[] niveisganhos = new bool[5];
        public Classificacao classificacao = Classificacao.Amador;
    }

    public enum Classificacao
    {
        Amador,
        Estagiário,
        Júnior,
        Sênior
    }

    public static GameData gameData;
}