using Neo4jAgent.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4jAgent
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 1)
            {
                var res = getOpenPorts.GetNetStatPorts();
                functions.GetMemory();
                Console.WriteLine(functions.getCPU());
                //Console.Read();

                Host h = new Host();
                h.CPUs = functions.getCPU();
                h.Memory = functions.GetMemory();
                string props = "Name: " + Environment.MachineName + Environment.NewLine;
                props += "CPU: " + h.CPUs + Environment.NewLine;
                props += "Memory: " + h.Memory;
                List<Tuple<string, string>> addedports = new List<Tuple<string, string>>();
                await functions.AddLabelAsync(args[1],"Root", args[0]);
                await functions.AddLabelAsync(args[1],"Host", Environment.MachineName);

                await functions.RelateRootNodeAsync(args[1],"Root", args[0],Environment.MachineName);
                foreach (var item in res)
                {
                    if (item.status == "LISTENING")
                    {
                        Tuple<string, string> x = new Tuple<string, string>(item.process_name, item.port_number);
                        if (!addedports.Contains(x))
                        {
                            addedports.Add(x);
                            string portprops = "name: \"" + item.process_name + "\"" + Environment.NewLine;
                            portprops += "Status: \"LISTENING\"" + Environment.NewLine;
                            portprops += "ProcessName: \"" + item.process_name + "\"" + Environment.NewLine;
                            portprops += "Port: \"" + item.port_number + "\"";
                            await functions.AddNodePortWithPropAsync(args[1],"Process", portprops, "Host", Environment.MachineName, item.process_name, item.port_number);
                        }



                    }
                } 
            }
            else
            {
                Console.WriteLine("Please enter root node name and server IP...");
                Console.ReadLine();
            }
        }
    }
}
