using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run(async (HttpContext context) =>
{
    if (context.Request.Method == "GET")
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            //Endpoint handling
            await context.Response.WriteAsync($"The method is:{context.Request.Method}\r\n");
            await context.Response.WriteAsync($"The url is:{context.Request.Path}\r\n");
            await context.Response.WriteAsync($"The header is:{context.Request.Headers}\r\n");

            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"{key}:{context.Request.Headers[key]}\r\n");
            }
        }

        else if (context.Request.Path.StartsWithSegments("/employees"))
        {
            //Endpoint handling
            var employees = Employeesrepository.GetEmployees();
            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"{employee.Name}:{employee.Address}\r\n");
            }
        }

    }
    else if (context.Request.Method == "POST")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            await context.Response.WriteAsync($"This is a post method");
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);
            Employeesrepository.AddEmployees(employee);
        }
    }

    
});

app.Run();

static class Employeesrepository
{
    private static List<Employee> employees = new List<Employee>
    {
        new Employee(1,"sadish","kathmandu"),
        new Employee(2,"Ram","palpa"),
        new Employee(3,"Shyam","butwal")
    };
    public static List<Employee> GetEmployees() => employees;
   
    public static void AddEmployees(Employee? employee)
    {
        if(employee is not null)
        {
            employees.Add(employee);
        }

        
    }
}


public class Employee { 

    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    
    public Employee(int id, string name, string address)

    {
        Id = id;
        Name=name; 
        Address=address; 
       
    }

}

