using NLog;
using System;
using System.Collections.Generic;

namespace SupportBankFramework
{
    class CSVParser
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

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

                int lineLength = line.Length;


                var currentTransaction = new Transaction
                {
                    date = line.Substring(0, comma1),
                    fromAccount = line.Substring(comma1 + 1, (comma2 - comma1) - 1),
                    toAccount = line.Substring(comma2 + 1, (comma3 - comma2) - 1),
                    narrative = line.Substring(comma3 + 1, (comma4 - comma3) - 1)

                };
                string testAmount = line.Substring(comma4 + 1, (lineLength - comma4 - 1));

                if (VerifyTransactionFields(currentTransaction.date, testAmount, i))
                {
                    transactionList.Add(currentTransaction);
                    currentTransaction.amount = Single.Parse(testAmount);

                }
                
            }

            return transactionList;
        }

        public static bool VerifyTransactionFields(string date, string amount, int id)
        {
            bool amountIsValid = false;
            bool dateIsValid = false;
            try
            {
                Single.Parse(amount);
                amountIsValid = true;
            }
            catch
            {
                log.Error("Failed to convert stored transaction value into a number. Transaction index: {0}, Transaction date: {1}", id, date);
                log.Warn("This transaction has not been stored.");
            }
            try
            {
                Convert.ToDateTime(date);
                dateIsValid = true;
            }
            catch
            {
                log.Error("Stored Transaction date was not in a valid format. Transaction index: {0}", id);
                log.Warn("This transaction has not been stored.");

            }
            if (dateIsValid && amountIsValid) { return true; } else { return false; }

        }


    }
}
