using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compilador
{
    public class Producciones
    {

        private string Nombre;
        private string primerElemento;
        private Token.Tipos primerClasificacion;

        public Producciones(string Nombre, string primerElemento, Token.Tipos primerClasificacion)
        {
            this.Nombre = Nombre;
            this.primerElemento = primerElemento;
            this.primerClasificacion = primerClasificacion;
        }

        public string getNombre()
        {
            return this.Nombre;
        }
        public string getPrimerElemento()
        {
            return this.primerElemento;
        }

        public Token.Tipos getPrimerClasificacion()
        {
            return this.primerClasificacion;
        }

    }
}