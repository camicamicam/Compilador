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
    commit github
*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {
        private bool primeraProduccion = true;
        private bool hayEpsilon = false;
        private List<string> listaParentesis;
        private List<Producciones> listaProducciones;
        private int nivelIndentacion = 0;
        private string produccionR = "";
        private int contadorR = 0;

        public Lenguaje()
        {
            listaParentesis = new List<string>();
            listaProducciones = new List<Producciones>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            listaParentesis = new List<string>();
            listaProducciones = new List<Producciones>();
        }
        private string ObtenerIndentacion()
        {
            return new string(' ', nivelIndentacion * 4);
        }

        private void EscribirConIndentacion(string linea)
        {
            lenguajecs.WriteLine(ObtenerIndentacion() + linea);
        }

        private void AbrirLlave()
        {
            
            lenguajecs.WriteLine(ObtenerIndentacion()+"{");
            nivelIndentacion++;
        }
        private void CerrarLlave()
        {
            if(nivelIndentacion > 0)
                nivelIndentacion--;
            EscribirConIndentacion("}");   
            
            //lenguajecs.WriteLine("}");
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
            AbrirLlave();
            EscribirConIndentacion("public class Lenguaje : Sintaxis");
            AbrirLlave();
            EscribirConIndentacion("public Lenguaje()");
            AbrirLlave();
            CerrarLlave();
            EscribirConIndentacion("public Lenguaje(string nombre) : base(nombre)");
            AbrirLlave();
            CerrarLlave();
        }
        public void Genera()
        {
            match("namespace");
            match(":");
            esqueleto(Contenido);
            match(Tipos.SNT);
            match(";");

            int cTemp = caracter - Contenido.Length - 2;
            int lTemp = linea;
            registroP();
            //ImprimirProducciones();
            caracter = cTemp;
            linea = lTemp;
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
            nextToken();
            producciones();

            CerrarLlave();
            CerrarLlave();
        }
        private void registroP()
        {
            
            string NombreProduccion;
            NombreProduccion = Contenido;
            match(Tipos.SNT);
            match(Tipos.Flecha);
            string primerElemento = Contenido;
            Tipos tipoPE = buscarTipo(Contenido);
            listaProducciones.Add(new Producciones(NombreProduccion, primerElemento, tipoPE));
            while (Clasificacion != Tipos.FinProduccion)
            {
                nextToken();
            }
            match(Tipos.FinProduccion);
            if (Clasificacion == Tipos.SNT)
            {
                registroP();
            }
        }

        private void ImprimirProducciones()
        {
            foreach (var item in listaProducciones)
            {
                Console.WriteLine(item.getNombre() + " : " + item.getPrimerElemento()+ " "+ item.getPrimerClasificacion());
            }
        }

        private void producciones()
        {
            if (Clasificacion == Tipos.SNT)
            {
                if (primeraProduccion)
                {
                    EscribirConIndentacion("public void " + Contenido + "()");
                    primeraProduccion = false;
                }
                else
                {
                    EscribirConIndentacion("private void " + Contenido + "()");
                }
                AbrirLlave();

            }
            match(Tipos.SNT);
            match(Tipos.Flecha);

            conjuntoTokens();
            match(Tipos.FinProduccion);
            CerrarLlave();
            if (Clasificacion == Tipos.SNT)
            {
                producciones();
            }
        }

        private void conjuntoTokens()
        {
            if (Clasificacion == Tipos.SNT)
            {
                var p = listaProducciones.Find(p => p.getNombre() == Contenido);
                if (p != null)
                {
                    EscribirConIndentacion(Contenido + "();");
                    match(Tipos.SNT);
                }
                else
                {
                    throw new Error("No se declaro la produccion "+Contenido+" ", log, linea-1);
                }
                
            }
            else if (Clasificacion == Tipos.ST)
            {
                EscribirConIndentacion("match(\"" + Contenido + "\");");
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                EscribirConIndentacion("match(Tipos." + Contenido + ");");
                match(Tipos.Tipo);
            }
            else if (Clasificacion == Tipos.Izquierdo)
            {
                match(Tipos.Izquierdo);
                lenguajecs.Write(ObtenerIndentacion()+"if(");

                if (Clasificacion == Tipos.ST)
                {
                    lenguajecs.WriteLine("Contenido == \"" + Contenido + "\")");
                    AbrirLlave();
                    EscribirConIndentacion("match(\"" + Contenido + "\");");
                    match(Tipos.ST);
                }
                else if (Clasificacion == Tipos.Tipo)
                {
                    lenguajecs.WriteLine("Clasificacion == Tipos." + Contenido + ")");
                    AbrirLlave();
                    EscribirConIndentacion("match(Tipos." + Contenido + ");");
                    match(Tipos.Tipo);
                }
                else if (Clasificacion == Tipos.SNT)
                {
                    epsilonSNT(Contenido,"");
                    match(Tipos.SNT);
                }

                if (Clasificacion != Tipos.Derecho)
                {
                    listaElementos();
                }
                match(Tipos.Derecho);
                var v = listaParentesis.Find(v => v == "|");
                if (v == null)
                {
                    if (Clasificacion != Tipos.Epsilon)
                    {
                        throw new Error("Falta un epsilon al final del parentesis ", log, linea);
                    }
                    else
                    {
                        match(Tipos.Epsilon);
                    }
                }
                else
                {
                    if (Clasificacion == Tipos.Epsilon)
                    {
                        match(Tipos.Epsilon);
                        hayEpsilon = true;
                    }
                }
                produccionParentesis();
                CerrarLlave();
            }
            if (Clasificacion != Tipos.FinProduccion)
            {
                if (Clasificacion == Tipos.Flecha)
                {
                    throw new Error("Debe acabar una producción con un ; ",log,linea-1);
                }
                else
                {
                    conjuntoTokens();
                }
            }
        }

        private void listaElementos()
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
            else if (Clasificacion == Tipos.Or)
            {
                match(Tipos.Or);
            }
            if (Clasificacion != Tipos.Derecho)
            {
                listaElementos();
            }
        }

        private void epsilonSNT(string contenido, string pRecursivo)
        {
            if (contadorR == 0)
            {
                produccionR = contenido;
            }
            var p = listaProducciones.Find(p => p.getNombre() == contenido);
            if (p != null)
            {
                
                if (p.getPrimerClasificacion() == Tipos.ST)
                {
                    lenguajecs.WriteLine("Contenido == \"" + p.getPrimerElemento() + "\")");
                    AbrirLlave();
                    if (pRecursivo != "")
                    {
                        EscribirConIndentacion(produccionR+"();");
                        contadorR = 0;
                    }
                    else
                    {
                        EscribirConIndentacion(contenido+"();");
                    }
                }
                else if (p.getPrimerClasificacion() == Tipos.Tipo)
                {
                    lenguajecs.WriteLine("Clasificacion == Tipos." + p.getPrimerElemento() + ")");
                    AbrirLlave();
                    if (pRecursivo != "")
                    {
                        EscribirConIndentacion(produccionR+"();");
                        contadorR = 0;
                    }
                    else
                    {
                        EscribirConIndentacion(contenido+"();");
                    }
                }
                else if (p.getPrimerClasificacion() == Tipos.SNT)
                {
                    contadorR++;
                    epsilonSNT(p.getPrimerElemento(),contenido);
                }
                else if (p.getPrimerClasificacion() == Tipos.FinProduccion)
                {
                    throw new Error("No hay contenido en la produccion "+contenido,log);
                }       
            }
            else
            {
                throw new Error("No se declaro la produccion "+contenido+" ", log, linea);
            }
            
        }

        private Token.Tipos buscarTipo(string contenido)
        {
            Token.Tipos tipo;
            if (EsTipo(contenido))
            {
                tipo = Tipos.Tipo;
            }
            else if (contenido == "\\;")
            {
                tipo = Tipos.FinProduccion;
            }
            else if (contenido == "|")
            {
                tipo = Tipos.Or;
            }
            else if (char.IsLower(contenido[0]))
            {
                tipo = Tipos.ST;
            }
            else if(EsST(contenido))
            {
                tipo = Tipos.ST;
            }
            else
            {
                tipo = Tipos.SNT;
            }
            return tipo;
        }
        private bool EsTipo(string contenido)
        {
            switch (contenido)
            {
                case "Identificador":
                case "Numero":
                case "FinSentencia":
                case "OpTermino":
                case "OpFactor":
                case "OpLogico":
                case "OpRelacional":
                case "OpTernario":
                case "Asignacion":
                case "IncTermino":
                case "IncFactor":
                case "Cadena":
                case "Inicio":
                case "Fin":
                case "Caracter":
                case "TipoDato":
                case "Ciclo":
                case "Condicion": return true;
            }
            return false;

        }
        private bool EsST(string contenido)
        {
            switch (contenido)
            {
                case "!":
                case "#":
                case "$":
                case "%":
                case "&":
                case "*":
                case "+":
                case "-":
                case "=":
                case ";":
                case "/": 
                case "(":
                case ")": 
                case "?":
                case "¿":return true;
            }
            return false;

        }

        private void produccionParentesis()
        {
            bool anteOR = false;
            for (int i = 0; i < listaParentesis.Count; i++)
            {
                Token.Tipos tipoLista = buscarTipo(listaParentesis[i]);
                if (tipoLista == Tipos.SNT)
                {
                    if (anteOR)
                    {
                       epsilonSNT(listaParentesis[i],"");
                       anteOR = false;
                    }
                    else
                    {
                        var p = listaProducciones.Find(p => p.getNombre() == listaParentesis[i]);
                        if (p != null)
                        {
                            EscribirConIndentacion(listaParentesis[i] + "();"); 
                        }
                        else
                        {
                            throw new Error("No se declaro la produccion " + listaParentesis[i], log);
                        }
                    }
                }
                else if (tipoLista == Tipos.ST)
                {
                    if (anteOR)
                    {
                        lenguajecs.WriteLine("Contenido == \"" + listaParentesis[i] + "\")");
                        AbrirLlave();
                        anteOR = false;
                    }
                    EscribirConIndentacion("match(\"" + listaParentesis[i] + "\");");
                }
                else if (tipoLista == Tipos.Tipo)
                {
                    if (anteOR)
                    {
                        lenguajecs.WriteLine("Clasificacion == Tipos." + listaParentesis[i] + ")");
                        AbrirLlave();
                        anteOR = false;
                    }
                    EscribirConIndentacion("match(Tipos." + listaParentesis[i] + "\");");
                }
                else if (listaParentesis[i] == "|")
                {
                    CerrarLlave(); 
                    if (i == listaParentesis.LastIndexOf("|") )
                    {
                        if (hayEpsilon)
                        {
                            lenguajecs.Write(ObtenerIndentacion()+"else if(");
                            anteOR = true;
                        }
                        else
                        {
                            EscribirConIndentacion("else");
                            AbrirLlave();
                            anteOR = false;
                        }
                    }
                    else
                    {
                        lenguajecs.Write(ObtenerIndentacion()+"else if(");
                        anteOR = true;
                    }


                }

            }
            listaParentesis.Clear();

        }

    }

}