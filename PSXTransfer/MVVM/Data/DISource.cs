using System;
using System.Windows.Markup;

namespace PSXTransfer.WPF.MVVM.Data
{
    public class DISource : MarkupExtension
    {
        public static Func<Type, object>? Resolver { get; set; }
        public Type? Type { get; set; }
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            return Resolver?.Invoke(Type!);
        }
    }
}
