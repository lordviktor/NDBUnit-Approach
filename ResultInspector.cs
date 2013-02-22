using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;

namespace NDbUnitQuery
{
    internal class ResultInspector
    {
        public static bool AreEqual(DataTable expected, DataTable actual)
        {
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count, "Different number of rows in table: {0}", expected.TableName );
            for (int i = 0; i < expected.Rows.Count; i++)
                AreEqual(expected.Rows[i], actual.Rows[i], i);
            
            return true;
        }

        public static bool AreEqual(DataRow expected, DataRow actual, int rowIndex)
        {
            Assert.AreEqual(expected.ItemArray.Length, actual.ItemArray.Length);
            for (int i = 0; i < expected.ItemArray.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Difference on row:" + rowIndex.ToString() + ", column:" + expected.Table.Columns[i]);
            }
            return true;
        }

        public static TypedDataSet LoadPartial<TypedDataSet>(string connectionString,
            string dataTableName, string selectCommand) where TypedDataSet : DataSet, new()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand mySelectCommand = new SqlCommand(selectCommand, connection);
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(mySelectCommand);
            TypedDataSet dataSet = new TypedDataSet();
            dataSet.EnforceConstraints = false;
            dataSet.DataSetName = typeof(TypedDataSet).ToString();
            mySqlDataAdapter.Fill(dataSet, dataTableName);
            return dataSet;
        }

        public static  DataTable GetExpectedTable(string schemaResourceURI, string dataResourceURI, string tableName)
        {
            DataSet expected = new DataSet();
            Stream stream = Util.AssemblyResourceStream(schemaResourceURI);
            Assert.IsNotNull(stream);
            expected.ReadXmlSchema(stream);
            expected.EnforceConstraints = false;
            expected.ReadXml(Util.AssemblyResourceStream(dataResourceURI));
            return expected.Tables[tableName];
        }

        public static DataTable Load(string connectionString, string selectCommand)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand mySelectCommand = new SqlCommand(selectCommand, connection);
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(mySelectCommand);
            DataSet dataSet = new DataSet();
            mySqlDataAdapter.Fill(dataSet);
            return dataSet.Tables[0];
        }
        
        public static XmlDocument LoadTablesAsXML<TypeDataSet>(string connectionString,
            bool doNestTableElements,                                                                
            params string[] tableNames) where TypeDataSet : DataSet, new()
        {
            TypeDataSet dataSet = new TypeDataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dataAdapter;
            for (int i = 0; i < tableNames.Length; i++)
            {
                SqlCommand command = new SqlCommand(
                    "SELECT * FROM " + tableNames[i],
                    connection);
                dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataSet, tableNames[i]);
            }

            if (doNestTableElements )
            {
                foreach (DataRelation relation in dataSet.Relations)
                {
                    // must select only the relationships on which we are intereseted to avoid elements that have multiple parents.
                    bool containsParent = ContainsName(tableNames, relation.ParentTable.TableName);
                    bool containsChild = ContainsName(tableNames, relation.ChildTable.TableName);
                    if (containsParent && containsChild)
                    {
                        relation.Nested = true;
                    }
                }
            }

            string data = dataSet.GetXml();
            // strip out all the namespace references to make XPath expressions easy to write
            data = Regex.Replace(data,
                                 @"(xmlns:?[^=]*=[""][^""]*[""])", "",
                                 RegexOptions.IgnoreCase | RegexOptions.Multiline);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return doc;
        }

        private static bool ContainsName(string[] names, string name)
        {
            // use the buit-in Array.Find method, with an anonymous delegate.
            string sFound = Array.Find(
                                names,
                                delegate(string currentArrayMember)
                                {
                                    return currentArrayMember.Equals(name);
                                }
                            );
            return sFound != null;
        }
        
    }
}
