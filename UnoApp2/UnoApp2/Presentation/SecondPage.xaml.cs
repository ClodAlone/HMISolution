namespace UnoApp2.Presentation;

public sealed partial class SecondPage : Page
{
    public SecondPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ShowSimpleDialog();
    }

    public async Task ShowSimpleDialog()
    {
        var result = await this.Navigator()?.ShowMessageDialogAsync<string>(this, title: "This is Uno", content: "Hello Uno.Extensions!");
    }
}

