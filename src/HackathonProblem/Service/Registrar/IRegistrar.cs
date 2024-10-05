using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Registrar;

public interface IRegistrar
{
    (IEnumerable<Employee> TeamLeads, IEnumerable<Employee> Juniors) Register();
}
