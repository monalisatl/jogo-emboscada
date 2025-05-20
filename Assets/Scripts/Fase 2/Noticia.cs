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

        public DateTime FormaterData
        {
            get
            {
                if (string.IsNullOrEmpty(data) || data.Length < 10)
                {
                    Debug.LogError("Data não possui o tamanho mínimo esperado: " + data);
                    return  new DateTime();
                }

                var date = data[^10..];
                if (DateTime.TryParseExact(date, "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out var dt))
                {
                    return dt;
                }
                else
                {
                    Debug.LogError("Data não está no formato esperado: " + date);
                    return new DateTime();
                }
            }

        }
    }

    [Serializable]
    public class BotaoOpcao
    {
        public string texto;
        public bool isCorreto;
    }
