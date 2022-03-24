using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4jAgent.models
{
    public class Host
    {
        public string HostName { get; set; }
        public string CPUs { get; set; }
        public string Memory { get; set; }
        public string hostname { get; set; }

        public Host()
        {
            CPUs = "";
            Memory = "";
            hostname = "";
           
        }


    }
}
