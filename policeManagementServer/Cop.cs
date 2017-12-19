using System;

namespace policeManagementServer
{
    [Serializable]
    public class Cop
    {
        public string Hex { get; set; }
        public string Name { get; set; }
        public string Callsign { get; set; }
        public Department Department { get; set; }
        public bool OnDuty { get; set; }
    }
}
