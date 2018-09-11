using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage.Tables;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Service.LiteCoin.API.AzureRepositories.Wallet;
using Lykke.Service.LiteCoin.API.Core.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.ObservableWalletsConverter
{
    class Program
    {
        private const string SettingsUrl = "settingsUrl";

        static void Main(string[] args)
        {
            var application = new CommandLineApplication
            {
                Description = "Converts all default address subscriptions to new azure storage model"
            };

            var arguments = new Dictionary<string, CommandArgument>
            {
                { SettingsUrl, application.Argument(SettingsUrl, "Url of a Litecoin API service settings.") }
            };

            application.HelpOption("-? | -h | --help");
            application.OnExecute(async () =>
            {
                try
                {
                    if (arguments.Any(x => string.IsNullOrEmpty(x.Value.Value)))
                    {
                        application.ShowHelp();
                    }
                    else
                    {
                        await ConvertAddresses(arguments[SettingsUrl].Value);
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);

                    return 1;
                }
            });

            application.Execute(args);

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");

            Console.ReadLine();
        }

        private static async Task ConvertAddresses(string settingsUrl)
        {
            if (!Uri.TryCreate(settingsUrl, UriKind.Absolute, out _))
            {
                Console.WriteLine($"{SettingsUrl} should be a valid uri");

                return;
            }

            var logFactory = LogFactory.Create()
                .AddConsole();

            var settings = new SettingsServiceReloadingManager<AppSettings>(settingsUrl, p=>{}).Nested(p=>p.LiteCoinAPI);
            
            var oldStorage = AzureTableStorage<ObservableWalletEntity>.Create(settings.Nested(p => p.Db.DataConnString),
                "ObservableWallets", logFactory);

            var newStorage = AzureTableStorage<ObservableWalletEntity>.Create(settings.Nested(p => p.Db.DataConnString),
                "ObservableWalletsRef", logFactory);
            
            string continuationToken = null;
            var progressCounter = 0;

            Console.WriteLine("Converting wallets...");
            
            var batchSize = 10;
            do
            {

                var rangeQuery = new TableQuery<ObservableWalletEntity>
                {
                    TakeCount = batchSize
                };

                var queryResult = await oldStorage.GetDataWithContinuationTokenAsync(rangeQuery, continuationToken);

                if (queryResult.Entities.Any())
                {
                    var entitiesToInsert = queryResult.Entities.Select(ObservableWalletEntity.Create).ToList();


                    foreach (var observableWalletEntity in entitiesToInsert)
                    {
                       await newStorage.InsertOrReplaceAsync(observableWalletEntity);
                    }

                    progressCounter += entitiesToInsert.Count;
                    continuationToken = queryResult.ContinuationToken;
                }
                else
                {
                    continuationToken = null;
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"{progressCounter} wallets converted");

            } while (continuationToken != null);

        }
    }
}
