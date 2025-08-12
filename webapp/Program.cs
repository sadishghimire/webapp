using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Runtime.Intrinsics.Arm;
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
    else if (context.Request.Method == "PUT")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            await context.Response.WriteAsync($"This is a put method");
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);
            var result = Employeesrepository.updateEmployees(employee);

            if (result)
            {
                await context.Response.WriteAsync("Content updated successfully");
            }
            else
            {
                await context.Response.WriteAsync("Employee not found");
            }

        }
    }
    else if (context.Request.Method == "DELETE")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            var id=context.Request.Query["id"];
            if(int.TryParse(id, out int employeeID))
            {
                if (context.Request.Headers["Authorization"] == "hehe")
                {

                    var result = Employeesrepository.DeleteEmployees(employeeID);
                    if (result)
                    {
                        await context.Response.WriteAsync($"Deleted successfully\r\n");
                    }
                    else/
                    {
                        await context.Response.WriteAsync($"Not  successfull\r\n");
                    }
                }
                else
                {
                    await context.Response.WriteAsync($"you are not authorized to delete\r\n");
                }

            }
        }
    }


        //this is the code for querystring
    //    foreach (var key in context.Request.Query.Keys)
    //{
    //    await context.Response.WriteAsync($"{key}:{context.Request.Query[key]}\r\n");
    //}
    


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
    public static bool updateEmployees(Employee? employee)
    {
        if (employee is not null)
        {
            var emp =employees.FirstOrDefault(x =>  x.Id == employee.Id);
            if (emp is not null)
            {
                emp.Name = employee.Name;
                emp.Address = employee.Address;
                return true;
            }
        }
        return false;
    }
    public static bool DeleteEmployees(int id)
    {
        var employee = employees.FirstOrDefault(x => x.Id == id);
        if (employee is not null)
        {
            employees.Remove(employee);
            return true;
        }
        return false;
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

