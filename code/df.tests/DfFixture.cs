﻿// --------------------------------------------------------------------------------
// <copyright file="DfFixture.cs" company="Michalis Sarigiannidis">
// Copyright 2019 © Michalis Sarigiannidis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// --------------------------------------------------------------------------------

namespace Df.Tests
{
    using Df.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Globalization;

    public sealed class DfFixture
        : IDisposable
    {
        private const string APP_SETTINGS_FILE = "settings.json";

        private const string SECTION_EXTENSIBILITY = "ValueFactoryManagerOptions";

        public string ConnectionString { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public DfFixture()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(PathUtil.CurrentDirectory)
            .AddJsonFile(APP_SETTINGS_FILE, false, true);
            var configuration = builder.Build();
            ConnectionString = configuration
                .GetConnectionString("TestDb")
                .Replace("|DataDirectory|", PathUtil.CurrentDirectory + "\\", false, CultureInfo.InvariantCulture);
            ServiceProvider = ConfigureServices(configuration).BuildServiceProvider();
        }

        public void Dispose()
        {
        }

        private IServiceCollection ConfigureServices(IConfiguration configuration) =>
            new ServiceCollection()
            .AddLogging(_ => _.AddDebug().AddEventSourceLogger().SetMinimumLevel(LogLevel.Debug))
            .AddDfData()
            .AddDfExtensibility(configuration.GetSection(SECTION_EXTENSIBILITY))
            .AddDfIo()
            .AddDfProduction()
            .AddDfOptionHandlers();
    }
}