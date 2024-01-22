namespace F3H.ProfileShark.Geometry
{
    public record Point3D
    {
        public Point3D(double x, double y, double z, double b)
        {
            X = x;
            Y = y;
            Z = z;
            B = b;
        }

        public double X { get; init; }
        public double Y { get; init; }
        public double Z { get; init; }
        public double B { get; init; }
    }

   
}
