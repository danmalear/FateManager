using DbUp;
using DbUp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FateManager.Data
{
    public class DatabaseManager
    {
        /// <summary>
        /// Runs all SQL scripts against the given database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="reset"></param>
        public static void UpgradeDatabase(string connectionString, bool reset)
        {

            Console.Out.WriteLine($"Connection String: {connectionString}");
            Console.Out.WriteLine($"Reset data: {reset}");

            // Create the DB if it doesn't exist
            EnsureDatabase.For.SqlDatabase(connectionString);

#if (DEBUG)
            if (reset)
            {
                Console.Out.WriteLine("Running reset script");

                DbUp.Engine.DatabaseUpgradeResult resetResult = Run(connectionString, "Scripts.Reset", false);

                if (!resetResult.Successful)
                {
                    Console.Error.WriteLine(resetResult.Error);
                    return;
                }
            }
#endif
            Console.Out.WriteLine("Running scripts");

            DbUp.Engine.DatabaseUpgradeResult oneTimeResult = Run(connectionString, "Scripts.Upgrade");

            if (!oneTimeResult.Successful)
            {
                Console.Error.WriteLine(oneTimeResult);
                return;
            }
        }

        private static DbUp.Engine.DatabaseUpgradeResult Run(string connectionString, string scriptDirectory, bool runOnce = true)
        {
            var builder = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithTransaction()
                .LogToConsole()
                .WithExecutionTimeout(TimeSpan.FromSeconds(300))
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    s => s.ToLower().Contains(scriptDirectory.ToLower()) &&
                        s.ToLower().Contains(".sql"));

            if (!runOnce)
            {
                builder = builder.JournalTo(new NullJournal());
            }

            return builder.Build().PerformUpgrade();
        }
    }
}
