using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MGI.FastImaging
{
    // TODO: Consider rewriting, not storing data in remote memory location

    /// <summary>
    /// Stores an ARGB color as a pointer in BGRA order. Used by <see cref="FastBitmap"/>.
    /// </summary>
    public class FastColor
    {
        /// <summary>
        /// Pointer to the <see cref="FastColor"/> data.
        /// </summary>
        public readonly IntPtr ptr;

        /// <summary>
        /// The alpha component value of this <see cref="FastColor"/>.
        /// </summary>
        public byte A { get => Marshal.ReadByte(ptr, 3); set => Marshal.WriteByte(ptr, 3, value); }
        /// <summary>
        /// The red component value of this <see cref="FastColor"/>.
        /// </summary>
        public byte R { get => Marshal.ReadByte(ptr, 2); set => Marshal.WriteByte(ptr, 2, value); }
        /// <summary>
        /// The green component value of this <see cref="FastColor"/>.
        /// </summary>
        public byte G { get => Marshal.ReadByte(ptr, 1); set => Marshal.WriteByte(ptr, 1, value); }
        /// <summary>
        /// The blue component value of this <see cref="FastColor"/>.
        /// </summary>
        public byte B { get => Marshal.ReadByte(ptr, 0); set => Marshal.WriteByte(ptr, 0, value); }

        /// <summary>
        /// Creates a new instance of the <see cref="FastColor"/> class by copying conetents of the specified pointer. Use BGRA order.
        /// </summary>
        /// <param name="ptr">Pointer to a BGRA color. Instance will not point to this memory address.</param>
        public FastColor(IntPtr ptr) : this(Marshal.ReadInt32(ptr)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="FastColor"/> class from specified <see cref="int"/>. Use BGRA order.
        /// </summary>
        /// <param name="bytes">4 bytes long integer in BGRA order.</param>
        public FastColor(int bytes)
        {
            ptr = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(ptr, bytes);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FastColor"/> class with specified values.
        /// </summary>
        /// <param name="a">The alpha component value.</param>
        /// <param name="r">The red component value.</param>
        /// <param name="g">The green component value.</param>
        /// <param name="b">The blue component value.</param>
        public FastColor(byte a, byte r, byte g, byte b)
        {
            ptr = Marshal.AllocHGlobal(4);
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FastColor"/> class from specified <see cref="Color"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> value.</param>
        public FastColor(Color color) : this(color.A, color.R, color.G, color.B) { }

        /// <summary>
        /// Returns an <see cref="int"/> representation of that <see cref="FastColor"/>.
        /// </summary>
        /// <returns><see cref="int"/> representation of that <see cref="FastColor"/>.</returns>
        public int ToInt() => Marshal.ReadInt32(ptr);

        /// <summary>
        /// Cast <see cref="FastColor"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="color"><see cref="FastColor"/> to cast.</param>
        public static explicit operator Color(FastColor color) => Color.FromArgb(color.A, color.R, color.G, color.B);
        
        /// <summary>
        /// Cast <see cref="Color"/> to <see cref="FastColor"/>.
        /// </summary>
        /// <param name="color"><see cref="Color"/> to cast.</param>
        public static explicit operator FastColor(Color color) => new FastColor(color.A, color.R, color.G, color.B);

        /// <summary>
        /// Converts this <see cref="FastColor"/> structure to a human-readable string.
        /// </summary>
        /// <returns>String representation of this <see cref="FastColor"/></returns>
        public override string ToString() => $"[{A}, {R}, {G}, {B}]";

        /// <summary>
        /// Returns a hash code for this <see cref="FastColor"/>.
        /// </summary>
        /// <returns>Hash code for this <see cref="FastColor"/></returns>
        public override int GetHashCode()
        {
            var hashCode = -1749689076;
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Tests whether the specified object is a <see cref="FastColor"/> class and is equivalent to this <see cref="FastColor"/>.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns true if test passes.</returns>
        public override bool Equals(object obj)
        {
            if (obj is FastColor color)
                return ToInt() == color.ToInt();
            else
                return false;
        }

        /// <summary>
        /// Tests if <see cref="FastColor"/>s are equal.
        /// </summary>
        public static bool operator ==(FastColor left, FastColor right) => left.ToInt() == right.ToInt();

        /// <summary>
        /// Tests if <see cref="FastColor"/>s are not equal.
        /// </summary>
        public static bool operator !=(FastColor left, FastColor right) => left.ToInt() != right.ToInt();
    }
}
