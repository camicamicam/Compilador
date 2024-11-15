using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compilador
{
    public class Token
    {
        public enum Tipos
        {
            ST,
            SNT,
            Flecha,
            FinProduccion,
            Epsilon,
            Or,
            Derecho,
            Izquierdo,
            Tipo
        };
        private string _contenido;
        private Tipos _clasificacion;
        public string Contenido
        {
            get => _contenido;
            set => _contenido = value;
        }
        public Tipos Clasificacion
        {
            get => _clasificacion;
            set => _clasificacion = value;
        }
        public Token()
        {
            _contenido = "";
        }
        
    }
}