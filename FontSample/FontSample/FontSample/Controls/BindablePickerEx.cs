using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FontSample.Controls {
	public class BindablePickerEx<T> : Picker
	{
		#region Private fields
		private IList<T> objects;
		#endregion

		#region Construtors

		public BindablePickerEx()
		{
			this.SelectedIndexChanged += OnSelectedIndexChanged;
		}

		#endregion

		#region Properties

		public Func<T, T, bool> Comparer { get; set; }
		public Func<T,string> StringFormat { get; set; }

		#region Items source

		public static BindableProperty ItemsSourceProperty =
			BindableProperty.Create<BindablePickerEx<T>, IList<T>>(o => o.ItemsSource, null, propertyChanged: OnItemsSourceChanged);


		public IList<T> ItemsSource
		{
			get { return (IList<T>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		private static void OnItemsSourceChanged(BindableObject bindable, IList<T> oldvalue, IList<T> newvalue)
		{			
			var picker = bindable as BindablePickerEx<T>;
			picker.objects = newvalue;
			picker.Items.Clear();
			if (newvalue != null)
			{
				foreach (var item in picker.objects)
				{
					var value = picker.StringFormat != null ? picker.StringFormat(item) : item.ToString();
					picker.Items.Add(value);
				}
				setSelectedItem(picker, picker.SelectedItem);
			}
		}

		#endregion

		#region Selected item

		public static BindableProperty SelectedItemProperty =
			BindableProperty.Create<BindablePickerEx<T>, T>(o => o.SelectedItem, default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode:BindingMode.TwoWay);

		public T SelectedItem
		{
			get { return (T)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private static void OnSelectedItemChanged(BindableObject bindable, T oldvalue, T newvalue)
		{
			var picker = bindable as BindablePickerEx<T>;
			setSelectedItem(picker, newvalue);
		}

		#endregion

		#endregion

		#region Utility methods

		private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			if (!(SelectedIndex < 0 || SelectedIndex > Items.Count - 1))
			{
				SelectedItem = objects[SelectedIndex];
			}
		}


		static void setSelectedItem(BindablePickerEx<T> pickerEx, T newvalue)
		{
			if (newvalue != null && pickerEx.objects != null)
			{
				if (pickerEx.Comparer == null)
				{ 
					pickerEx.SelectedIndex = pickerEx.objects.IndexOf(newvalue);
				}
				else
				{
					int i = 0;
					foreach (var item in pickerEx.objects)
					{
						if (pickerEx.Comparer(item, newvalue) == true)
							break;
						i++;
					}
					pickerEx.SelectedIndex = i < pickerEx.objects.Count ? i : -1;
				}
			}
			else
			{
				pickerEx.SelectedIndex = -1;
			}
		}
		#endregion
	}
}
