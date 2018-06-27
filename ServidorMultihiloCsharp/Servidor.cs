using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ServidorMultihiloCsharp
{
    public class Servidor
    {
        private const int PUERTO = 31416;
        private Socket socketEscucha;

        public void InicioServidor()
        {
            Console.WriteLine("Servidor");
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, PUERTO);
            socketEscucha = new Socket(AddressFamily.InterNetwork,
                                              SocketType.Stream,
                                              ProtocolType.Tcp);
            socketEscucha.Bind(ie); //OJO, si el puerto de escucha 
                                    //está ocupado lanza excepción.
            socketEscucha.Listen(5);
            Console.WriteLine("A la espera en puerto {0}", PUERTO);
            try
            {
                while (true)
                {
                    Socket socketCliente = socketEscucha.Accept();
                    Thread hiloCliente = new Thread(AtencionACliente);
                    hiloCliente.IsBackground = true;
                    hiloCliente.Start(socketCliente);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Finalizado");
            }

        }

        public void AtencionACliente(object o)
        {
            // Faltaría el control de excepciones
            string mensaje;
            Boolean clienteActivo = true;
            Socket cliente = (Socket)o;
            IPEndPoint ieCliente = (IPEndPoint)cliente.RemoteEndPoint;

            Console.WriteLine("Conectado con el cliente {0} en el puerto {1}",
                            ieCliente.Address, ieCliente.Port);

            NetworkStream ns = new NetworkStream(cliente);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            sw.WriteLine("Bienvenido al servidor");
            sw.Flush();

            while (clienteActivo)
            {
                    // Recibe mensaje del cliente
                    mensaje = sr.ReadLine();

                    //Se recibe null si se cierra el cliente de golpe
                    if (mensaje == null)
                        break;

                    // Gestion del protocolo
                    switch (mensaje)
                    {
                        case "#salir":
                            clienteActivo = false;
                            break;
                        case "#apagar":
                            clienteActivo = false;
                            socketEscucha.Close();
                            break;
                        default:
                            sw.WriteLine(mensaje);
                            sw.Flush();
                            break;
                    }

                    Console.WriteLine("{0} dice: {1}",
                        ieCliente.Address, mensaje);
            }

            Console.WriteLine("Conexión finalizada con {0}:{1}",
                    ieCliente.Address, ieCliente.Port);

            sw.Close();
            sr.Close();
            ns.Close();
            cliente.Close();
        }
    }
}
