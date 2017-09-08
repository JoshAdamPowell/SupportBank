using System;
using System.Collections.Generic;

namespace SupportBankFramework
{
    class UserHandler
    {
        public static string AskUserForFile()
        {
            Console.WriteLine("First, import a transaction file with 'Import <filename>'. Be sure to include the file extension!");
            string fileNameInput = Console.ReadLine();
            var inputLength = fileNameInput.Length;
            var filePath = fileNameInput.Substring(7, inputLength - 7);
            return filePath;
        }

        public static void ExecuteUserCommands(List<Transaction> transactionList, Dictionary<string, float> accountLog)
        {
            Console.WriteLine("Please enter command: (List All) or (List [Account])");
            string userInput = Console.ReadLine();

            if (userInput == "List All")
            {
                DataAnalyser.ListAll(accountLog);
            }
            if (userInput != "List All" && userInput.Contains("List"))
            {
                DataAnalyser.ListSpecific(userInput, transactionList);
            }
        }

    }
}
