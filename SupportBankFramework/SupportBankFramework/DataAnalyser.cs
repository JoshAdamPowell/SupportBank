using System;
using System.Collections.Generic;

namespace SupportBankFramework
{
    class DataAnalyser
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
