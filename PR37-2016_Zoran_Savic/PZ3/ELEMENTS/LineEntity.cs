using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.ELEMENTS
{
    public class LineEntity
    {
        private long id;
        private string name;
        private bool isUnderground;
        private double r;
        private string conductorMaterial;
        private string lineType;
        private int terminalConstantHeat;
        private double firstEnd;
        private double secondEnd;
        private List<MyPoint> vertices;
 
        public LineEntity()
        {
            Vertices = new List<MyPoint>();
        }

        public LineEntity(long idParam, string nameParam, bool IsUnderParam, double rParam, string condMaterialParam, string lineTypeParam, int terminalConstantHeatParam, double firstEndParam, double secondEndParam)
        {
            this.id = idParam;
            this.name = nameParam;
            this.isUnderground = IsUnderParam;
            this.r = rParam;
            this.conductorMaterial = condMaterialParam;
            this.lineType = lineTypeParam;
            this.terminalConstantHeat = terminalConstantHeatParam;
            this.firstEnd = firstEndParam;
            this.secondEnd = secondEndParam;
            this.vertices = new List<MyPoint>();
        }
        public long ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        public bool IsUnderground
        {
            get
            {
                return this.isUnderground;
            }
            set
            {
                this.isUnderground = value;
            }
        }
        public double R
        {
            get
            {
                return this.r;
            }
            set
            {
                this.r = value;
            }
        }
        public string ConductorMaterial
        {
            get
            {
                return this.conductorMaterial;
            }
            set
            {
                this.conductorMaterial = value;
            }
        }
        public string LineType
        {
            get
            {
                return this.lineType;
            }
            set
            {
                this.lineType = value;
            }
        }

        public int TerminalConstantHeat
        {
            get
            {
                return this.terminalConstantHeat;
            }
            set
            {
                this.terminalConstantHeat = value;
            }
        }
        public double FirstEnd
        {
            get
            {
                return this.firstEnd;
            }
            set
            {
                this.firstEnd = value;
            }
        }
        public double SecondEnd
        {
            get
            {
                return this.secondEnd;
            }
            set
            {
                this.secondEnd = value;
            }
        }
        public List<MyPoint> Vertices
        {
            get
            {
                return this.vertices;
            }
            set
            {
                this.vertices = value;
            }
        }
    }
}
