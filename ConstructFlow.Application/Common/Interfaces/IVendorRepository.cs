using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Common.Interfaces;

public interface IVendorRepository
{
    Task<int> CreateAsync(Vendor vendor);
    Task<Vendor?> GetByIdAsync(int id);
    Task<IEnumerable<Vendor>> GetAllAsync();
}