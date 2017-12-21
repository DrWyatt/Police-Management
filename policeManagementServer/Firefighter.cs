using System;

namespace policeManagementServer
{
    [Serializable]
    public class Firefighter
    {
        public string Hex { get; set; }
        public string Name { get; set; }
        public string Callsign { get; set; }
        public FDepartment Department { get; set; }
        public bool OnDuty { get; set; }
    }
}
