namespace HHSAdvMAUI
{

	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();
			this.BindingContext = new SettingsPageModel(ZSystem.Instance.Properties);
        }
	}
}