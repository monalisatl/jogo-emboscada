using UnityEngine;

namespace Fase_5.Respescagem_Scritps.Fase_4
{
    [CreateAssetMenu(fileName = "Enigma", menuName = "Scriptable Objects/Enigma")]
    public class EnigmaRepescagem : ScriptableObject
    {
        public string enigma;
        public string[] alternativas;
        public int idCorreto;
        [TextArea] public string explicacao;

    }
}
