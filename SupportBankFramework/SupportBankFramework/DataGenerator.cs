using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;

namespace SupportBankFramework
{
    class DataGenerator
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static List<Transaction> GenerateTransactionList(string filePath)
        {
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
            if (GetFileExtension(filePath) == "xml")
            {



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
}
