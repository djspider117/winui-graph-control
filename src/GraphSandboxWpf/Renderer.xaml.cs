using GraphData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GraphSandboxWpf;

public partial class Renderer : UserControl
{
    private readonly Graph _graph;
    private readonly GraphRenderData _renderData;

    public Renderer()
    {
        _graph = DemoData.CreateDemoGraph();
        _renderData = new GraphRenderData(_graph);

        InitializeComponent();
    }
    protected override void OnRender(DrawingContext dc)
    {
        foreach (var node in _renderData.Nodes.Values)
        {
            dc.DrawRoundedRectangle(node.Background, node.Pen, node.Rect, node.CornerRadius, node.CornerRadius);
            dc.DrawText(node.TitleText, node.TitleLocation);

            RenderProperties(node.InputProperties, dc);
            RenderProperties(node.OutputProperties, dc);
        }

        foreach (var conn in _renderData.Connections)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(conn.Start, false, false);
                ctx.BezierTo(conn.Control1, conn.Control2, conn.End, true, true);
            }

            geometry.Freeze();

            dc.DrawGeometry(null, conn.Pen, geometry);
        }
    }

    private static void RenderProperties(List<PropertyRenderData> propData, DrawingContext dc)
    {
        foreach (var prop in propData)
        {
            dc.DrawEllipse(prop.Color, null, prop.CanvasPosition, prop.Radius, prop.Radius);
            dc.DrawText(prop.TitleText, prop.TextPosition);
        }
    }
}

public class GraphRenderData
{
    private Graph _graph;

    public Dictionary<uint, NodeRenderData> Nodes { get; set; } = [];
    public List<ConnectionRenderData> Connections { get; set; } = [];

    public GraphRenderData(Graph graph)
    {
        _graph = graph;
        Initialize();
    }

    private void Initialize()
    {
        var tf = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        var gray = new SolidColorBrush(Colors.Gray);
        var lgray = new SolidColorBrush(Colors.LightGray);
        var dgray = new SolidColorBrush(Colors.DarkGray);
        var white = new SolidColorBrush(Colors.White);
        var lblue = new SolidColorBrush(Colors.LightBlue);

        foreach (var node in _graph.Nodes)
        {
            const int nodeWidth = 150;
            const int nodeHeight = 150;
            const int rowHeight = 26;

            var ftext = new FormattedText(node.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 12, white, 1);

            var nrd = new NodeRenderData(
                node.Id,
                gray,
                new Pen(dgray, 2),
                8,
                new Rect(node.Position.X, node.Position.Y, nodeWidth, nodeHeight),
                new Point(nodeWidth / 2 - ftext.Width / 2 + node.Position.X, 10 + node.Position.Y),
                ftext,
                [], []);

            Nodes.Add(node.Id, nrd);

            var offsetX = 15;
            var offsetY = ftext.Height + 10;
            int i = 0;
            foreach (var prop in node.InputProperties)
            {
                var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

                ftext = new FormattedText(prop.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 10, white, 1);
                nrd.InputProperties.Add(new PropertyRenderData(
                    prop.PropertyId,
                    lgray,
                    new Point(offsetX + node.Position.X, centerRow),
                    new Point(offsetX + 10 + node.Position.X, centerRow - ftext.Height / 2),
                    ftext,
                    5));

                i++;
            }

            offsetX = nodeWidth - offsetX;
            i = 0;
            foreach (var prop in node.OutputProperties)
            {
                var centerRow = rowHeight / 2 + offsetY + i * rowHeight + node.Position.Y;

                ftext = new FormattedText(prop.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, 10, white, 1);
                nrd.OutputProperties.Add(new PropertyRenderData(
                    prop.PropertyId,
                    lgray,
                    new Point(offsetX + node.Position.X, centerRow),
                    new Point(offsetX - 10 + node.Position.X - ftext.Width, centerRow - ftext.Height / 2),
                    ftext,
                    5));

                i++;
            }
        }

        foreach (var conn in _graph.Connections)
        {
            var srcNodeData = Nodes[conn.SourceNodeId];
            var targetNodeData = Nodes[conn.TargetNodeId];

            // TODO make properties dictionaries
            var srcProp = srcNodeData.OutputProperties.First(x => x.PropertyId == conn.SourcePropertyId);
            var dstProp = targetNodeData.InputProperties.First(x => x.PropertyId == conn.TargetPropertyId);

            Connections.Add(new ConnectionRenderData(conn,
                new Pen(lblue, 2),
                srcProp.CanvasPosition,
                dstProp.CanvasPosition,
                new Point(srcProp.CanvasPosition.X + 32, srcProp.CanvasPosition.Y),
                new Point(dstProp.CanvasPosition.X - 32, dstProp.CanvasPosition.Y)));
        }
    }
}

public record NodeRenderData(uint NodeId, Brush Background, Pen Pen, double CornerRadius, Rect Rect, Point TitleLocation, FormattedText TitleText, List<PropertyRenderData> InputProperties, List<PropertyRenderData> OutputProperties);
public record PropertyRenderData(uint PropertyId, Brush Color, Point CanvasPosition, Point TextPosition, FormattedText TitleText, float Radius);

public record ConnectionRenderData(Connection Connection, Pen Pen, Point Start, Point End, Point Control1, Point Control2);