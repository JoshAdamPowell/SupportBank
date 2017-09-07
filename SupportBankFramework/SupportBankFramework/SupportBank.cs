using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        public static void Main(string[] args)
        {

            string[] rawData = System.IO.File.ReadAllLines(@"C:\Work\Training\supportbank\Transactions2014.csv");

            var transactionList = CreateTransactionList(rawData);


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
            foreach (var kvp in dictionary)
            {
                Console.WriteLine(" Account name: " + kvp.Key + " -- Account Value: " + kvp.Value);
            }
        }


        public static void ListSpecific(string userInput, List<Transaction> listOfTransactions)
        {
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
                currentTransaction.description = rawData[i].Substring(comma3 + 1, (comma4 - comma3) - 1);
                currentTransaction.amount = Single.Parse(rawData[i].Substring(comma4 + 1, (lineLength - comma4 - 1)));

                transactionList.Add(currentTransaction);
            }

            return transactionList;
        }


    }
}
