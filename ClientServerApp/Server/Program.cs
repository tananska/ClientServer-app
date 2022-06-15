using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;

namespace Server
{
    class Program
    {
        static Dictionary<Type, object> recievedOperation = new Dictionary<Type, object>();
        static Dictionary<Type, object> recievedData = new Dictionary<Type, object>(); static BinaryMessage operationToSend = new BinaryMessage();
        static BinaryMessage dataToSend = new BinaryMessage();
        static List<Teacher> teachers = new List<Teacher>();
        static List<Student> students = new List<Student>();

        static void Main(string[] args)
        {
            try
            {
                ServerManager.InitializeServer();

                while (true)
                {
                    ServerManager.ListenForNewConnections();

                    do
                    {
                        recievedOperation = ServerManager.WaitForMessage();
                        recievedData = ServerManager.WaitForMessage();

                        ProcessClientOperation();

                        if (!ServerManager.CommunicationIsActive)
                        {
                            break;
                        }
                    } while (true);

                    if (!ServerManager.ContinueListening())
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close the program");
                Console.ReadKey();
            }
            finally
            {
                ServerManager.CloseConnection();
            }
        }
        
            private static void ProcessClientOperation()
            {
            Teacher teacher = null;
            Student student = null;
            int? index;

            int? operation = recievedOperation[typeof(Int32)] as int?;

            switch (operation)
            {
                case 1: teacher = recievedData[typeof(Teacher)] as Teacher;
                    teachers.Add(teacher);
                    Console.WriteLine("Teacher added successfully!");
                    break;

                case 2: student = recievedData[typeof(Student)] as Student;
                    students.Add(student);
                    Console.WriteLine("Student added successfully!");
                    break;

                case 3: dataToSend = TransformDataManager.Serialize(teachers);
                    ServerManager.SendMessage(dataToSend);
                    break;

                case 4: dataToSend = TransformDataManager.Serialize(students);
                    ServerManager.SendMessage(dataToSend);
                    break;

                case 5: index = recievedData[typeof(Int32)] as int?;
                    teacher = teachers[index ?? 0];
                    dataToSend = TransformDataManager.Serialize(teacher);
                    ServerManager.SendMessage(dataToSend);
                    break;

                case 6: index = recievedData[typeof(Int32)] as int?;
                    student = students[index ?? 0];
                    dataToSend = TransformDataManager.Serialize(student);
                    ServerManager.SendMessage(dataToSend);
                    break;

                case 7: ServerManager.CommunicationIsActive = false;
                    break;

                default:
                    break;
              }

            } 
        }
        
    
}
