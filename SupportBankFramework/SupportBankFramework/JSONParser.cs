using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBankFramework
{
    class JSONParser
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static List<Transaction> CreateTransactionListJSON(string filePath)
        {
            string rawJson = System.IO.File.ReadAllText(filePath);
            List<Transaction> transactionList = JsonConvert.DeserializeObject<List<Transaction>>(rawJson);
            log.Info("Imported json file, creating transaction list of length " + transactionList.Count);
            return transactionList;
        }

    }
}
