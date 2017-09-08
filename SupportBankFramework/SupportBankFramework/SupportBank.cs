using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;
using System.Xml;

namespace SupportBankFramework
{

    class Transaction
    {
        public string date;
        public string fromAccount;
        public string toAccount;
        public string narrative;
        public float amount;

    }





    class SupportBank
    {

        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            StartLogging();

            string filePath = UserHandler.AskUserForFile();

            var transactionList = DataGenerator.GenerateTransactionList(filePath);

            var accountLog = DataGenerator.CreateAccountLog(transactionList);

            UserHandler.AskUserForActions(transactionList, accountLog);

            Console.ReadKey();

        }


        public static void StartLogging()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Training\Supportbank\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

    }

}
