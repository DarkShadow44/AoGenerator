using System.Text;
// ReSharper disable CheckNamespace

internal class Axis
{
    public required bool OriginTopLeft { get; init; }
    public required string Name { get; init; }

    public string MakeCorner(bool resultTopLeft, bool realTopLeft)
    {
        bool isMin;
        bool isOneMinus;

        if (resultTopLeft && realTopLeft)
        {
            isMin = true;
            isOneMinus = false;
        }
        else if (resultTopLeft && !realTopLeft)
        {
            isMin = true;
            isOneMinus = true;
        }
        else if (!realTopLeft && resultTopLeft)
        {
            isMin = false;
            isOneMinus = false;
        }
        else
        {
            isMin = false;
            isOneMinus = true;
        }

        if (!OriginTopLeft)
            isOneMinus = !isOneMinus;

        var sb = new StringBuilder();
        sb.Append(isOneMinus ? "(1.0 - " : "");
        sb.Append(isMin ? "minRender" : "maxRender");
        sb.Append(Name);
        sb.Append(isOneMinus ? ")" : "");
        return sb.ToString();
    }
}

internal class Face
{
    public required string Name { get; init; }
    public required Axis AxisLeftRight { get; init; }
    public required Axis AxisBottomTop { get; init; }

    public string MakeCorner(bool resultTop, bool resultLeft)
    {
        var sb = new StringBuilder();
        // Top left Outer
        sb.Append("f8 * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, true));
        sb.Append(" * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, true));
        sb.Append(", ");
        // Bottom left Outer
        sb.Append("f9 * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, false));
        sb.Append(" * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, true));
        sb.Append(", ");
        // Bottom right Outer
        sb.Append("f10 * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, false));
        sb.Append(" * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, false));
        sb.Append(", ");
        // Top right Outer
        sb.Append("f11 * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, true));
        sb.Append(" * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, false));
        return sb.ToString();
    }
}

internal static class Program
{
    private static void Main()
    {
        Face[] faces =
        [
            new Face
            {
                Name = "XPos",
                AxisLeftRight = new Axis
                {
                    OriginTopLeft = true,
                    Name = "Y"
                },
                AxisBottomTop = new Axis
                {
                    OriginTopLeft = false,
                    Name = "Z"
                }
            }
        ];

        var sb = new StringBuilder();

        foreach (var face in faces)
        {
            sb.AppendLine(face.Name);
            sb.AppendLine(face.MakeCorner(true, true));
            sb.AppendLine(face.MakeCorner(false, true));
            sb.AppendLine(face.MakeCorner(false, false));
            sb.AppendLine(face.MakeCorner(true, false));
            sb.AppendLine();
        }

        File.WriteAllText("/tmp/ramdisk/out.txt", sb.ToString());
    }
}