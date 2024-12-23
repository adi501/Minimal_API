using Minimal_API_EXP2.Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());
// Hello Word
app.MapGet("/", () => "Hello, World!");

List<Employee> LstEmployees = new List<Employee>()
{
    new Employee { ID = 1, Name = "Adi", Address = "AP" },
    new Employee { ID = 2, Name = "JC", Address = "AP" },
    new Employee { ID = 3, Name = "Anil", Address = "KA" },
    new Employee { ID = 4, Name = "pavan", Address = "AP" },
    new Employee { ID = 4, Name = "Raju", Address = "KA" }
};
var employeeAPI = app.MapGroup("/EmployeeAPI");
// GET call - Return all employees
employeeAPI.MapGet("/Employees", () => Results.Ok(LstEmployees));
// GET Return Employee with emp id
employeeAPI.MapGet("/Employee/{ID:int}", (int ID) =>
{
    var employee = LstEmployees.FirstOrDefault(e => e.ID == ID);
    return employee is not null ? Results.Ok(employee) : Results.NotFound();
});
// POST Add new employee
employeeAPI.MapPost("/Employees", (Employee emp) =>
{
    emp.ID = LstEmployees.Max(e => e.ID) + 1;
    LstEmployees.Add(emp);
    return Results.Created($"New Employee {emp.ID} Added successfully", emp);
});
// PUT Update Employee
employeeAPI.MapPut("/Employee/{ID:int}", (int ID, Employee updatedEmp) =>
{
    var emp = LstEmployees.FirstOrDefault(e => e.ID == ID);
    if (emp is null)
        return Results.NotFound();
    LstEmployees.Remove(emp);
    LstEmployees.Add(updatedEmp);
    return Results.Ok(LstEmployees);
});
// DELETE
employeeAPI.MapDelete("/Employee/{ID:int}", (int ID) =>
{
    var emp = LstEmployees.FirstOrDefault(e => e.ID == ID);
    if (emp is null)
        return Results.NotFound();
    LstEmployees.Remove(emp);
    return Results.Ok(LstEmployees);
});
app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Employee>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
