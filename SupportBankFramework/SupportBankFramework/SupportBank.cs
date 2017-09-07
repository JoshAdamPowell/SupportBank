using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBankFramework
{

    class Transaction
    {
        public string dates;
        public string accountFrom;
        public string accountTo;
        public string description;
        public float amount;

    }





    class SupportBank
    {

        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Training\Supportbank\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;



            string path = @"C:\Work\Training\supportbank\DodgyTransactions2015.csv";
            string[] rawData = System.IO.File.ReadAllLines(path);
            log.Info("loaded rawData from " + path);

            var transactionList = CreateTransactionList(rawData);
            log.Info("Created transaction list of length {0}", transactionList.Count);

            var accountLog = CreateAccountLog(transactionList);

            Console.WriteLine("Please enter command: (List All) or (List [Account])");
            string userInput = Console.ReadLine();

            if (userInput == "List All")
            {
                ListAll(accountLog);
            }
            if (userInput != "List All" && userInput.Contains("List"))
            {
                ListSpecific(userInput, transactionList);
            }
            Console.ReadKey();

        }


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
                if (currentTransaction.accountFrom == accountName)
                {
                    string transactionString = string.Format("{0} Paid {1} to {2} for {3}.", currentTransaction.dates, currentTransaction.amount,
                        currentTransaction.accountTo, currentTransaction.description);
                    Console.WriteLine(transactionString);
                }

                if (currentTransaction.accountTo == accountName)
                {
                    string transactionString = string.Format("{0} Received {1} from {2} for {3}.", currentTransaction.dates, currentTransaction.amount,
                        currentTransaction.accountFrom, currentTransaction.description);
                    Console.WriteLine(transactionString);
                }

            }
        }

        public static Dictionary<string, float> CreateAccountLog(List<Transaction> transactionList)
        {
            //Given a list of all transactions that have occured it will create a dictionary of accounts and values in each.
            var accountLog = new Dictionary<string, float>();

            foreach (var currentTransaction in transactionList)
            {
                if (accountLog.ContainsKey(currentTransaction.accountFrom))
                {
                    accountLog[currentTransaction.accountFrom] = accountLog[currentTransaction.accountFrom] - currentTransaction.amount;
                }
                else
                {
                    accountLog.Add(currentTransaction.accountFrom, -currentTransaction.amount);
                }
            }

            foreach (var currentTransaction in transactionList)
            {
                if (accountLog.ContainsKey(currentTransaction.accountTo))
                {
                    accountLog[currentTransaction.accountTo] = accountLog[currentTransaction.accountTo] + currentTransaction.amount;
                }
                else
                {
                    accountLog.Add(currentTransaction.accountTo, currentTransaction.amount);
                }
            }
            return accountLog;
        }

        public static List<Transaction> CreateTransactionList(string[] rawData)
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


                var currentTransaction = new Transaction();


                currentTransaction.dates = line.Substring(0, comma1);
                currentTransaction.accountFrom = line.Substring(comma1 + 1, (comma2 - comma1) - 1);
                currentTransaction.accountTo = line.Substring(comma2 + 1, (comma3 - comma2) - 1);
                currentTransaction.description = line.Substring(comma3 + 1, (comma4 - comma3) - 1);
                bool amountIsValid = false;
                bool dateIsValid = false;
                try
                {
                    currentTransaction.amount = Single.Parse(rawData[i].Substring(comma4 + 1, (lineLength - comma4 - 1)));
                    amountIsValid = true;
                }
                catch
                {
                    log.Error("Failed to convert stored transaction value into a number. Transaction index: {0}, Transaction date: {1}", i, currentTransaction.dates);
                    log.Warn("This transaction has not been stored.");
                }
                try
                {
                    Convert.ToDateTime(currentTransaction.dates);
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


    }
}
