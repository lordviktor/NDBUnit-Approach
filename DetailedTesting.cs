using System.Xml;
using NDbUnit.Core;
using NDbUnitXPath.Schemas;
using NUnit.Framework;

namespace NDbUnitQuery
{
    [TestFixture]
    public class DetailedTesting : TestBase
    {
        XmlDocument xmlDocument;
        
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
        public void AroundTheHornOrderedCPUs()
        {
            xmlDocument = ResultInspector.LoadTablesAsXML<DBSchema>(
                ConnectionString,
                false,
                "Customers",
                "Orders",
                "OrderDetails"
            );
            Assert.IsTrue(
                0 < xmlDocument.SelectNodes(
                        "//Orders[" +
                        "       CustomerID=//Customers[CompanyName='Around the Horn']/CustomerID" +
                        "   and OrderID=//OrderDetails[SKU='CPU-64X2']/OrderID" +
                        "]").Count
            );
        }

        [Test]
        public void MichelaHasCreatedExactlyOneOrder()
        {
            xmlDocument = ResultInspector.LoadTablesAsXML<DBSchema>(
                ConnectionString,
                false,
                "Orders",
                "Employees"
            );
            Assert.AreEqual(1, xmlDocument.SelectNodes("//Orders[EmployeeID=//Employees[LastName='Michela']/EmployeeID]").Count);
        }

        [Test]
        public void AroundTheHornHasBigOrders()
        {
            xmlDocument = ResultInspector.LoadTablesAsXML<DBSchema>(
                ConnectionString,
                true,
                "Customers",
                "Orders",
                "OrderDetails"
            );
            Assert.IsTrue(0 < xmlDocument.SelectNodes("//Customers[CompanyName='Around the Horn']/Orders/OrderDetails[Quantity>=100]").Count);
        }
   }
}
