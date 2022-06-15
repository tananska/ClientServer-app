using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using BusinessLayer;

namespace Server
{
    public static class ServerManager
    {
        static TcpListener server;
        static Socket serverSocket, clientSocket;
        static IPAddress serverIPAddress;
        static int maxConnections, serverPort;
        static string serverIP;
        public static bool CommunicationIsActive = true;

        public static void InitializeServer()
        {
            Console.Write("Enter server IP address to use (or nothing for local IP): ");
            serverIP = Console.ReadLine();
            serverIP = (serverIP != string.Empty) ? serverIP : "127.0.0.1";

            Console.Write("Enter server port to use: ");
            serverPort = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter maximum clients: ");
            maxConnections = Convert.ToInt32(Console.ReadLine());

            serverIPAddress = IPAddress.Parse(serverIP);
            server = new TcpListener(serverIPAddress, serverPort);

            serverSocket = server.Server;

            server.Start(maxConnections);

            Console.WriteLine("Socket local endpoint: {0}", serverSocket.LocalEndPoint.ToString());
        }
        public static void ListenForNewConnections()
        {
            clientSocket = server.AcceptSocket();

            Console.WriteLine("Socket connected: {0}", clientSocket.Connected);
            Console.WriteLine("Socket remote endpoint: {0}", clientSocket.RemoteEndPoint.ToString());
        }

        public static Dictionary<Type, object> WaitForMessage()
        {
            BinaryMessage binaryMessage = new BinaryMessage();

            clientSocket.Receive(binaryMessage.Data);

            return RecieveMessage(binaryMessage);
        }

        private static Dictionary<Type, object> RecieveMessage(BinaryMessage binaryMessage)
        {
            object obj = TransformDataManager.Deserialize(binaryMessage);

            if (obj is string)
            {
                return new Dictionary<Type, object>()
                {
                    { typeof(string), obj.ToString() }
                };
            }
            else if (obj is int)
            {
                return new Dictionary<Type, object>()
                {
                    { typeof(int), int.Parse(obj.ToString()) }
                };
            }
            else if (obj is Teacher)
            {
                Teacher teacher = obj as Teacher;

                return new Dictionary<Type, object>()
                {
                    { typeof(Teacher), teacher }
                };
            }
            else if (obj is Student)
            {
                Student student = obj as Student;

                return new Dictionary<Type, object>()
                {
                    { typeof(Student), student }
                };
            }
            else if (obj is List<Teacher>)
            {
                List<Teacher> teachers = obj as List<Teacher>;

                return new Dictionary<Type, object>()
                {
                    { typeof(List<Teacher>), teachers }
                };
            }
            else if (obj is List<Student>)
            {
                List<Student> students = obj as List<Student>;

                return new Dictionary<Type, object>()
                {
                    { typeof(List<Student>), students }
                };
            }
            else
            {
                throw new ArgumentException("Unsupported type format!");
            }
        }

        public static void SendMessage(BinaryMessage binaryMessage)
        {
            clientSocket.Send(binaryMessage.Data);

            Console.WriteLine("Server sent {0} bytes to the client!", binaryMessage.Data.Length);
        }

        public static bool ContinueListening()
        {
            Console.WriteLine("Continue listening for new connections? [y/Y or n/N]");
            string input = Console.ReadLine();

            if (input.ToLower() == "y")
            {
                CommunicationIsActive = true;
                return true;
            }
            else
            {
                CommunicationIsActive = false;
                return false;
            }
        }

        public static void CloseConnection()
        {
            server.Stop();
        }
    }
}
