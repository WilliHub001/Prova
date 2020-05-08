using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    
    class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 42069;
            //Apertura connessione con client
            TcpListener server = new TcpListener(IPAddress.Any, PORT);
            server.Start();

            while (true)
            {

                TcpClient client = server.AcceptTcpClient();

                StreamReader entrata = new StreamReader(client.GetStream());
                StreamWriter uscita = new StreamWriter(client.GetStream());
                //Ricezione variabili da client
                string key = entrata.ReadLine();
                string countS = entrata.ReadLine();
                int count = Int32.Parse(countS);
                string text = null;
                if (count >1) //Se il file selezionato per il testo contiene più di una riga, verrà ricevuta una riga alla volta e salvata in text
                {
                    for(int i=0; i < count; i++)
                    {
                        string textapp = entrata.ReadLine();
                        text = text + " " + textapp;
                    }
                }
                else
                    text = entrata.ReadLine();

                //Codifica del testo
                text = encode(text, key);
                Console.WriteLine("Testo cifrato: {0}",text);
                //Invio del testo cifrato al client
                uscita.WriteLine(text);
                uscita.Flush();

                client.Close();
            }
        }

        static string encode(string input, string key)
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

                    //Cifratura del messaggio
                    int c = (a + b) % 26;

                    output += (char)(c + 'a');
                }
            }

            return output;
        }

    }
}
