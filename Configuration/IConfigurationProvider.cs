using System.Threading.Tasks;

namespace GoogleCalendarCopier.Configuration {
    public interface IConfigurationProvider
    {
        Task<Configuration> Read();
    }
}