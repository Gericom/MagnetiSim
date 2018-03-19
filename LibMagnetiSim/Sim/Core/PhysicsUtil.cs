using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnetiSim.Sim.Core
{
    public static class PhysicsUtil
    {
        public static Vector3 GetAcceleration(Vector3 force, double mass) 
            => force / mass;
    }
}
