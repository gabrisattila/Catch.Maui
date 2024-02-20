using Catch.Maui.ViewModel;
using Catch.Maui.View;
using Catch.Model;
using Catch.Persistence;
using Catch.Model.CatchEventArgs;

namespace Catch.Maui
{
    public partial class AppShell : Shell
    {
        #region Fields
        private DataAccess _dataAccess;
        private GameModel _model;
        private GameViewModel _viewModel;

        private IDispatcherTimer _timer;

        private IStore _store;
        private StoredGameBrowserModel _storedGameBrowserModel;
        private StoredGameBrowserViewModel _storedGameBrowserViewModel;

        private const string Name = "Catch Me If You Can";
        private const string Ok_Button = "OK";
        #endregion

        #region Ctor
        public AppShell(
                        IStore store,
                        DataAccess dataAccess,
                        GameModel gameModel,
                        GameViewModel gameViewModel
                        )
        {
            InitializeComponent();
            _store = store;
            _dataAccess = dataAccess;
            _model = gameModel;
            _viewModel = gameViewModel;

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);

            SetupEventHandlers();
            SetupStoring();
            Start();
        }
        #endregion

        #region Setup Methods
        private void SetupEventHandlers()
        {
            _timer.Tick += Timer_Tick;
            _model.GameOver += Model_GameOver;
            _viewModel.NewGame += ViewModel_NewGame;
            _viewModel.LoadGame += ViewModel_LoadGame;
            _viewModel.SaveGame += ViewModel_SaveGame;
            _viewModel.Settings += ViewModel_Settings;
            _viewModel.PauseGame += ViewModel_PauseGame;
        }


        private void SetupStoring()
        {
            _storedGameBrowserModel = new StoredGameBrowserModel(_store);
            _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
            _storedGameBrowserViewModel.GameLoading += StoredGameBrowserViewModel_GameLoading;
            _storedGameBrowserViewModel.GameSaving += StoredGameBrowserViewModel_GameSaving;
        }

        #endregion


        #region Timer Methods
        internal void Start() => _timer.Start();
        internal void Stop() => _timer.Stop();
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            _viewModel.RefreshTable();
            if (_viewModel.IsGame)
            {
                _model.AdvanceTime();
                if (_model.ENEMY1.Alive)
                {
                    _model.Enemy1Move(_model.Time);
                }
                if (_model.ENEMY2.Alive)
                {
                    _model.Enemy2Move(_model.Time);
                }
            }
        }

        #endregion

        #region Model Handler Methods
        private async void Model_GameOver(object sender, GameOverEventArgs e)
        {
            _viewModel.IsGame = false;
            await DisplayAlert(Name, EndMessage(e._end), Ok_Button);
        }

        private string EndMessage(Ends end)
        {
            if (end == Ends.Player_Exploded)
            {
                return "Vesztettél te két ballábas!";
            }
            else if (end == Ends.Player_Get_Caught)
            {
                return "Vesztettél lassúság!";
            }
            else
            {
                return "Nice boii nyertél :D !";
            }
        }

        #endregion

        #region ViewModel Handler Methods

        private async void ViewModel_NewGame(object sender, NewGameEventArgs e)
        {
            Stop();
            await Navigation.PopAsync();
            _model = new GameModel(e.GameSize);
            _viewModel = new GameViewModel(_model);
            SetupEventHandlers();
            SetupStoring();
            Start();
            BindingContext = _viewModel;
        }
        private async void ViewModel_LoadGame(object sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(new LoadGamePage
            {
                BindingContext = _storedGameBrowserViewModel
            });

        }
        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync();
            await Navigation.PushAsync(new SaveGamePage
            {
                BindingContext = _storedGameBrowserViewModel
            });

        }
        private async void ViewModel_Settings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage
            {
                BindingContext = _viewModel
            });
        }
        private void ViewModel_PauseGame(object sender, EventArgs e)
        {
            if (_viewModel.IsGame)
            {
                _viewModel.IsGame = false;
                Stop();
            }
            else
            {
                _viewModel.IsGame = true;
                Start();
            }
        }

        #endregion

        #region BrowserViewModel Handler Methods
        private async void StoredGameBrowserViewModel_GameLoading(object sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();
            try
            {
                await _dataAccess.LoadGameAsync(e.Name);
                _model = await _dataAccess.LoadGameAsync(e.Name);
                _viewModel = new GameViewModel(_model);
                SetupEventHandlers();
                SetupStoring();
                await Navigation.PopAsync();
                BindingContext = _viewModel;
                await DisplayAlert(Name, "Sikeres betöltés", Ok_Button);
                Start();
            }
            catch
            {
                await DisplayAlert(Name, "Nem sikerült a betöltés", Ok_Button);
            }
        }
        private async void StoredGameBrowserViewModel_GameSaving(object sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();
            try
            {
                await _dataAccess.SaveGameAsync(e.Name, _model);
                await DisplayAlert(Name, "Sikeres mentés", Ok_Button);
            }
            catch
            {
                await DisplayAlert(Name, "Sikertelen mentés", Ok_Button);
            }
        }
        #endregion


    }
}