using System;
using UnityEngine;

public class EmboscadaController : MonoBehaviour
{    
    [Serializable]
    public class GameData
    {
        public int currentLevel;
        public Personagem player;
        public string playerName;
        public bool[] niveisganhos;
        public Classificacao classificacao;
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
