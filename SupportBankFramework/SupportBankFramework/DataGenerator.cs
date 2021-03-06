﻿using Newtonsoft.Json;
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
                transactionList = CSVParser.CreateTransactionListCSV(filePath);
            }
            if (GetFileExtension(filePath) == "json")
            {
                transactionList = JSONParser.CreateTransactionListJSON(filePath);
            }
            if (GetFileExtension(filePath) == "xml")
            {
                transactionList = XMLParser.CreateTransactionListXML(@"C:\Work\Training\supportbank\Transactions2012.xml");
                //XML parsing code will go here when ready (or, more likely, refer to that code elsewhere)
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

        public static string GetFileExtension(string fileName)
        {
            int nameLength = fileName.Length;
            var extensionStart = fileName.LastIndexOf(".");
            string extension = fileName.Substring(extensionStart + 1, nameLength - extensionStart - 1);
            Console.WriteLine(extension);
            return extension;
        }

    }
}
