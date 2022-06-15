using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    [SerializableAttribute]
    public class Student
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public int ClassNumber { get; private set; }
        public char ClassLetter { get; private set; }
        public int NumberInClass { get; private set; }
        public Teacher Teacher { get; private set; }

        private Student()
        {

        }

        public Student(string name, int age, int classNumber, char classLetter, int numberInClass, Teacher teacher)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.Age = age;
            this.ClassNumber = classNumber;
            this.ClassLetter = classLetter;
            this.NumberInClass = numberInClass;
            this.Teacher = teacher;
        }
    }
}
