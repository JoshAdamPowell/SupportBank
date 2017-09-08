using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SupportBankFramework
{
    class XMLParser
    {
        public static List<Transaction> CreateTransactionListXML(string filePath)
        {
            var transactionList = new List<Transaction>();

            XmlReader readxml = XmlReader.Create(@"C:\Work\Training\supportbank\Transactions2012.xml");
            readxml.MoveToContent();

            bool readingXMLfile = true;
            int i = 0;
            readxml.ReadStartElement("TransactionList");
            while (readingXMLfile)
            {
                Transaction currentTransaction = new Transaction();
                readxml.ReadToFollowing("SupportTransaction");
                string date = readxml.ReadElementContentAsString();
                Console.WriteLine("");
            }

                
            


            var fakeList = new List<Transaction>();
            return fakeList;
        }

    }
}
