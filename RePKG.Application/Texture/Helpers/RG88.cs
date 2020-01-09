using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RePKG.Application.Texture.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RG88 : IPixel<RG88>, IPackedVector<uint>
    {
        public byte R;
        public byte G;

        private static readonly Vector4 MaxBytes = new Vector4(255, 255, 255, 255);
        private static readonly Vector4 Half = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RG88(byte r, byte g)
        {
            R = r;
            G = g;
        }
        public ushort Rg
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.As<RG88, ushort>(ref this);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Unsafe.As<RG88, ushort>(ref this) = value;
        }

        public uint PackedValue
        {
            get => Rg;
            set => Rg = (ushort) value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(RG88 s) => new Color(new Rgba32(s.G, s.G, s.G, s.R));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RG88(Color color)
        {
            var rgba = color.ToPixel<Rgba32>();

            return new RG88(rgba.R, rgba.G);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RG88 left, RG88 right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RG88 left, RG88 right) => !left.Equals(right);

        public PixelOperations<RG88> CreatePixelOperations() => new PixelOperations<RG88>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromScaledVector4(Vector4 vector) => FromVector4(vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 ToScaledVector4() => ToVector4();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromVector4(Vector4 vector) => Pack(ref vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 ToVector4() => new Vector4(G, G, G, R) / MaxBytes;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromArgb32(Argb32 source) => PackedValue = source.PackedValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromBgr24(Bgr24 source)
        {
            R = source.R;
            G = source.G;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromBgra32(Bgra32 source)
        {
            R = source.R;
            G = source.G;
        }

        public void FromGray8(Gray8 source)
        {
            R = source.PackedValue;
            G = source.PackedValue;
        }

        public void FromGray16(Gray16 source)
        {
            var b = DownScaleFrom16BitTo8Bit(source.PackedValue);
            R = b;
            G = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromRgb24(Rgb24 source)
        {
            R = source.R;
            G = source.G;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromRgba32(Rgba32 source)
        {
            R = source.R;
            G = source.G;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToRgba32(ref Rgba32 dest)
        {
            dest.R = R;
            dest.G = G;
        }

        public static byte DownScaleFrom16BitTo8Bit(ushort component)
        {
            return (byte) (component * byte.MaxValue + 32895 >> 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromRgb48(Rgb48 source)
        {
            R = DownScaleFrom16BitTo8Bit(source.R);
            G = DownScaleFrom16BitTo8Bit(source.G);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromRgba64(Rgba64 source)
        {
            R = DownScaleFrom16BitTo8Bit(source.R);
            G = DownScaleFrom16BitTo8Bit(source.G);
        }

        public override bool Equals(object obj) => obj is Argb32 argb32 && Equals(argb32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RG88 other) => Rg == other.Rg;

        public override string ToString() => $"RG({R}, {G})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => Rg.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Pack(ref Vector4 vector)
        {
            vector *= MaxBytes;
            vector += Half;
            vector = Vector4.Clamp(vector, Vector4.Zero, MaxBytes);

            R = (byte) vector.X;
            G = (byte) vector.Y;
        }
    }
}