using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Models;
using CarTransportDashboard.Context;
using Microsoft.EntityFrameworkCore;

namespace CarTransportDashboard.Repository
{
    public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(ApplicationDbContext context, ILogger<VehicleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id) =>
        await _context.Vehicles.FindAsync(id);

        public async Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber) =>
            await _context.Vehicles
            .FirstOrDefaultAsync(v => v.RegistrationNumber == registrationNumber);

        public async Task<IEnumerable<Vehicle>> GetAllAsync() =>
        await _context.Vehicles.ToListAsync();

        public async Task<OperationResult<Vehicle>> AddAsync(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                return OperationResult<Vehicle>.CreateSuccess(vehicle, "Vehicle added successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to add vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Database error while adding vehicle.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Unexpected error occurred.");
            }
        }

        public async Task<OperationResult<Vehicle>> UpdateAsync(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return OperationResult<Vehicle>.CreateSuccess(vehicle, "Vehicle updated successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Database error while updating vehicle.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Unexpected error occurred.");
            }
        }

        public async Task<OperationResult<Vehicle>> DeleteAsync(Guid id)
        {
            var vehicle = await GetByIdAsync(id);
            if (vehicle is null)
                return OperationResult<Vehicle>.CreateFailure("Vehicle not found.");

            try
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                return OperationResult<Vehicle>.CreateSuccess(vehicle, "Vehicle deleted successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Database error while deleting vehicle.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting vehicle.");
                return OperationResult<Vehicle>.CreateFailure("Unexpected error occurred.");
            }
        }

        public async Task<bool> IsAssignedToActiveJobAsync(Guid vehicleId) =>
        await _context.TransportJobs.AnyAsync(j =>
            j.AssignedVehicleId == vehicleId && j.Status != JobStatus.Completed);
}

}