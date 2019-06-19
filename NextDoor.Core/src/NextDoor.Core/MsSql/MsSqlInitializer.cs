using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextDoor.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextDoor.Core.MSSQL
{
    public abstract class MsSqlInitializer : IInitializer
    {
        private readonly IOptions<MsSqlDbOptions> _MsSqlOptions;
        private readonly ILogger<MsSqlInitializer> _logger;

        public MsSqlInitializer(
            IOptions<MsSqlDbOptions> msSqlOptions,
            ILogger<MsSqlInitializer> logger)
        {
            _MsSqlOptions = msSqlOptions;
            _logger = logger;
        }


        public async Task InitializeAsync()
        {
            if (!_MsSqlOptions.Value.Seed)
            {
                _logger.LogInformation("Data initialization skipped.");

                return;
            }

            await SeedData();

            _logger.LogInformation("Initializing data...");

            _logger.LogInformation("Data initialized.");
        }

        // Let child Initilizaer override this method
        public abstract Task SeedData();
    }
}
