namespace movil_app.Page;

public partial class LoginPage : ContentPage
{
   
    public LoginPage()
	{
		InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        if (Application.Current.MainPage is AppShell appShell)
        {
            appShell.menuuu(true);
            await Shell.Current.GoToAsync("//Inicio");
        }

    }
}