using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;

namespace ProniaProject.Services
{
    public class FooterService
    {
        private readonly AppDbContext _context;

        public FooterService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;

        }
    }
}
