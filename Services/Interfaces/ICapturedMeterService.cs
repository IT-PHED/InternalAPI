using INTERNAL.Models;
namespace INTERNAL.Services.Interfaces {
    public interface ICapturedMeterService
    {
        Task<List<CapturedMeters>> GetCapturedMeterReport();
    }
}