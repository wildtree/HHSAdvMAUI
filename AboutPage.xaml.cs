namespace HHSAdvMAUI;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
		BindingContext = new AboutPageModel();
    }
}