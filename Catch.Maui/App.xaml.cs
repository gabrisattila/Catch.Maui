using Catch.Maui.Persistence;
using Catch.Maui.ViewModel;
using Catch.Model;
using Catch.Persistence;

namespace Catch.Maui
{
    public partial class App : Application
    {
        #region Fields

        private const string SuspendedGameSavePath = "SuspendedGame";

        private readonly AppShell _appshell;
        private readonly DataAccess _dataAccess;
        private readonly IStore _store;
        private GameModel _model;
        private GameViewModel _viewModel;

        #endregion

        #region Ctor
        public App()
        {
            InitializeComponent();

            _store = new CatchStore();
            _dataAccess = new DataAccess(FileSystem.AppDataDirectory);

            _model = new GameModel(15);
            _viewModel = new GameViewModel(_model);

            _appshell = new AppShell(_store, _dataAccess, _model, _viewModel)
            {
                BindingContext = _viewModel
            };
            MainPage = _appshell;
        }
        #endregion

        #region Methods
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Created += (s, e) =>
            {
                _model = new GameModel(15);
            };

            window.Activated += (s, e) =>
            {
                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                    return;

                Task.Run(async () =>
                {
                    try
                    {
                        _model = await _dataAccess.LoadGameAsync(SuspendedGameSavePath);
                        _viewModel = new GameViewModel(_model);
                        BindingContext = _viewModel;
                    }
                    catch
                    {
                    }
                });
            };

            window.Stopped += (s, e) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _dataAccess.SaveGameAsync(SuspendedGameSavePath, _model);
                    }
                    catch
                    {
                    }
                });
            };

            return window;
        }
        #endregion
    }
}