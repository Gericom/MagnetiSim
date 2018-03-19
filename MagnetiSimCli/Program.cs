using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagnetiSim.Sim;
using MagnetiSim.Sim.Magnetics;

namespace MagnetiSimCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MagnetiSim by Florian Nouwt and Loriana Pascual");
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MagnetiSimCli.exe grid_size output_file");
                return;
            }

            int sampleCount = int.Parse(args[0]);
            var outFile = args[1];
            //for (int sc = 8; sc <= 128; sc += 4)
            //{
            int sc = sampleCount;

            //calculate a field for one of the dipoles of the bar magnet
            var d = new MagneticDipole() {MagneticMoment = (0.0000001f, 0, 0)};
            var field = d.GetField(sc);

            //combine multiple dipoles to get a bar magnet
            var newField = new MagneticField(field.Width, field.Height, field.Depth, field.StepSize, field.Position);
            for (int i = -12; i < 12; i++)
                for (int j = -2; j < 2; j++)
                    for (int k = -3; k < 3; k++)
                        newField = newField.Add(field, i * 7.8125E-06, j * 7.8125E-06, k * 7.8125E-06);

            //create the particle
            var particle = new Particle
            {
                Charge = -1.602e-19f,
                Mass = 1e-20f,
                Position = (0, 0.0001f, 0f),
                Velocity = (-0.005f / 128, 0, -0.005f / 128)
            };

            //run the simulation and collect the results
            var b = new StringBuilder();
            b.AppendLine("t;x;y;z");
            b.AppendLine($"0;{particle.Position.X};{particle.Position.Y};{particle.Position.Z}");
            double t = 0;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    particle.Step(newField, 0.00001);
                    t += 0.00001;
                }

                b.AppendLine($"{t};{particle.Position.X};{particle.Position.Y};{particle.Position.Z}");
            }
            
            //output the results to a csv file
            File.WriteAllText(outFile, b.ToString());
            //File.WriteAllText($"output/out_{sc}.csv", b.ToString());
            //}
            /*var values = new string[31][][];
            int row = 0;
            for (int sc = 8; sc <= 128; sc += 4)
            {
                values[row] = new string[100][];
                string[] s = File.ReadAllLines($"output/out_{sc}.csv");
                for(int i = 0; i < 100; i++)
                {
                    values[row][i] = s[i + 1].Split(';');
                }
                row++;
            }
            var b = new StringBuilder();
            b.Append("t");
            for (int sc = 8; sc <= 128; sc += 4)
                b.Append($";x{sc}");
            for (int sc = 8; sc <= 128; sc += 4)
                b.Append($";y{sc}");
            for (int sc = 8; sc <= 128; sc += 4)
                b.Append($";z{sc}");
            b.AppendLine();
            for (int i = 0; i < 100; i++)
            {
                b.Append(values[0][i][0]);
                row = 0;
                for (int sc = 8; sc <= 128; sc += 4)
                    b.Append(";" + values[row++][i][1]);
                row = 0;
                for (int sc = 8; sc <= 128; sc += 4)
                    b.Append(";" + values[row++][i][2]);
                row = 0;
                for (int sc = 8; sc <= 128; sc += 4)
                    b.Append(";" + values[row++][i][3]);
                b.AppendLine();
            }
            File.WriteAllText("output/out_complete.csv", b.ToString());*/
        }
    }
}