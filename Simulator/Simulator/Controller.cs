using Simulator.Interfaces;
using Simulator.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace Simulator
{
    class Controller
    {
        public IList<ISysComponent> Components = new List<ISysComponent>();
        public IList<CSVFormat> Data = new List<CSVFormat>();

        public Controller(IList<ISysComponent> components, IList<CSVFormat> data)
        {
            Components = components;
            Data = data;
        }
    }
}
