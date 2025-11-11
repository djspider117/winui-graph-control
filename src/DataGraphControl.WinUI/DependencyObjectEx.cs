using Microsoft.UI.Xaml;

namespace DataGraphControl.WinUI;

public abstract class DependencyObjectEx<TOwner> : DependencyObject
{
    public static DependencyProperty Register<TPropType>(string propName, PropertyMetadata metadata) 
        => DependencyProperty.Register(propName, typeof(TPropType), typeof(TOwner), metadata);

    public static DependencyProperty Register<TPropType>(string propName, TPropType defaultValue)
        => DependencyProperty.Register(propName, typeof(TPropType), typeof(TOwner), new PropertyMetadata(defaultValue));

    public static DependencyProperty Register<TPropType>(string propName, TPropType defaultValue, PropertyChangedCallback? propertyChangedHandler) 
        => DependencyProperty.Register(propName, typeof(TPropType), typeof(TOwner), new PropertyMetadata(defaultValue, propertyChangedHandler));

}