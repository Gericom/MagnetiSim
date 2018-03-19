using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MagnetiSim.Sim;
using MagnetiSim.Sim.Core;
using MagnetiSim.Sim.Magnetics;
using Tao.OpenGl;
using static Tao.OpenGl.Gl;

namespace MagnetiSim
{
    public partial class Form1 : Form
    {
        private readonly MagneticField _magField;
        private readonly Particle _particle;
        private readonly List<Vector3> _particlePoints = new List<Vector3>();

        public Form1()
        {
            //calculate a field for one of the dipoles of the bar magnet
            var d = new MagneticDipole {MagneticMoment = (0.0000001f, 0, 0)};
            var field = d.GetField(56);

            //combine multiple dipoles to get a bar magnet
            var newField = new MagneticField(field.Width, field.Height, field.Depth, field.StepSize, field.Position);
            for (int i = -12; i < 12; i++)
                for (int j = -2; j < 2; j++)
                    for (int k = -3; k < 3; k++)
                        newField = newField.Add(field, i * 7.8125E-06, j * 7.8125E-06, k * 7.8125E-06);
          
            _magField = newField;

            //create the particle
            _particle = new Particle
            {
                Charge = -1.602e-19f,
                Mass = 1e-20f,
                Position = (0, 0.0001f, 0f),
                Velocity = ( -0.005f / 128, 0, -0.005f / 128)
            };
            _particlePoints.Add(_particle.Position);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadFunctions();
            glEnable(GL_COLOR_MATERIAL);
            glEnable(GL_DEPTH_TEST);
            glDepthFunc(GL_LEQUAL);

            glDisable(GL_CULL_FACE);
            glEnable(GL_TEXTURE_2D);

            glClearDepth(1);
            glEnable(GL_ALPHA_TEST);
            glAlphaFunc(GL_GREATER, 0f);

            glEnable(GL_LINE_SMOOTH);
            glEnable(GL_BLEND);

            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            glClearColor(0, 0, 0, 0);

            Render();
            Render();
        }

        private void DrawFieldLine(MagneticField field, Vector3 start)
        {
            var curPoint = start;
            int count = 0;
            while (true)
            {
                if (!field.Inside(curPoint))
                    break;
                var vec = field.GetVector(curPoint);
                var newPoint = curPoint + vec.Normalize() * 7.8125E-06 * 0.1;
                if (double.IsNaN(newPoint.X + newPoint.Y + newPoint.Z) || vec.Length == 0)
                    break;
                var strength = vec.Length;
                int strengthColor;
                if (double.IsNaN(strength))
                    strengthColor = 255;
                else
                {
                    strengthColor = (int) (strength * strength * 255f / 1000f);
                    if (strengthColor > 255)
                        strengthColor = 255;
                }

                var p1 = ((curPoint - field.Position) / 7.8125E-06 * 6);
                var p2 = ((newPoint - field.Position) / 7.8125E-06 * 6);
                glBegin(GL_LINES);
                {
                    glColor4f(1, 0 / 255f, 0 / 255f, strengthColor / 255f);
                    glVertex3d(p1.X, p1.Y, p1.Z);
                    glVertex3d(p2.X, p2.Y, p2.Z);
                }
                glEnd();
                curPoint = newPoint;
                count++;
                if (count == 1000)
                    break;
            }
        }

        private int _ang = 90;
        private int _ang2 = 0;

        private void Render()
        {
            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();
            glViewport(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);

            //RectangleF rect = displayRect2D;
            //glOrtho(
            //    0, simpleOpenGlControl1.Width, //rect.Left, rect.Right,
            //    simpleOpenGlControl1.Height, 0,
            //    -8192, 8192);
            Glu.gluPerspective(30f, simpleOpenGlControl1.Width / (double) simpleOpenGlControl1.Height, 0.25f, 10000f);

            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();

            glClearColor(0 / 255f, 0 / 255f, 0 / 255f, 0f);
            glEnable(GL_LINE_SMOOTH);
            glEnable(GL_POINT_SMOOTH);

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            glColor4f(1, 1, 1, 1);
            glEnable(GL_TEXTURE_2D);
            glBindTexture(GL_TEXTURE_2D, 0);
            glColor4f(1, 1, 1, 1);
            glDisable(GL_CULL_FACE);
            glEnable(GL_ALPHA_TEST);
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            glAlphaFunc(GL_GREATER, 0f);

            glLoadIdentity();

            glTranslatef(0, 0, -500);

            glRotatef(_ang2, 0, 1, 0);
            glRotatef(_ang, 1, 0, 0);

            glTranslated(-128 / 2 * 6, -128 / 2 * 6, -128 / 2 * 6);

            glPushMatrix();
            {
                //render the bar magnet
                glPushMatrix();
                {
                    glTranslatef(128 / 2 * 6, 128 / 2 * 6, 128 / 2 * 6);
                    glTranslatef(-12.5f * 6, -2.5f * 6, -3.5f * 6);
                    glScaled(12 * 6, 4 * 6, 6 * 6);
                    glColor4f(0.5f, 0.5f, 0.5f, 1.0f);
                    RenderUtil.DrawCube();
                    glTranslatef(1.0f, 0, 0);
                    glColor4f(0.5f, 0f, 0, 1.0f);
                    RenderUtil.DrawCube();
                }
                glPopMatrix();

                //render (a part of) the magnetic field vectors
                for (double z = 0; z < 0.001; z += 1.5625E-05)
                {
                    for (double y = 0.000421875; y < 0.000578125; y += 1.5625E-05)
                    {
                        for (double x = 0; x < 0.001; x += 1.5625E-05)
                        {
                            if (x >= 0 && x < 2 * 12 * 7.8125E-06 &&
                                y >= 0 && y < 2 * 2 * 7.8125E-06 &&
                                z >= 0 && z < 2 * 3 * 7.8125E-06)
                                continue;
                            var vec = _magField.GetVector((x, y, z) + _magField.Position);
                            var dir = vec.Normalize();
                            var strength = vec.Length;
                            int strengthColor;
                            if (double.IsNaN(strength))
                                strengthColor = 255;
                            else
                            {
                                strengthColor = (int) (strength * strength * 255f / 100f);
                                if (strengthColor > 255)
                                    strengthColor = 255;
                            }

                            var center = ((Vector3) (x, y, z) / 7.8125E-06 * 6);
                            var p1 = center - dir * 2f;
                            var p2 = center + dir * 2f;
                            glBegin(GL_LINES);
                            {
                                glColor4f(1, 1, 1, strengthColor / 255f);
                                glVertex3d(p1.X, p1.Y, p1.Z);
                                glVertex3d(p2.X, p2.Y, p2.Z);
                            }
                            glEnd();
                        }
                    }
                }

                //render the particle
                glPointSize(10);
                glBegin(GL_POINTS);
                {
                    glColor4f(0 / 255f, 255 / 255f, 0 / 255f, 1);
                    var pos = ((_particle.Position - _magField.Position) / 7.8125E-06 * 6);
                    glVertex3d(pos.X, pos.Y, pos.Z);
                }
                glEnd();

                /*int margin = 1;
                for (int i = -12 - margin; i < 12 + margin; i++)
                {
                    for (int j = -2 - margin; j < 2 + margin; j++)
                    {
                        for (int k = -3 - margin; k < 3 + margin; k++)
                        {
                            if (i >= -12 && i < 12 &&
                                j >= -2 && j < 2 &&
                                k >= -3 && k < 3)
                                continue;
                            DrawFieldLine(_field3, /*_field3.Position +/ (i * 7.8125E-06, j * 7.8125E-06, k * 7.8125E-06));
                        }
                    }
                }*/

                //render the path of the particle
                if (_particlePoints.Count >= 2)
                {
                    glBegin(GL_LINE_STRIP);
                    {
                        glColor4f(0 / 255f, 128 / 255f, 0 / 255f, 1);
                        foreach (var point in _particlePoints)
                        {
                            var pos = ((point - _magField.Position) / 7.8125E-06 * 6);
                            glVertex3d(pos.X, pos.Y, pos.Z);
                        }
                    }
                    glEnd();
                }
            }
            glPopMatrix();

            simpleOpenGlControl1.Refresh();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Render();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //rotation of the 3d
            if (keyData == Keys.Up)
            {
                _ang += 5;
                Render();
            }
            else if(keyData == Keys.Down)
            {
                _ang -= 5;
                Render();
            }
            else if (keyData == Keys.Left)
            {
                _ang2 += 5;
                Render();
            }
            else if (keyData == Keys.Right)
            {
                _ang2 -= 5;
                Render();
            }
            else if (keyData == Keys.P)
            {
                //evaluation of the particle
                for (int i = 0; i < 100000; i++)
                {
                    _particle.Step(_magField, 0.00001);
                    if (i % 10000 != 0)
                        _particlePoints.Add(_particle.Position);
                }

                Console.WriteLine(_particle.Velocity.Length);
                Render();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}