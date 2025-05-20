using UnityEngine;

[CreateAssetMenu(fileName = "Enigma", menuName = "Scriptable Objects/Enigma")]
public class Enigma : ScriptableObject
{
    public string enigma;
    public string[] alternativas;
    public int idCorreto;
    [TextArea] public string explicacao;

}
