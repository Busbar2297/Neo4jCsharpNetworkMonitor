using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neo4jAgent.models
{
    
    public static class functions
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static string GetMemory()
        {
            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);
            return memKb / 1024 / 1024 + " GB";
        }
        public static string getCPU()
        {
            ManagementObjectSearcher mos =
  new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                return mo["Name"].ToString() ;
            }
            return "";
        }
        private static IDriver _driver;

        public static async Task<bool> AddLabelAsync(string ip,string labelName, string Name)
        {
            try
            {
                _driver = GraphDatabase.Driver("neo4j://"+ip+":7687", AuthTokens.Basic("neo4j", "P@ssw0rd"));
                using (var session1 = _driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)))
                {
                    await session1.WriteTransactionAsync(tx => CreateLabel(tx, labelName, Name).ConsumeAsync());

                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        
        public static async Task<bool> RelateParentNodeAsync(string ip,string labelName, string properties,
         string parenttype, string parenttypename)
        {
            string[] props = properties.Split('\n');

            try
            {
                _driver = GraphDatabase.Driver("neo4j://"+ip+":7687", AuthTokens.Basic("neo4j", "P@ssw0rd"));
                using (var session1 = _driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)))
                {
                    await session1.WriteTransactionAsync(tx => CreateNodeWithProp(tx, labelName, props).ConsumeAsync());
                    await session1.WriteTransactionAsync(tx => relate(tx, parenttype, parenttypename, labelName, "medo5",
                        "dependson").ConsumeAsync());
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public static async Task<bool> AddNodePortWithPropAsync(string ip,string labelName, string properties,
         string parenttype, string parenttypename,string childname,string portname)
        {
            string[] props = properties.Split('\n');

            try
            {
                _driver = GraphDatabase.Driver("neo4j://"+ip+":7687", AuthTokens.Basic("neo4j", "P@ssw0rd"));
                using (var session1 = _driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)))
                {
                    await session1.WriteTransactionAsync(tx => CreateNodeWithProp(tx, labelName, props).ConsumeAsync());
                    await session1.WriteTransactionAsync(tx => relate(tx, parenttype, parenttypename, labelName, childname,
                        "Listening_" + portname).ConsumeAsync());
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public static async Task<bool>RelateRootNodeAsync(string ip,string parenttype, string parenttypename, string childname)
        {

            try
            {
                _driver = GraphDatabase.Driver("neo4j://"+ip+":7687", AuthTokens.Basic("neo4j", "P@ssw0rd"));
                using (var session1 = _driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)))
                {
                    await session1.WriteTransactionAsync(tx => relate(tx, "Root", parenttypename, "Host", childname,
                        "MemberOf").ConsumeAsync());
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        private static IResultCursor CreateLabel(IAsyncTransaction tx, string labelName, string name)
        {
            return tx.RunAsync("MERGE (a:" + labelName + " {name: $name})", new { name }).Result;
        }
        private static IResultCursor CreateNodeWithProp(IAsyncTransaction tx, string labelName, string[] props)
        {
            string query = "MERGE (a:" + labelName + " {";
            for (int i = 0; i < props.Length; i++)
            {
                if (i == props.Length - 1)
                {
                    if (!String.IsNullOrEmpty(props[i]))
                    {
                        query += props[i] + "})";

                    }

                }
                else
                {
                    if (!String.IsNullOrEmpty(props[i]))
                    {
                        query += props[i] + ",";
                    }

                }
            }

            return tx.RunAsync(query).Result;
        }
        public static IResultCursor relate(IAsyncTransaction tx, string parenttype, string parenttypename,
            string childtype, string childtypename, string relationshiptype)
        {
            string query = @"MATCH (" + childtype.ToLower() + ":" + childtype + " {name: \"" + childtypename + "\"})" +
             " MATCH (" + parenttype.ToLower() + ":" + parenttype + "{name: \"" + parenttypename + "\"})" +
             "CREATE (" + childtype.ToLower() + ")-[:" + relationshiptype + "]->(" + parenttype.ToLower() + ")";

            return tx.RunAsync(query).Result;
        }
       
    }
}
