namespace MeshEditor.Enums
{
    public struct ConstantValue
    {
        internal const double SmallValue = 0.00001;
        internal const double BigValue = 99999;
    }

    public enum VertexType
    {
        ErrorPoint,
        ConvexPoint,
        ConcavePoint
    }

    public enum PolygonType
    {
        Unknown,
        Convex,
        Concave
    }

    public enum PolygonDirection
    {
        Unknown,
        Clockwise,
        CountClockwise
    }
}