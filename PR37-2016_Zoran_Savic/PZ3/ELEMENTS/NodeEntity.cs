using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.ELEMENTS
{
    public class NodeEntity : element
    {
        public NodeEntity() { }
        public NodeEntity(long idParam, string nameParam, double xParam, double yParam)
        {
            base.ID = idParam;
            base.Name = nameParam;
            base.X = xParam;
            base.Y = yParam;
        }
    }
}
