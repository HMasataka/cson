using Newtonsoft.Json;

namespace Company;

public record Employee(string Id, string Name);

var employee = new Employee("user_id", "name");
var json = JsonConvert.SerializeObject(employee, Formatting.Indented);
Console.WriteLine(json);
