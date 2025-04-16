namespace alexm_app
{
    public partial class App : Application
    {
        public App()
        {
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}