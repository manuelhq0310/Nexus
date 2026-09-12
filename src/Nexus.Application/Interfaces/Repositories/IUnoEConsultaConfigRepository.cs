using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories
{
    public interface IUnoEConsultaConfigRepository
    {
        Task<UnoEConsultaConfig?> GetByCodigoAsync(string codigoConsulta);
        Task<IEnumerable<UnoEConsultaConfig>> GetAllActivasAsync();
        Task<UnoEConsultaConfig> AddAsync(UnoEConsultaConfig entity);
        Task<bool> UpdateAsync(UnoEConsultaConfig entity);
    }
}
