using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Zentient.Configurator.Schemas;

namespace Zentient.Configurator.Contexts
{
    /// <summary>
    /// Acts as a bridge between the application and the database. Manages the MongoDB connection and provides access to the configuration data.
    /// </summary>
    public class ConfigurationContext
    {
        private static MongoDbRunner? _runner;
        public IMongoCollection<ConfigurationSchema> Schemas { get; }
        public IMongoCollection<Configuration> Configurations { get; }

        public string? ConnectionString => _runner?.ConnectionString;

        public ConfigurationContext(string databaseName, string? backupFilePath = null)
        {
            _runner = MongoDbRunner.Start();

            if (!string.IsNullOrEmpty(backupFilePath))
            {
                RestoreData(backupFilePath);
            }

            var client = new MongoClient(_runner.ConnectionString);
            var database = client.GetDatabase(databaseName);

            Schemas = database.GetCollection<ConfigurationSchema>("schemas");
            Configurations = database.GetCollection<Configuration>("configurations");
        }

        public async Task BackupDataAsync(string backupFilePath)
        {
            try
            {
                var backupDir = Path.GetDirectoryName(backupFilePath);
                Directory.CreateDirectory(backupDir);

                var adminDb = GetAdminDatabase();

                var command = new BsonDocument { { "createBackup", 1 }, { "backupDir", backupDir } };
                await adminDb.RunCommandAsync<BsonDocument>(command);

                File.Move(Path.Combine(backupDir, "backup"), backupFilePath, overwrite: true);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error backing up data to {backupFilePath}", ex);
            }
            catch (MongoException ex)
            {
                throw new MongoException("Error executing MongoDB command during backup", ex);
            }
        }

        public async Task RestoreData(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                    throw new FileNotFoundException($"Backup file not found: {backupFilePath}");

                var backupDir = Path.GetDirectoryName(backupFilePath);
                Directory.CreateDirectory(backupDir);

                File.Copy(backupFilePath, Path.Combine(backupDir, "backup"), overwrite: true);

                var adminDb = GetAdminDatabase();

                var command = new BsonDocument { { "restoreBackup", 1 }, { "backupDir", backupDir } };
                await adminDb.RunCommandAsync<BsonDocument>(command);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new IOException($"Error restoring data from {backupFilePath}", ex);
            }
            catch (MongoException ex)
            {
                throw new MongoException("Error executing MongoDB command during restore", ex);
            }
        }

        private IMongoDatabase GetAdminDatabase()
        {
            if (_runner?.ConnectionString is null)
            {
                throw new InvalidOperationException("MongoDB connection string is not set.");
            }

            var client = new MongoClient(_runner.ConnectionString);
            return client.GetDatabase("admin");
        }
    }
}
