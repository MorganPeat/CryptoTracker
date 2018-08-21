using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoTracker.MarketData.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace CryptoTracker.MarketData.DataAccess
{
    public class SchemaUpdater
    {
        private const string SchemaVersionSetting = "schemaVersion";
        private readonly SettingsRepository _settingsRepository;
        private readonly IReadOnlyCollection<IUpdateSchema> _schemaUpdaters;

        private readonly ILogger _logger;

        public SchemaUpdater(SettingsRepository settingsRepository, IEnumerable<IUpdateSchema> schemaUpdaters, ILogger<SchemaUpdater> logger)
        {
            _settingsRepository = settingsRepository;
            _schemaUpdaters = schemaUpdaters.ToArray();
            _logger = logger;
        }

        public async Task UpdateSchema(CancellationToken cancellationToken)
        {
            int currentDbSchemaVersion = await _settingsRepository.Get<int>(SchemaVersionSetting, cancellationToken);
            _logger.LogInformation(LoggingEvents.UpdateDatabaseSchema, "Current DB schema is at version {schemaVersion}", currentDbSchemaVersion);

            if (currentDbSchemaVersion < 1)
            {
                _logger.LogInformation(LoggingEvents.UpdateDatabaseSchema, "Updating DB schema from version {currentSchemaVersion} to {newSchemaVersion}", currentDbSchemaVersion, 1);
                await Task.WhenAll(_schemaUpdaters.Select(x => x.Update(currentDbSchemaVersion, cancellationToken)));
                await _settingsRepository.Set(SchemaVersionSetting, 1);
            }

            _logger.LogInformation(LoggingEvents.UpdateDatabaseSchema, "Current DB schema is at version {schemaVersion}", 1);
        }
    }
}