using FateManager.Data;
using System;
using System.Windows.Forms;
using System.Configuration;

namespace FateManager
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UpgradeDatabase();
            Application.Run(new Form1());
        }

        public static void UpgradeDatabase()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ME_FateConnectionString"].ConnectionString;
            bool resetData = bool.Parse(ConfigurationManager.AppSettings["resetData"]);
            DatabaseManager.UpgradeDatabase(connectionString, resetData);
        }
    }
}
