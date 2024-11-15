using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje L = new Lenguaje("gramatica.txt"))
                {
                    /*
                    while (!L.finArchivo())
                    {
                        L.nextToken();
                    }
                    */
                  
                    L.Genera();
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}