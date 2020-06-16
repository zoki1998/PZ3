using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.ELEMENTS
{
   public class SwitchEntity : element
   {
        private string status;
        public SwitchEntity() { }
        public SwitchEntity(long idParam, string nameParam, string statusParam, double xParam, double yParam)
        {
            base.ID = idParam;
            base.Name = nameParam;
            this.Status = statusParam;
            base.X = xParam;
            base.Y = yParam;
        }

        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }
    }
}
