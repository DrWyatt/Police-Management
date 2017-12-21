using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace policeManagementServer
{
    [Serializable]
    public class FDepartment
    {
        public string Name { get; set; }
        public List<Firefighter> firefighters { get; set; }
        public string Acronym { get; set; }
    }
}
