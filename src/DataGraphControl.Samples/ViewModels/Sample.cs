using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGraphControl.Samples.ViewModels;

public class Sample(string title, string description, Symbol icon, Type xamlType)
{
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public Symbol Icon { get; set; } = icon;
    public Type XamlType { get; set; } = xamlType;
}
