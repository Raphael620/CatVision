using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace CatVision.Wpf.Helper
{
    public class ItemsControlHelp
    {
        /// <summary>
        /// 绑定 enum 类型所有值给 ItemsSource 赋值
        /// 必须绑定 SelectedItem
        /// </summary>
        public static readonly DependencyProperty EnumValuesToItemsSourceProperty = DependencyProperty.RegisterAttached(
            "EnumValuesToItemsSource", typeof(bool), typeof(ItemsControlHelp), new PropertyMetadata(default(bool), OnEnumValuesToItemsSourceChanged));

        public static void SetEnumValuesToItemsSource(DependencyObject element, bool value)
        {
            element.SetValue(EnumValuesToItemsSourceProperty, value);
        }

        public static bool GetEnumValuesToItemsSource(DependencyObject element)
        {
            return (bool)element.GetValue(EnumValuesToItemsSourceProperty);
        }

        private static void OnEnumValuesToItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl && GetEnumValuesToItemsSource(itemsControl))
            {
                if (itemsControl.IsLoaded)
                {
                    SetItemsSource(itemsControl);
                }
                else
                {
                    itemsControl.Loaded += ItemsControl_Loaded;
                }
            }
        }

        private static void SetItemsSource(ItemsControl itemsControl)
        {
            var itemsBindingExpression = BindingOperations.GetBinding(itemsControl, ItemsControl.ItemsSourceProperty);
            if (itemsBindingExpression != null)
            {
                throw new InvalidOperationException("When using ItemsControlHelper.EnumValuesToItemsSource, cannot be used ItemsSource at the same time.");
            }

            if (itemsControl.Items.Count > 0)
            {
                throw new InvalidOperationException("When using ItemsControlHelper.EnumValuesToItemsSource, Items Collection must be null");
            }

            var bindingExpression = BindingOperations.GetBindingExpression(itemsControl, Selector.SelectedItemProperty);
            if (bindingExpression == null)
            {
                throw new InvalidOperationException("ItemsControl must be binding SelectedItem property");
            }

            var binding = bindingExpression.ParentBinding;
            var dataType = bindingExpression.DataItem?.GetType();
            var paths = binding.Path.Path.Split('.');
            foreach (var path in paths)
            {
                var propertyInfo = dataType?.GetProperty(path);
                if (propertyInfo == null)
                {
                    return;
                }

                dataType = propertyInfo.PropertyType;
            }

            //if (!dataType!.IsEnum)
            if (!dataType.IsEnum)
            {
                var underlyingType = Nullable.GetUnderlyingType(dataType);
                if (underlyingType == null)
                {
                    return;
                }

                dataType = underlyingType;
            }

            var itemsSourceBinding = new Binding();
            itemsSourceBinding.Source = Enum.GetValues(dataType);
            itemsSourceBinding.Mode = BindingMode.OneWay;
            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);
        }

        private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;
            itemsControl.Loaded -= ItemsControl_Loaded;
            SetItemsSource(itemsControl);
        }
    }
}
