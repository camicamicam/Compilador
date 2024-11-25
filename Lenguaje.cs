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
        private List<string> listaParentesis;
        public Lenguaje()
        {
            listaParentesis = new List<string>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            listaParentesis = new List<string>();
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

                    if(Clasificacion!=Tipos.Epsilon){
                        throw new Error("Falta un epsilon al final en la linea ",log,linea);
                    }
                    else{
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
            if (Clasificacion == Tipos.SNT)
            {
                match(Tipos.SNT);
            }
            else if (Clasificacion == Tipos.ST)
            {
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                match(Tipos.Tipo);
            }
            if (Clasificacion != Tipos.Derecho)
            {
                listaproduccion();
            }
        }

        private Token.Tipos buscarTipo(string contenido)
        {
            Token.Tipos tipo;
            if (EsTipo(contenido))
            {
                tipo = Tipos.Tipo;
            }
            else if (char.IsUpper(contenido[0]) )
            {
                tipo = Tipos.SNT;
            }
            else
            {
                tipo = Tipos.ST;
            }
            return tipo;
        }
        private bool EsTipo(string contenido)
        {
            switch(contenido)
            {
                case "Identificador":
                case "Numero":
                case "FinSentencia":
                case "OpTermino" :
                case "OpFactor" :
                case "OpLogico" :
                case "OpRelacional" :
                case "OpTernario" :
                case "Asignacion" :
                case "IncTermino" :
                case "IncFactor" :
                case "Cadena" :
                case "Inicio" :
                case "Fin" :
                case "Caracter" :
                case "TipoDato" :
                case "Ciclo": 
                case "Condicion" : return true;
            }
            return false;
            
        } 

        private void Epsilonproduccion()
        {

            for (int i = 0; i < listaParentesis.Count; i++)
            {

                Token.Tipos tipoLista = buscarTipo(listaParentesis[i]);
                if (tipoLista == Tipos.SNT)
                {
                    lenguajecs.WriteLine("                  " + listaParentesis[i] + "();");
                }
                else if (tipoLista == Tipos.ST)
                {
                    lenguajecs.WriteLine("                  match(\"" + listaParentesis[i] + "\");");
                }
                else if (tipoLista == Tipos.Tipo)
                {
                    lenguajecs.WriteLine("                  match(Tipos." + listaParentesis[i] + "\");");
                }

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