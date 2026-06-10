using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Utilities
{
    public static class DatabaseMigrationHelper
    {
        public static bool MigrationHistoryTableExists(DbContext context)
        {
            return TableExists(context, "__EFMigrationsHistory");
        }

        public static bool HasLegacySchema(DbContext context)
        {
            return TableExists(context, "Accounts")
                || TableExists(context, "Products")
                || TableExists(context, "Orders");
        }

        public static void EnsureLegacySchemaCompatibility(DbContext context)
        {
            if (TableExists(context, "Products"))
            {
                EnsureColumn(context, "Products", "VariantsJson", "nvarchar(max) NULL");
            }

            if (TableExists(context, "OrderDetails"))
            {
                EnsureColumn(context, "OrderDetails", "Note", "nvarchar(500) NULL");
            }

            if (TableExists(context, "Brands"))
            {
                if (!ColumnExists(context, "Brands", "Website"))
                {
                    context.Database.ExecuteSqlRaw("ALTER TABLE [Brands] ADD [Website] nvarchar(500) NULL;");
                    if (ColumnExists(context, "Brands", "Description"))
                    {
                        context.Database.ExecuteSqlRaw("UPDATE [Brands] SET [Website] = [Description] WHERE [Website] IS NULL;");
                    }
                }
            }
        }

        public static void EnsureMigrationHistory(DbContext context, IEnumerable<string> migrationIds)
        {
            CreateMigrationHistoryTable(context);

            var productVersion = GetProductVersion();
            foreach (var migrationId in migrationIds)
            {
                if (!MigrationExists(context, migrationId))
                {
                    context.Database.ExecuteSqlRaw(
                        "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (@migrationId, @productVersion);",
                        new SqlParameter("@migrationId", migrationId),
                        new SqlParameter("@productVersion", productVersion));
                }
            }
        }

        private static void CreateMigrationHistoryTable(DbContext context)
        {
            if (TableExists(context, "__EFMigrationsHistory"))
                return;

            context.Database.ExecuteSqlRaw(
                "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] nvarchar(150) NOT NULL PRIMARY KEY, [ProductVersion] nvarchar(32) NOT NULL);");
        }

        private static bool TableExists(DbContext context, string tableName)
        {
            var connection = context.Database.GetDbConnection();
            var openedHere = false;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@tableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);

                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
            finally
            {
                if (openedHere)
                    connection.Close();
            }
        }

        private static bool ColumnExists(DbContext context, string tableName, string columnName)
        {
            var connection = context.Database.GetDbConnection();
            var openedHere = false;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName";
                var tableParameter = command.CreateParameter();
                tableParameter.ParameterName = "@tableName";
                tableParameter.Value = tableName;
                command.Parameters.Add(tableParameter);

                var columnParameter = command.CreateParameter();
                columnParameter.ParameterName = "@columnName";
                columnParameter.Value = columnName;
                command.Parameters.Add(columnParameter);

                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
            finally
            {
                if (openedHere)
                    connection.Close();
            }
        }

        private static void EnsureColumn(DbContext context, string tableName, string columnName, string columnDefinition)
        {
            if (!ColumnExists(context, tableName, columnName))
            {
                context.Database.ExecuteSqlRaw($"ALTER TABLE [{tableName}] ADD [{columnName}] {columnDefinition};");
            }
        }

        private static bool MigrationExists(DbContext context, string migrationId)
        {
            var connection = context.Database.GetDbConnection();
            var openedHere = false;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                openedHere = true;
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [__EFMigrationsHistory] WHERE [MigrationId] = @migrationId";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@migrationId";
                parameter.Value = migrationId;
                command.Parameters.Add(parameter);

                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
            finally
            {
                if (openedHere)
                    connection.Close();
            }
        }

        private static string GetProductVersion()
        {
            return typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "7.0.0";
        }
    }
}
