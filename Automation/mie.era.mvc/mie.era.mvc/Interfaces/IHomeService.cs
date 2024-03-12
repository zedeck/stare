using mie.era.mvc.Models;

namespace mie.era.mvc.Interfaces
{
    public interface IHomeService
    {
        Task<DashboardViewModel> GetDashboardData(HttpContext context);
    }
}
