using Notia.Desctop.Routing;

namespace Notia.Desctop.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AppRouter _router;

    public MainViewModel(AppRouter router)
    {
        _router = router;
        CurrentViewModel = router.CurrentPage;
    }

    public ViewModelBase CurrentViewModel { get; private set; }

}
