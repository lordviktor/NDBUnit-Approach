using System.Data;
using NDbUnit.Core;
using NDbUnitXPath.Schemas;
using NUnit.Framework;

namespace NDbUnitQuery
{
    [TestFixture]
    public class GlobalTesting : TestBase
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            SetUpDatabase(
                    ConnectionString,
                    Util.AssemblyResourceStream("Schemas.DBSchema.xsd"),
                    Util.AssemblyResourceStream("TestData.Initial.xml"),
                    DbOperationFlag.CleanInsertIdentity
            );
        }

        [Test]
        public void TestCustomers()
        {
            Assert.IsTrue(
                ResultInspector.AreEqual(
                    ResultInspector.GetExpectedTable(
                        "Schemas.DBSchema.xsd", 
                        "TestData.ExpectedCustomers.xml", 
                        "Customers"
                     ),
                    GetDatabaseCustomers()
                )
            );
        }
        
        [Test]
        public void MarchOrders()
        {
            Assert.IsTrue(
                ResultInspector.AreEqual(
                    ResultInspector.GetExpectedTable(
                        "Schemas.CustomersEmployeesOrders.xsd",
                        "TestData.ExpectedMarchOrders.xml",
                        "Result"
                     ),
                    GetMarchOrders()
                )
            );
        }
        
        #region UtilityMethods
        
        private DBSchema.CustomersDataTable GetDatabaseCustomers()
        {
            DBSchema ds = ResultInspector.LoadPartial<DBSchema>(
                ConnectionString,
                "Customers",
                "SELECT CompanyName, ContactName, ContactTitle, Address " +
                "FROM Customers " +
                "ORDER BY CompanyName, ContactName, ContactTitle, Address"
            );
            return ds.Customers;
        }

        private DataTable GetMarchOrders()
        {
            string fields = "CompanyName, FirstName, LastName, OrderDate";
            string query = "SELECT " + fields + "\r\n" +
                           "FROM Customers " + "\r\n" +
                           "JOIN Orders ON (Customers.CustomerID = Orders.CustomerID )" + "\r\n" +
                           "JOIN Employees ON (Orders.EmployeeID = Employees.EmployeeID)" + "\r\n" +
                           "ORDER BY " + fields;
            return ResultInspector.Load( ConnectionString, query );
        }

        #endregion
    }
}
