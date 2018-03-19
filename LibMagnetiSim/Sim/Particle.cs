using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagnetiSim.Sim.Core;
using MagnetiSim.Sim.Magnetics;

namespace MagnetiSim.Sim
{
    public class Particle
    {
        public double Charge { get; set; }
        public double Mass { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; set; }

        /// <summary>
        /// Calculates the magnetic force as in the equation F = qv x B
        /// </summary>
        /// <param name="fieldVector">The magnetic field vector B</param>
        /// <returns>The magnetic force F</returns>
        public Vector3 GetMagneticForce(Vector3 fieldVector)
            => Charge * Velocity.Cross(fieldVector);

        public void Step(MagneticField field, double deltaT)
        {
            //evaluate using the midpoint method
            var oldVelo = Velocity;
            var oldPos = Position;
            Velocity += PhysicsUtil.GetAcceleration(GetMagneticForce(field.GetVector(Position)), Mass) * deltaT / 2;
            Position += Velocity * deltaT / 2;

            Velocity = oldVelo + PhysicsUtil.GetAcceleration(GetMagneticForce(field.GetVector(Position)), Mass) * deltaT;
            Position = oldPos + Velocity * deltaT;

            //euler method
            //Velocity += PhysicsUtil.GetAcceleration(GetMagneticForce(field.GetVector(Position)), Mass) * deltaT;
            //Position += Velocity * deltaT;
        }
    }
}
