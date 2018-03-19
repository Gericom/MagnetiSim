using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MagnetiSim.Sim.Core
{
	public struct Vector3
	{
		public Vector3(double value)
			: this(value, value, value) { }

		public Vector3(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

	    public double X { get; set; }
        public double Y { get; set; }
	    public double Z { get; set; }

	    public double this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				switch (index)
				{
					case 0: X = value; return;
					case 1: Y = value; return;
					case 2: Z = value; return;
				}
				throw new IndexOutOfRangeException();
			}
		}

		[Browsable(false)]
        public double Length => (double)Math.Sqrt(X * X + Y * Y + Z * Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Normalize() 
            => this / Length;

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Dot(Vector3 right) 
            => X * right.X + Y * right.Y + Z * right.Z;

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Cross(Vector3 right) 
            => (Y * right.Z - right.Y * Z, Z * right.X - right.Z * X, X * right.Y - right.X * Y);

	    public double Angle(Vector3 right)
            => (double)Math.Acos(Dot(right) / (Length * right.Length));

        public Vector3 Lerp(Vector3 b, double t)
            => (1 - t) * this + t * b;

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 left, Vector3 right) 
            => (left.X + right.X, left.Y + right.Y, left.Z + right.Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 left, double right)
            => (left.X + right, left.Y + right, left.Z + right);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 left, Vector3 right)
            => (left.X - right.X, left.Y - right.Y, left.Z - right.Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 left, double right)
            => (left.X - right, left.Y - right, left.Z - right);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 left)
            => (-left.X, -left.Y, -left.Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 left, Vector3 right)
            => (left.X * right.X, left.Y * right.Y, left.Z * right.Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 left, double right)
            => (left.X * right, left.Y * right, left.Z * right);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(double left, Vector3 right)
            => (left * right.X, left * right.Y, left * right.Z);

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 left, double right)
            => (left.X / right, left.Y / right, left.Z / right);

	    public static bool operator ==(Vector3 left, Vector3 right)
            => left.Equals(right);

	    public static bool operator !=(Vector3 left, Vector3 right)
            => !left.Equals(right);

	    public override bool Equals(object obj)
		{
			if (!(obj is Vector3 vec))
                return false;
			return vec.X == X && vec.Y == Y && vec.Z == Z;
		}

		public override int GetHashCode()
            => base.GetHashCode();

	    public override string ToString()
            => "(" + X + "; " + Y + "; " + Z + ")";

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    public static implicit operator Vector3((double x, double y, double z) a)
	        => new Vector3(a.x, a.y, a.z);
    }
}
