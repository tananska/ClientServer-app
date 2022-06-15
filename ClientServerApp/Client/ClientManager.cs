using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using BusinessLayer;

namespace Client
{
    public static class ClientManager
    {
        static TcpClient client;
        static Socket clientSocket;
        static IPAddress clientIPAddress, serverIPAddress;
        static int clientPort, serverPort;
        static string clientIP, serverIP;
        public static bool CommunicationIsActive = true;

        public static void InitializeClient()
        {
            Console.Write("Enter server IP address to use (or nothing for local IP): ");
            serverIP = Console.ReadLine();
            serverIP = (serverIP != string.Empty) ? serverIP : "127.0.0.1";

            Console.Write("Enter server port to use: ");
            serverPort = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter client IP address to use (or nothing for local IP): ");
            clientIP = Console.ReadLine();
            clientIP = (clientIP != string.Empty) ? clientIP : "127.0.0.1";

            Console.Write("Enter client port to use: ");
            clientPort = Convert.ToInt32(Console.ReadLine());

            serverIPAddress = IPAddress.Parse(serverIP);
            clientIPAddress = IPAddress.Parse(clientIP);

            IPEndPoint endPoint = new IPEndPoint(clientIPAddress, clientPort);
            client = new TcpClient(endPoint);

            clientSocket = client.Client;

            client.Connect(serverIPAddress, serverPort);

            Console.WriteLine("Socket local endpoint: {0}", clientSocket.LocalEndPoint.ToString());
            Console.WriteLine("Socket connected: {0}", clientSocket.Connected);
        }

        public static void SendMessage(BinaryMessage binaryMessage)
        {
            clientSocket.Send(binaryMessage.Data);
            Console.WriteLine("Client sent {0} bytes to the server!", binaryMessage.Data.Length);
        }

        public static Dictionary<Type, object> WaitForMessage()
        {
            BinaryMessage binaryMessage = new BinaryMessage();
            clientSocket.Receive(binaryMessage.Data);
            return ReceiveMessage(binaryMessage);
        }

        private static Dictionary<Type, object> ReceiveMessage(BinaryMessage binaryMessage)
        {
            object obj = TransformDataManager.Deserialize(binaryMessage);
            if(obj is string)
            {
                return new Dictionary<Type, object>()
                {
                    { typeof(string), obj.ToString() }
                };
            }
            if(obj is int)
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
            else if(obj is List<Teacher>)
            {
                List<Teacher> teachers = obj as List<Teacher>;

                return new Dictionary<Type, object>()
                {
                    { typeof(List<Teacher>), teachers }
                };
            }
            else if(obj is List<Student>)
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

        public static bool ContinueCommunicating()
        {
            Console.WriteLine("Continue sending data to the server? [y/Y or n/N]");
            string input = Console.ReadLine();
            if (input.ToLower() == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
         public static void CloseConnection()
        {
            client.Close();
        }
    }
}
