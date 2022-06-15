using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;

namespace Client
{
    class Program
    {
        static Dictionary<Type, object> recievedOperation = new Dictionary<Type, object>();
        static Dictionary<Type, object> recievedData = new Dictionary<Type, object>();
        static BinaryMessage operationToSend = new BinaryMessage();
        static BinaryMessage dataToSend = new BinaryMessage();

        static void Main(string[] args)
        {
            try
            {
                ClientManager.InitializeClient();

                do
                {
                    ShowMenu();
                    if (!ClientManager.CommunicationIsActive)
                    {
                        break;
                    }
                } while (ClientManager.ContinueCommunicating());
                EndCommunication();

                Console.WriteLine("Press any key to close the client API!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close the program");
                Console.ReadKey();
            }
            finally
            {
                ClientManager.CloseConnection();
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Select option:");
            Console.WriteLine("1) Add Teacher");
            Console.WriteLine("2) Add Student");
            Console.WriteLine("3) View Teachers");
            Console.WriteLine("4) View Students");
            Console.WriteLine("5) Exit");

            Console.Write("Your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1: AddTeacher(); break;
                case 2: AddStudent(); break;
                case 3: ViewTeachers(); break;
                case 4: ViewStudents(); break;
                case 5: ClientManager.CommunicationIsActive = false; break;
                default: throw new ArgumentException("Invalid option!");
            }
        }

        private static void AddTeacher()
        {

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Age: ");
            int age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Experience: ");
            int experience = Convert.ToInt32(Console.ReadLine());

            Teacher teacher = new Teacher(name, age, experience);

            operationToSend = TransformDataManager.Serialize(1);
            dataToSend = TransformDataManager.Serialize(teacher);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);
        }
        private static void AddStudent()
        {
            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Age: ");
            int age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Class number: ");
            int classNumber = Convert.ToInt32(Console.ReadLine());

            Console.Write("Class letter: ");
            char classLetter = Convert.ToChar(Console.ReadLine());

            Console.Write("Number in class: ");
            int numberInClass = Convert.ToInt32(Console.ReadLine());

            Console.Write("Teacher [enter the index of the Teachers array]: ");
            int index = Convert.ToInt32(Console.ReadLine());

            operationToSend = TransformDataManager.Serialize(5);
            dataToSend = TransformDataManager.Serialize(index);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);

            recievedData = ClientManager.WaitForMessage();

            Teacher teacher = recievedData[typeof(Teacher)] as Teacher;
            Student student = new Student(name, age, classNumber, classLetter, numberInClass, teacher);

            operationToSend = TransformDataManager.Serialize(2);
            dataToSend = TransformDataManager.Serialize(student);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);
        }

        private static void ViewTeachers()
        {
            operationToSend = TransformDataManager.Serialize(3);
            dataToSend = TransformDataManager.Serialize(string.Empty);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);

            recievedData = ClientManager.WaitForMessage();

            List<Teacher> teachers = recievedData[typeof(List<Teacher>)] as List<Teacher>;

            Console.WriteLine(Environment.NewLine + "Teachers Information:");

            foreach (Teacher teacher in teachers)
            {
                Console.WriteLine("ID: {0}", teacher.ID);
                Console.WriteLine("Name: {0} # Age: {1} # Experience: {2}", teacher.Name, teacher.Age, teacher.Experience);
            }
            Console.WriteLine();
        }

        private static void ViewStudents()
        {
            operationToSend = TransformDataManager.Serialize(4);
            dataToSend = TransformDataManager.Serialize(string.Empty);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);

            recievedData = ClientManager.WaitForMessage();

            List<Student> students = recievedData[typeof(List<Student>)] as List<Student>;

            Console.WriteLine(Environment.NewLine + "Students Information:");

            foreach (Student student in students)
            {
                Console.WriteLine("ID: {0}", student.ID);
                Console.WriteLine("Name: {0} # Age: {1} # Class number: {2} # Class letter: {3} # Number in class: {4} ", 
                    student.Name, student.Age, student.ClassNumber, student.ClassLetter, student.NumberInClass);
            }
            Console.WriteLine();
        }

        public static void EndCommunication()
        {
            operationToSend = TransformDataManager.Serialize(7);
            dataToSend = TransformDataManager.Serialize(string.Empty);

            ClientManager.SendMessage(operationToSend);
            ClientManager.SendMessage(dataToSend);
        }
    }
}
