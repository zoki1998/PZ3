using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.ELEMENTS
{
    public class SubstationEntity : element
    {
        public SubstationEntity() { }
        public SubstationEntity(long idParam, string nameParam, double xParam, double yParam)
        {
            base.ID = idParam;
            base.Name = nameParam;
            base.X = xParam;
            base.Y = yParam;
        }
    }
}
