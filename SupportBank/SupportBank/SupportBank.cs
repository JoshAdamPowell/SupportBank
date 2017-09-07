using System;
using System.Collections.Generic;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] rawData = System.IO.File.ReadAllLines(@"C:\Work\Training\supportbank\Transactions2014.csv");
            int dataLength = rawData.Length;

            List<string> dates = new List<string>();
            List<string> accountFrom = new List<string>();
            List<string> accountTo = new List<string>();
            List<float> transactionAmount = new List<float>();
            List<string> transactionDescription = new List<string>();

            for (int i = 1; i < dataLength; i++)
            {
                string line = rawData[i];
                int comma1 = line.IndexOf(",");
                int comma2 = line.IndexOf(",", comma1 + 1);
                int comma3 = line.IndexOf(",", comma2 + 1);
                int comma4 = line.IndexOf(",", comma3 + 1);

                int lineLength = rawData[i].Length;



                dates.Add(line.Substring(0, comma1));
                accountFrom.Add(line.Substring(comma1 + 1, (comma2 - comma1) - 1));
                accountTo.Add(line.Substring(comma2 + 1, (comma3 - comma2) - 1));
                transactionDescription.Add(rawData[i].Substring(comma3 + 1, (comma4 - comma3) - 1));
                transactionAmount.Add(Single.Parse(rawData[i].Substring(comma4 + 1, (lineLength - comma4 - 1))));


            }

            var accountLog = new Dictionary<string, float>();

            for (int i = 0; i < dataLength- 1; i++)
            {
                if (accountLog.ContainsKey(accountFrom[i]))
                {
                    accountLog[accountFrom[i]] = accountLog[accountFrom[i]] - transactionAmount[i];
                }
                else
                {
                    accountLog.Add(accountFrom[i], -transactionAmount[i]);
                }


                if (accountLog.ContainsKey(accountTo[i]))
                {
                    accountLog[accountTo[i]] = accountLog[accountTo[i]] + transactionAmount[i];
                }
                else
                {
                    accountLog.Add(accountTo[i], transactionAmount[i]);
                }



            }

            Console.WriteLine("Please enter command: (List All) or (List [Account])");
            string userInput = Console.ReadLine();

            if (userInput == "List All")
            {
                foreach(var kvp in accountLog)
                {
                    Console.WriteLine(" Account name: " + kvp.Key + " -- Account Value: " + kvp.Value);
                }
                Console.ReadKey();
            }
            if (userInput != "List All" && userInput.Contains("List"))
            {
                int spaceLocation = userInput.IndexOf(" ");
                string accountName = userInput.Substring(spaceLocation + 1);


                for (int i = 0; i < dataLength - 1; i++)
                {
                    if (accountFrom[i] == accountName)
                    {
                        string transactionString = string.Format("{0} Paid {1} to {2} for {3}.", dates[i], transactionAmount[i], accountTo[i], transactionDescription[i]);
                        Console.WriteLine(transactionString);
                    }
                    if (accountTo[i] == accountName)
                    {
                        string transactionString = string.Format("{0} Received {1} from {2} for {3}.", dates[i], transactionAmount[i], accountFrom[i], transactionDescription[i]);
                        Console.WriteLine(transactionString);
                    }
                }
                Console.ReadKey();

            }

        }

    }
}
