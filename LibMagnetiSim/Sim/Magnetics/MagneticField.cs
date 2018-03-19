using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagnetiSim.Sim.Core;

namespace MagnetiSim.Sim.Magnetics
{
    public class MagneticField
    {
        private readonly Vector3[,,] _samples;
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public float StepSize { get; }
        public Vector3 Position { get; }

        public MagneticField(int w, int h, int d, float stepSize, Vector3 position)
        {
            Width = w;
            Height = h;
            Depth = d;
            StepSize = stepSize;
            _samples = new Vector3[d, h, w];
            Position = position;
        }

        public Vector3 this[int z, int y, int x]
        {
            get => _samples[z, y, x];
            set => _samples[z, y, x] = value;
        }

        /// <summary>
        /// Adds together 2 fields
        /// </summary>
        public MagneticField Add(MagneticField b, double offsetX, double offsetY, double offsetZ)
        {
            var result = new MagneticField(Width, Height, Depth, StepSize, Position);
            Parallel.For(0, Depth, z =>
            {
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        result[z, y, x] = _samples[z, y, x] + b.GetVector((Vector3)(x,y,z) * StepSize + Position - (offsetX, offsetY, offsetZ));
            });

            return result;
        }

        /// <summary>
        /// Returns if the point is inside the field
        /// </summary>
        public bool Inside(Vector3 point)
        {
            var inField = (point - Position) / StepSize;
            if (inField.X < 0 || inField.Y < 0 || inField.Z < 0 ||
                inField.X >= Width || inField.Y >= Height || inField.Z >= Depth)
                return false;
            return true;
        }

        /// <summary>
        /// Returns the field vector at the given point
        /// </summary>
        public Vector3 GetVector(Vector3 point)
        {
            if (!Inside(point + (StepSize, StepSize, StepSize)) || !Inside(point - (StepSize, StepSize, StepSize)))
                return (0, 0, 0);
            var inField = (point - Position) / StepSize;
            //carry out trilineair interpolation
            var a = this[(int) Math.Floor(inField.Z), (int) Math.Floor(inField.Y), (int) Math.Floor(inField.X)];
            var b = this[(int) Math.Floor(inField.Z), (int) Math.Floor(inField.Y), (int) Math.Ceiling(inField.X)];
            var ab = a.Lerp(b, inField.X - (float) Math.Floor(inField.X));
            var c = this[(int) Math.Floor(inField.Z), (int) Math.Ceiling(inField.Y), (int) Math.Floor(inField.X)];
            var d = this[(int) Math.Floor(inField.Z), (int) Math.Ceiling(inField.Y), (int) Math.Ceiling(inField.X)];
            var cd = c.Lerp(d, inField.X - (float) Math.Floor(inField.X));
            var e = this[(int) Math.Ceiling(inField.Z), (int) Math.Floor(inField.Y), (int) Math.Floor(inField.X)];
            var f = this[(int) Math.Ceiling(inField.Z), (int) Math.Floor(inField.Y), (int) Math.Ceiling(inField.X)];
            var ef = e.Lerp(f, inField.X - (float) Math.Floor(inField.X));
            var g = this[(int) Math.Ceiling(inField.Z), (int) Math.Ceiling(inField.Y), (int) Math.Floor(inField.X)];
            var h = this[(int) Math.Ceiling(inField.Z), (int) Math.Ceiling(inField.Y), (int) Math.Ceiling(inField.X)];
            var gh = g.Lerp(h, inField.X - (float) Math.Floor(inField.X));

            var abcd = ab.Lerp(cd, inField.Y - (float) Math.Floor(inField.Y));
            var efgh = ef.Lerp(gh, inField.Y - (float) Math.Floor(inField.Y));
            var abcdefgh = abcd.Lerp(efgh, inField.Z - (float) Math.Floor(inField.Z));
            return abcdefgh;
        }
    }
}