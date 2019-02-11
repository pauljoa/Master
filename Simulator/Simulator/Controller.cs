using Simulator.Interfaces;
using Simulator.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    class Controller
    {
        public IList<Interfaces.ISysComponent> Components = new List<Interfaces.ISysComponent>();
        public IList<CSVFormat> Data = new List<CSVFormat>();

        public Controller(IList<Interfaces.ISysComponent> components, IList<CSVFormat> data)
        {
            Components = components;
            Data = data;
        }
    }
}
