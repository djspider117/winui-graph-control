using DataGraphControl.Core;
using DataGraphControl.Samples.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DataGraphControl.Samples.Samples;

public sealed partial class UnityStyleSample : Page
{
    public UnityStyleSample()
    {
        InitializeComponent();
        Loaded += UnityStyleSample_Loaded;
    }

    private void UnityStyleSample_Loaded(object sender, RoutedEventArgs e)
    {
        var demoData = CreateDemoData();
        graph.GraphData = demoData;
    }

    private IGraph CreateDemoData()
    {
        var s = new Scalar { X = 10 };

        var v1 = new V3 { X = 0, Y = 1, Z = 2 };
        var v2 = new V3 { X = 2, Y = 1, Z = 0 };
        var v3 = new V3 { X = 3, Y = 2, Z = 1 };

        var math1 = new VectorMath();
        var math2 = new VectorMath();
        var math3 = new VectorMath();
        var swizler = new VectorSwizzler();
        swizler.ConnectToPort("Input", math3, "Mul");

        var rv = new DataGraph();

        var nv1 = new EvaluatableNode(v1, 1, null, new(180, 77));
        var nv2 = new EvaluatableNode(v2, 2, null, new(180, 313));
        var nm1 = new EvaluatableNode(math1, 4, null, new(409, 209));
        var nv3 = new EvaluatableNode(v3, 3, null, new(409, 458));
        var nm2 = new EvaluatableNode(math2, 5, null, new(666, 308));
        var ns = new EvaluatableNode(s, 0, null, new(666, 52));
        var nm3 = new EvaluatableNode(math3, 6, null, new(878, 193));
        var nsz = new EvaluatableNode(swizler, 7, null, new(1106, 193));

        rv.AddNodes(ns, nv1, nv2, nv3, nm1, nm2, nm3, nsz);

        rv.Connect(nv1, nm1, nameof(VectorMath.A));
        rv.Connect(nv2, nm1, nameof(VectorMath.B));

        rv.Connect(nm1, nameof(VectorMath.Sum), nm2, nameof(VectorMath.A));
        rv.Connect(nv3, nm2, nameof(VectorMath.B));

        rv.Connect(nm2, nameof(VectorMath.Sum), nm3, nameof(VectorMath.A));
        rv.Connect(ns, nm3, nameof(VectorMath.B), FloatToVector3Converter.Instance);

        rv.Connect(nm3, "Mul", nsz, "Input");
        rv.RebuildNodeCache();

        return rv;
    }
}
