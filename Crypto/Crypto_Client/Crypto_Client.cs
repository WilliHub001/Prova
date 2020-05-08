using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Cifratura
{
    class Program
    { 
        static void Main(string[] args)
        {
            const int PORT = 42069;

            Console.Write("Input da:\n1) File\n2) Input\nScegli: ");
            int c = int.Parse(Console.ReadLine());

            var count = 1;
            string text;
            switch(c) {
                case 1:
                    Console.Write("Percorso: ");
                    string path = Console.ReadLine();

                    StreamReader file = new StreamReader(path);

                    //Nel caso ci siano più righe nel file, verrà salvato in una variabile il numero di righe
                    count = File.ReadLines(path).Count();
                    text = file.ReadToEnd();
                break;
                case 2:
                    Console.Write("Inserire testo: ");
                    text = Console.ReadLine();
                break;
                default:
                    Console.Write("Scelta invalida");
                return;
            }

            Console.Write("Chiave: ");
            string key = Console.ReadLine();

            Console.Write("Server: ");
            string server = Console.ReadLine();

            //Apertura connesisone con il server
            TcpClient client = new TcpClient(server, PORT);
            StreamWriter uscita = new StreamWriter(client.GetStream());
            StreamReader entrata = new StreamReader(client.GetStream());

            //invio delle variabili al server
            uscita.WriteLine(key);
            uscita.WriteLine(count);
            uscita.WriteLine(text);
            uscita.Flush();

            //Ricezione del testo cifrato dal server
            text = entrata.ReadLine();

            Console.WriteLine("Cifrato: {0}", text);
            Console.WriteLine("Decifrato: {0}", decode(text, key));

            client.Close();
            Console.ReadLine();
        }

        static string decode(string input, string key)
        {
            //Tutto minuscolo
            input = input.ToLower();
            key = key.ToLower();

            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 'a' || input[i] > 'z') //Controllo che il carattere alla posizione i sia una lettera
                {
                    output += input[i];
                }
                else
                {
                    //Serve per spostare l'indice della chiave, per fare in modo che una volta raggiunta l'ultima lettera della chiave, si ricominci a cifrare dalla prima lettera
                    int j = i % key.Length;

                    //Indici dell'input e della chiave per la cifratura
                    int a = input[i] - 'a';
                    int b = key[j] - 'a';

                    //Decifra il messaggio
                    int c = (26 + a - b) % 26;

                    output += (char)(c + 'a');
                }
            }

            return output;
        }
    }
}
