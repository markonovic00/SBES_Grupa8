using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public class LocalSettings
    {
        public Dictionary<int, string> Ports { get; }
        public Byte[] Key { get; }

        public LocalSettings()
        {
            Ports = new Dictionary<int, string>();
            Ports.Add(9000, "SevernaBacka");
            Ports.Add(9001, "JuznaBacka");
            Ports.Add(9002, "ZapadnaBacka");
            Ports.Add(9003, "SeverniBanat");
            Ports.Add(9004, "SrednjiBanat");
            Ports.Add(9005, "JuzniBanat");
            Ports.Add(9006, "Srem");

            Key = new byte[]{ 250,226,86,77,136,181,153,166,128,49,63,181,232,214,110,91 };
        }


    }
}
