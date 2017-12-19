using System;
using System.Collections.Generic;

namespace policeManagementServer
{
    [Serializable]
    public class Department
    {
        public string Name { get; set; }
        public List<Cop> officers { get; set; }
        public string Acronym { get; set; }
    }
}
