using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.ELEMENTS
{
    public class MyPoint
    {
        private double x;
        private double y;
        public MyPoint()
        {
        }
        public MyPoint(float xParam, float yParam)
        {
            this.x = xParam;
            this.y = yParam;
        }

        public double X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }
        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
    }
}
