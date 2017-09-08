using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;


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

            Console.WriteLine("First, import a transaction file with 'Import <filename>'. Be sure to include the file extension!");
            string fileNameInput = Console.ReadLine();
            var inputLength = fileNameInput.Length;
            var filePath = fileNameInput.Substring(7, inputLength - 7);


            var transactionList = GenerateData.GenerateTransactionList(filePath);

            var accountLog = GenerateData.CreateAccountLog(transactionList);

            Console.WriteLine("Please enter command: (List All) or (List [Account])");
            string userInput = Console.ReadLine();

            if (userInput == "List All")
            {
                AnalyseData.ListAll(accountLog);
            }
            if (userInput != "List All" && userInput.Contains("List"))
            {
                AnalyseData.ListSpecific(userInput, transactionList);
            }
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

    class GenerateData
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static List<Transaction> GenerateTransactionList(string filePath)
        {
            StartLogging();
            List<Transaction> transactionList = new List<Transaction>();
            if (GetFileExtension(filePath) == "csv")
            {
                string[] rawData = System.IO.File.ReadAllLines(filePath);
                log.Info("loaded rawData from " + filePath);

                transactionList = CreateTransactionListCSV(rawData);
                log.Info("Created transaction list of length {0}, from csv file.", transactionList.Count);
            }
            if (GetFileExtension(filePath) == "json")
            {
                string rawJson = System.IO.File.ReadAllText(filePath);
                transactionList = JsonConvert.DeserializeObject<List<Transaction>>(rawJson);
                log.Info("Imported json file, creating transaction list of length " + transactionList.Count);
            }

            return transactionList;

        }

        public static string GetFileExtension(string fileName)
        {
            int nameLength = fileName.Length;
            var extensionStart = fileName.LastIndexOf(".");
            string extension = fileName.Substring(extensionStart + 1, nameLength - extensionStart - 1);
            return extension;
        }

        public static void StartLogging()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Training\Supportbank\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

        public static List<Transaction> CreateTransactionListCSV(string[] rawData)
        {
            //given a CSV file of a suitable format, this will create a list of transactions.
            int dataLength = rawData.Length;

            List<Transaction> transactionList = new List<Transaction>();


            for (int i = 1; i < dataLength; i++)
            {
                string line = rawData[i];
                int comma1 = line.IndexOf(",");
                int comma2 = line.IndexOf(",", comma1 + 1);
                int comma3 = line.IndexOf(",", comma2 + 1);
                int comma4 = line.IndexOf(",", comma3 + 1);

                int lineLength = rawData[i].Length;


                var currentTransaction = new Transaction
                {
                    date = line.Substring(0, comma1),
                    fromAccount = line.Substring(comma1 + 1, (comma2 - comma1) - 1),
                    toAccount = line.Substring(comma2 + 1, (comma3 - comma2) - 1),
                    narrative = line.Substring(comma3 + 1, (comma4 - comma3) - 1)
                };
                bool amountIsValid = false;
                bool dateIsValid = false;
                try
                {
                    currentTransaction.amount = Single.Parse(rawData[i].Substring(comma4 + 1, (lineLength - comma4 - 1)));
                    amountIsValid = true;
                }
                catch
                {
                    log.Error("Failed to convert stored transaction value into a number. Transaction index: {0}, Transaction date: {1}", i, currentTransaction.date);
                    log.Warn("This transaction has not been stored.");
                }
                try
                {
                    Convert.ToDateTime(currentTransaction.date);
                    dateIsValid = true;
                }
                catch
                {
                    log.Error("Stored Transaction date was not in a valid format. Transaction index: {0}", i);
                    log.Warn("This transaction has not been stored.");

                }
                if (dateIsValid && amountIsValid)
                {
                    transactionList.Add(currentTransaction);
                }
            }

            return transactionList;
        }
        public static Dictionary<string, float> CreateAccountLog(List<Transaction> transactionList)
        {
            //Given a list of all transactions that have occured it will create a dictionary of accounts and values in each.
            var accountLog = new Dictionary<string, float>();

            foreach (var currentTransaction in transactionList)
            {
                if (accountLog.ContainsKey(currentTransaction.fromAccount))
                {
                    accountLog[currentTransaction.fromAccount] = accountLog[currentTransaction.fromAccount] - currentTransaction.amount;
                }
                else
                {
                    accountLog.Add(currentTransaction.fromAccount, -currentTransaction.amount);
                }
            }

            foreach (var currentTransaction in transactionList)
            {
                if (accountLog.ContainsKey(currentTransaction.toAccount))
                {
                    accountLog[currentTransaction.toAccount] = accountLog[currentTransaction.toAccount] + currentTransaction.amount;
                }
                else
                {
                    accountLog.Add(currentTransaction.toAccount, currentTransaction.amount);
                }
            }
            return accountLog;
        }

    }

    class AnalyseData
    {
        public static void ListAll(Dictionary<string, float> dictionary)
        {
            //given a dictionary of accounts with values it will return them all, line by line.
            foreach (var kvp in dictionary)
            {
                Console.WriteLine(" Account name: " + kvp.Key + " -- Account Value: " + kvp.Value);
            }
        }


        public static void ListSpecific(string userInput, List<Transaction> listOfTransactions)
        {
            //given a userinput, which will be in the form "List <<account_name>>" it will find and list all transactions involving that account.
            int spaceLocation = userInput.IndexOf(" ");
            string accountName = userInput.Substring(spaceLocation + 1);


            foreach (var currentTransaction in listOfTransactions)
            {
                if (currentTransaction.fromAccount == accountName)
                {
                    string transactionString = string.Format("{0} Paid {1} to {2} for {3}.", currentTransaction.date, currentTransaction.amount,
                        currentTransaction.toAccount, currentTransaction.narrative);
                    Console.WriteLine(transactionString);
                }

                if (currentTransaction.toAccount == accountName)
                {
                    string transactionString = string.Format("{0} Received {1} from {2} for {3}.", currentTransaction.date, currentTransaction.amount,
                        currentTransaction.fromAccount, currentTransaction.narrative);
                    Console.WriteLine(transactionString);
                }

            }
        }
    }



}
