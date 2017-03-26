using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses {
    class EmptyMapException : Exception {

        public EmptyMapException() : base("Map is empty") {

        }

    }
}
