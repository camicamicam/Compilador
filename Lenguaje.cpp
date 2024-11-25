using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Lenguaje
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {
        }
        public Lenguaje(string nombre) : base(nombre)
        {
        }
        public void Programa()
        {
              A();
              match("b");
              C();
        }
        private void A()
        {
              C();
              if(Clasificacion == Tipos.Numero")
              {
                  match(Tipos.Numero");
