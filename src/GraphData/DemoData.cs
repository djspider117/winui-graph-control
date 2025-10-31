namespace GraphData;

public static class DemoData
{
    public static Graph CreateDemoGraph()
    {
        var rv = new Graph();

        var rectNode = NodeBuilder.Create("Rect")
            .WithPosition(50, 50)
            .WithInputProperties(KnownProperty.Transform, KnownProperty.Color, KnownProperty.Size)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var textNode = NodeBuilder.Create("SimpleText")
            .WithPosition(50, 260)
            .WithInputProperties(KnownProperty.Transform, KnownProperty.Color, KnownProperty.Text)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var merge = NodeBuilder.Create("Merge")
            .WithPosition(240, 150)
            .WithInputProperty(KnownProperty.Texture)
            .WithInputProperty(MetadataCache.GetPropertyByName("Texture2"))
            .WithInputProperty(MetadataCache.GetPropertyByName("Mask"))
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var renderer = NodeBuilder.Create("Renderer")
            .WithPosition(420, 150)
            .WithInputProperty(KnownProperty.Texture)
            .Build();

        rv.Nodes.AddRange(rectNode, textNode, merge, renderer);
        rv.Connections.Add(new Connection(rectNode.Id, (uint)KnownProperty.Texture, merge.Id, (uint)KnownProperty.Texture));
        rv.Connections.Add(new Connection(textNode.Id, (uint)KnownProperty.Texture, merge.Id, MetadataCache.GetPropertyByName("Texture2").PropertyId));
        rv.Connections.Add(new Connection(merge.Id, (uint)KnownProperty.Texture, renderer.Id, (uint)KnownProperty.Texture));

        rv.RebuildNodeCache();

        return rv;
    }

    public static Graph BuildNukeStyleGraph()
    {
        var rv = new Graph();

        var readNode1 = NodeBuilder.Create("Read A")
            .WithPosition(110,50)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var readNode2 = NodeBuilder.Create("Read B")
            .WithPosition(311, 50)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var readNode3 = NodeBuilder.Create("Read C")
            .WithPosition(500, 50)
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var merge1 = NodeBuilder.Create("Merge")
            .WithPosition(311, 150)
            .WithInputProperty(KnownProperty.Texture)
            .WithInputProperty(MetadataCache.GetPropertyByName("Texture2"))
            .WithInputProperty(MetadataCache.GetPropertyByName("Mask"))
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        var merge2 = NodeBuilder.Create("Merge")
            .WithPosition(450, 302)
            .WithInputProperty(KnownProperty.Texture)
            .WithInputProperty(MetadataCache.GetPropertyByName("Texture2"))
            .WithInputProperty(MetadataCache.GetPropertyByName("Mask"))
            .WithOutputProperty(KnownProperty.Texture)
            .Build();

        rv.Nodes.AddRange(readNode1, readNode2, readNode3, merge1, merge2);
        rv.Connections.Add(new Connection(readNode1.Id, (uint)KnownProperty.Texture, merge1.Id, (uint)KnownProperty.Texture));
        rv.Connections.Add(new Connection(readNode2.Id, (uint)KnownProperty.Texture, merge1.Id, MetadataCache.GetPropertyByName("Texture2").PropertyId));
        rv.Connections.Add(new Connection(readNode3.Id, (uint)KnownProperty.Texture, merge1.Id, MetadataCache.GetPropertyByName("Mask").PropertyId));

        rv.Connections.Add(new Connection(merge1.Id, (uint)KnownProperty.Texture, merge2.Id, (uint)KnownProperty.Texture));
        rv.Connections.Add(new Connection(readNode3.Id, (uint)KnownProperty.Texture, merge2.Id, MetadataCache.GetPropertyByName("Texture2").PropertyId));

        rv.RebuildNodeCache();
        return rv;
    }
}

// random idea, source gen the cache from a projection object