namespace GraphData;

public static class DemoData
{
    public static Graph CreateDemoGraph()
    {
        var rv = new Graph();

        var rectTypedef = NodeTypeDefinitionBuilder.Create("Rect")
            .WithInputProperty("Transform", KnownPropertyType.Matrix3x2)
            .WithInputProperty("Color", KnownPropertyType.Vector4)
            .WithInputProperty("Size", KnownPropertyType.Vector2)
            .WithOutputProperty("Output", KnownPropertyType.Texture2D)
            .Build();

        var simpleTextTypedef = NodeTypeDefinitionBuilder.Create("SimpleText")
            .WithInputProperty("Transform", KnownPropertyType.Matrix3x2)
            .WithInputProperty("Color", KnownPropertyType.Vector4)
            .WithInputProperty("Text", KnownPropertyType.String)
            .WithOutputProperty("Output", KnownPropertyType.Texture2D)
            .Build();

        var mergeTypedef = NodeTypeDefinitionBuilder.Create("Merge")
            .WithInputProperty("A", KnownPropertyType.Texture2D)
            .WithInputProperty("B", KnownPropertyType.Texture2D)
            .WithInputProperty("Mask", KnownPropertyType.Texture2D)
            .WithOutputProperty("Output", KnownPropertyType.Texture2D)
            .Build();

        var rendererTypedef = NodeTypeDefinitionBuilder.Create("Renderer")
            .WithInputProperty("Texture", KnownPropertyType.Texture2D)
            .Build();

        var nRect1 = new Node(rectTypedef, 0, "Rect1", new(50, 50));
        var nText = new Node(simpleTextTypedef, 1, "Text", new(50, 260));
        var nRect2 = new Node(rectTypedef, 2, "Rect2", new(50, 460));
        var nMerge = new Node(mergeTypedef, 3, "Merge", new(240, 150));
        var nRenderer = new Node(rendererTypedef, 4, "Renderer", new(420, 150));

        rv.Nodes.AddRange(nRect1, nRect2, nText, nMerge, nRenderer);

        
        rv.Connections.Add(new Connection(
            nRect1.Id,
            nRect1.GetOutputProperty("Output").PropertyId,
            nMerge.Id,
            nMerge.GetInputProperty("A").PropertyId));

        rv.Connections.Add(new Connection(
            nText.Id,
            nText.GetOutputProperty("Output").PropertyId,
            nMerge.Id,
            nMerge.GetInputProperty("B").PropertyId));

        rv.Connections.Add(new Connection(
            nMerge.Id,
            nMerge.GetOutputProperty("Output").PropertyId,
            nRenderer.Id,
            nRenderer.GetInputProperty("Texture").PropertyId));

        rv.RebuildNodeCache();

        return rv;
    }
}

// random idea, source gen the cache from a projection object