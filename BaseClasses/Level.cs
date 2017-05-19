using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public class Level
    {
        public Map Map;
        public Dictionary<string, Character> Characters;
        public List<Interactive> Interactives;
    }
}
