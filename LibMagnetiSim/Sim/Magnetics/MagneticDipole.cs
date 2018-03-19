using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MagnetiSim.Sim.Core;

namespace MagnetiSim.Sim.Magnetics
{
    public class MagneticDipole
    {
        public Vector3 MagneticMoment { get; set; }
        public Vector3 Position { get; set; }

        public MagneticField GetField(int sampCount = 128)
        {
            //calculate a sampCount x sampCount x sampCount field for this dipole
            const float dim = 0.001f; //1x1x1 mm
            var field = new MagneticField(sampCount, sampCount, sampCount, dim / sampCount, Position - (dim / 2, dim / 2, dim / 2));
            Parallel.For(0, sampCount, z =>
            {
                for (int y = 0; y < sampCount; y++)
                {
                    for (int x = 0; x < sampCount; x++)
                    {
                        //vector from dipole to point in the field
                        Vector3 r = (
                            (x - (sampCount / 2)) * dim / sampCount,
                            (y - (sampCount / 2)) * dim / sampCount,
                            (z - (sampCount / 2)) * dim / sampCount);
                        var rr = r.Length;
                        //calculate the field vector
                        var b = (float) (Consts.MagneticConstant / (4 * Math.PI)) *
                                (3 * r * MagneticMoment.Dot(r) / (float) Math.Pow(rr, 5) -
                                 MagneticMoment / (float) Math.Pow(rr, 3));
                        if (rr == 0)
                            b = (0, 0, 0);
                        field[z, y, x] = b;
                    }
                }
            });
            return field;
        }
    }
}
