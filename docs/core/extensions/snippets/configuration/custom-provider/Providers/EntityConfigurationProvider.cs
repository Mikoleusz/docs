﻿using CustomProvider.Example.Models;
using Microsoft.Extensions.Configuration;

namespace CustomProvider.Example.Providers;

public class EntityConfigurationProvider(
    string? connectionString)
    : ConfigurationProvider
{
    public override void Load()
    {
        using var dbContext = new EntityConfigurationContext(connectionString);

        dbContext.Database.EnsureCreated();

        Data = dbContext.Settings.Any()
            ? dbContext.Settings.ToDictionary<Settings, string, string?>(c => c.Id, c => c.Value, StringComparer.OrdinalIgnoreCase)
            : CreateAndSaveDefaultValues(dbContext);
    }

    static IDictionary<string, string?> CreateAndSaveDefaultValues(
        EntityConfigurationContext context)
    {
        var settings = new Dictionary<string, string?>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["WidgetOptions:EndpointId"] = "b3da3c4c-9c4e-4411-bc4d-609e2dcc5c67",
            ["WidgetOptions:DisplayLabel"] = "Widgets Incorporated, LLC.",
            ["WidgetOptions:WidgetRoute"] = "api/widgets"
        };

        context.Settings.AddRange(
            settings.Select(kvp => new Settings(kvp.Key, kvp.Value))
                    .ToArray());

        context.SaveChanges();

        return settings;
    }
}
