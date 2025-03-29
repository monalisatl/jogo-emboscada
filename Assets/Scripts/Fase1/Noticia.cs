using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Noticia", menuName = "Scriptable Objects/Noticia")]
public class Noticia : ScriptableObject
{
    public string titulo;
    public string data;
    public string conteudo;
    public string linkFonte;
    public List<BotaoOpcao> opcoesResposta;
}

[Serializable]
public class BotaoOpcao {
    public string texto;
    public bool isCorreto;
}
