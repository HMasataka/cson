using Cson;

var employee = new Employee("user_id", "name");
var json = Serializer.SerializeIndented(employee);
Console.WriteLine(json);

var deserialized = Deserializer.Deserialize<Employee>(json);
Console.WriteLine($"Id: {deserialized.Id}, Name: {deserialized.Name}");
