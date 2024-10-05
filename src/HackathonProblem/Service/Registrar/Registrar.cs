using HackathonProblem.Config;
using Microsoft.Extensions.Options;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Registrar;

public class Registrar(IOptions<EmployeeLists> employeeListsOptions) : IRegistrar
{
    private readonly EmployeeLists _employeeLists = employeeListsOptions.Value;

    public (IEnumerable<Employee> TeamLeads, IEnumerable<Employee> Juniors) Register() =>
        (RegisterCertain(_employeeLists.TeamLeads), RegisterCertain(_employeeLists.Juniors));

    private IEnumerable<Employee> RegisterCertain(string who)
    {
        var employees = new List<Employee>();

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", who);
        using (var streamReader = new StreamReader(path))
        {
            // Пропускаем заголовок
            streamReader.ReadLine();

            while (streamReader.ReadLine() is { } line)
            {
                var values = line.Split(';');
                employees.Add(new Employee(int.Parse(values[0]), values[1]));
            }
        }

        return employees.AsEnumerable();
    }
}
