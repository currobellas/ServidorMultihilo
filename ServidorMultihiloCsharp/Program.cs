using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ServidorMultihiloCsharp
{
    public class Servidor
    {
        private const int PUERTO = 31416;
        public bool Funcionando { set; get; }

        public void InicioServidor()
        {
            Console.WriteLine("Servidor");
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, PUERTO);
            Socket socketEscucha = new Socket(AddressFamily.InterNetwork, 
                                              SocketType.Stream, 
                                              ProtocolType.Tcp);
            socketEscucha.Bind(ie); //OJO, si el puerto de escucha 
                                    //está ocupado lanza excepción.
            socketEscucha.Listen(5);
            Console.WriteLine("A la espera");

            while (Funcionando)
            {
                Socket socketCliente = socketEscucha.Accept();
                Thread hiloCliente = new Thread(AtencionACliente);
                hiloCliente.Start(socketCliente);
            }
        }

        public void AtencionACliente(object o)
        {

        }
    }
}
