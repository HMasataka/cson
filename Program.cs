using Newtonsoft.Json;

namespace Company
{
    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Employee(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    class Program
    {
        static void Main()
        {
            Employee employee = new Employee("user_id", "name");

            string json = JsonConvert.SerializeObject(employee, Formatting.Indented);

            Console.WriteLine(json);
        }
    }
}
