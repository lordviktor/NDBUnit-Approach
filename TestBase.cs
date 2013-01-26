using System.Configuration;
using System.IO;
using NDbUnit.Core;
using NDbUnit.Core.SqlClient;
using NUnit.Framework;

namespace NDbUnitQuery
{
    public class TestBase
    {
        protected const string rootNamespace = "NDbUnitXPath";
        
        protected string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["DbConnectionString"]; }
        }

        
        internal SqlDbUnitTest SetUpDatabase(string connectionString, Stream xsdStream, Stream dataStream, DbOperationFlag operation)
        {
            Assert.IsNotNull(xsdStream);
            Assert.IsNotNull(dataStream);
            SqlDbUnitTest dbUnitTest = new SqlDbUnitTest(connectionString);
            dbUnitTest.ReadXmlSchema(xsdStream);
            dbUnitTest.ReadXml(dataStream);
            dbUnitTest.PerformDbOperation(operation);
            return dbUnitTest;
        }

    }
}
