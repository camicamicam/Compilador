using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/*
    1. Solo la primera producción es publica, el resto es privada (manejar bandera);
    2. Implementar la cerradura Epsilon (si es epsilon solo epsilon, si no tiene ninguno error de que no hay ?)
    3. Implementar el operador OR(si es or solo or)
    4. Indentar el código (control de tabuladores o espacios)
    Solo el ultimo token puede ser SNT
    commit github
*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {
        private bool primeraProduccion = true;
        private List<String> listaParentesis;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }

        private void esqueleto(string nspace)
        {
            lenguajecs.WriteLine("using System;");
            lenguajecs.WriteLine("using System.Collections.Generic;");
            lenguajecs.WriteLine("using System.Linq;");
            lenguajecs.WriteLine("using System.Net.Http.Headers;");
            lenguajecs.WriteLine("using System.Reflection.Metadata.Ecma335;");
            lenguajecs.WriteLine("using System.Runtime.InteropServices;");
            lenguajecs.WriteLine("using System.Threading.Tasks;");
            lenguajecs.WriteLine("\nnamespace " + nspace);
            lenguajecs.WriteLine("{");
            lenguajecs.WriteLine("    public class Lenguaje : Sintaxis");
            lenguajecs.WriteLine("    {");
            lenguajecs.WriteLine("        public Lenguaje()");
            lenguajecs.WriteLine("        {");
            lenguajecs.WriteLine("        }");
            lenguajecs.WriteLine("        public Lenguaje(string nombre) : base(nombre)");
            lenguajecs.WriteLine("        {");
            lenguajecs.WriteLine("        }");
        }
        public void Genera()
        {
            match("namespace");
            match(":");
            esqueleto(Contenido);
            match(Tipos.SNT);
            match(";");
            producciones();

            lenguajecs.WriteLine("      }");
            lenguajecs.WriteLine("}");
        }

        private void producciones()
        {
            if (Clasificacion == Tipos.SNT)
            {
                if (primeraProduccion)
                {
                    lenguajecs.WriteLine("        public void " + Contenido + "()");
                    primeraProduccion = false;
                }
                else
                {
                    lenguajecs.WriteLine("        private void " + Contenido + "()");
                }
                lenguajecs.WriteLine("        {");
            }
            match(Tipos.SNT);
            match(Tipos.Flecha);
            conjuntoTokens();
            match(Tipos.FinProduccion);
            lenguajecs.WriteLine("        }");
            if (Clasificacion == Tipos.SNT)
            {
                producciones();
            }
        }

        private void conjuntoTokens()
        {
            if (Clasificacion == Tipos.SNT)
            {
                lenguajecs.WriteLine("              " + Contenido + "();");
                match(Tipos.SNT);
            }
            else if (Clasificacion == Tipos.ST)
            {
                lenguajecs.WriteLine("              match(\"" + Contenido + "\");");
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                lenguajecs.WriteLine("              match(Tipos." + Contenido + "\");");
                match(Tipos.Tipo);
            }
            else if (Clasificacion == Tipos.Izquierdo)
            {
                match(Tipos.Izquierdo);
                lenguajecs.Write("              if(");

                if (Clasificacion == Tipos.ST)
                {
                    lenguajecs.WriteLine("Contenido == \"" + Contenido + "\")");
                    lenguajecs.WriteLine("              {");
                    lenguajecs.WriteLine("                  match(\"" + Contenido + "\");");
                    match(Tipos.ST);
                }
                else if (Clasificacion == Tipos.Tipo)
                {
                    lenguajecs.WriteLine("Clasificacion == Tipos." + Contenido + "\")");
                    lenguajecs.WriteLine("              {");
                    lenguajecs.WriteLine("                  match(Tipos." + Contenido + "\");");
                    match(Tipos.Tipo);
                }
                if (Clasificacion != Tipos.Derecho)
                {
                    listaproduccion();
                }
                    match(Tipos.Derecho);
                    if (Clasificacion == Tipos.Epsilon)
                    {
                        match(Tipos.Epsilon);
                        Epsilonproduccion();
                    }
                    lenguajecs.WriteLine("              }");
            }
            if (Clasificacion != Tipos.FinProduccion)
            {
                conjuntoTokens();
            }
        }

        private void listaproduccion()
        {
            listaParentesis.Add(Contenido);
            match(Contenido);
            if (Clasificacion != Tipos.Derecho)
            {
                listaproduccion();
            }
        }

        private void Epsilonproduccion()
        {
            
            if (Clasificacion == Tipos.SNT)
            {
                lenguajecs.WriteLine("              " + Contenido + "();");
                match(Tipos.SNT);
            }
            else if (Clasificacion == Tipos.ST)
            {
                lenguajecs.WriteLine("              match(\"" + Contenido + "\");");
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                lenguajecs.WriteLine("              match(Tipos." + Contenido + "\");");
                match(Tipos.Tipo);
            }
        }
        /*private void produccionOr(){
            lenguajecs.WriteLine("              }");
            match(Tipos.Or);
            lenguajecs.Write("              else if(");
            if (Clasificacion == Tipos.ST)
                {
                    lenguajecs.WriteLine("Contenido == \"" + Contenido + "\")");
                    lenguajecs.WriteLine("              {");
                    lenguajecs.WriteLine("                  match(\"" + Contenido + "\");");
                    match(Tipos.ST);
                }
                else if (Clasificacion == Tipos.Tipo)
                {
                    lenguajecs.WriteLine("Clasificacion == Tipos." + Contenido + ")");
                    lenguajecs.WriteLine("              {");
                    lenguajecs.WriteLine("                  match(Tipos." + Contenido + ");");
                    match(Tipos.Tipo);
                }
                if (Clasificacion == Tipos.Or)
                {
                    produccionOr();
                }
        }*/
    }

}