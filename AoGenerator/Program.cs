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

        // Take opposite corner
        realTopLeft = !realTopLeft;

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
        else if (!resultTopLeft && realTopLeft)
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
        {
            isOneMinus = !isOneMinus;
            isMin = !isMin;
        }

        var sb = new StringBuilder();
        sb.Append(isOneMinus ? "(1.0D - " : "");
        sb.Append(isMin ? "renderMin" : "renderMax");
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

    public string MakeCorner(string resultName, bool resultTop, bool resultLeft)
    {
        var sb = new StringBuilder();
        sb.Append($"float {resultName} = (");
        // Top left Outer
        sb.Append("f9 * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, true));
        sb.Append(" * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, true));
        sb.Append(" + ");
        // Bottom left Outer
        sb.Append("f10 * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, true));
        sb.Append(" * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, false));
        sb.Append(" + ");
        // Bottom right Outer
        sb.Append("f11 * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, false));
        sb.Append(" * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, false));
        sb.Append(" + ");
        // Top right Outer
        sb.Append("f8 * ");
        sb.Append(AxisLeftRight.MakeCorner(resultLeft, false));
        sb.Append(" * ");
        sb.Append(AxisBottomTop.MakeCorner(resultTop, true));

        sb.Append(')');
        return sb.ToString();
    }
}

internal static class Program
{
    private static void Main()
    {
        Face[] faces =
        [
            new()
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
            },
            new()
            {
                Name = "XNeg",
                AxisLeftRight = new Axis
                {
                    OriginTopLeft = false,
                    Name = "Y"
                },
                AxisBottomTop = new Axis
                {
                    OriginTopLeft = false,
                    Name = "Z"
                }
            },
            new()
            {
                Name = "ZPos",
                AxisLeftRight = new Axis
                {
                    OriginTopLeft = true,
                    Name = "X"
                },
                AxisBottomTop = new Axis
                {
                    OriginTopLeft = false,
                    Name = "Y"
                }
            },
            new()
            {
                Name = "YPos",
                AxisLeftRight = new Axis
                {
                    OriginTopLeft = false,
                    Name = "X"
                },
                AxisBottomTop = new Axis
                {
                    OriginTopLeft = false,
                    Name = "Z"
                }
            },
            new()
            {
                Name = "YNeg",
                AxisLeftRight = new Axis
                {
                    OriginTopLeft = true,
                    Name = "X"
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
            sb.AppendLine(face.Name + "(generated)");
            sb.AppendLine(face.MakeCorner("f3", true, true));
            sb.AppendLine(face.MakeCorner("f4", false, true));
            sb.AppendLine(face.MakeCorner("f5", false, false));
            sb.AppendLine(face.MakeCorner("f6", true, false));
        }

        File.WriteAllText("/tmp/ramdisk/out.txt", sb.ToString());
    }
}