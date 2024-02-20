using Catch.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Maui.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        #region Fields & Props

        private Brush _Pcolor = Brush.Orange;
        private Brush _Ecolor = Brush.Blue;
        private Brush _Fieldcolor = Brush.Green;
        private Brush _Minecolor = Brush.Black; 

        private GameModel _model;
        private int _size;
        private bool _onGame;

        public GameModel Model { get { return _model; } set { _model = value; } }
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GameTableRows));
                OnPropertyChanged(nameof(GameTableColumns));
            }
        }
        public bool IsGame { get { return _onGame; } set { _onGame = value; } }
        public String Time { get { return _model.Time.ToString(); } }
        public RowDefinitionCollection GameTableRows
        {
            get => new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), Size).ToArray());
        }
        public ColumnDefinitionCollection GameTableColumns
        {
            get => new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), Size).ToArray());
        }
        public ObservableCollection<Field> Fields { get; set; }

        public string All { get { return IsGame ? "Forog a gép" : "Szünet"; } }

        #endregion

        #region Commands & Events
        public DelegateCommand NewGameCommand { get; set; }
        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }
        public DelegateCommand SettingsCommand { get; set; }
        public DelegateCommand PauseGameCommand { get; set; }

        public event EventHandler<NewGameEventArgs> NewGame = null!;
        public event EventHandler SaveGame = null!;
        public event EventHandler LoadGame = null!;
        public event EventHandler Settings = null!;
        public event EventHandler PauseGame = null!;

        #endregion

        #region Ctor
        public GameViewModel(GameModel gameModel)
        {
            _model = gameModel;
            Size = _model.BoardSize;
            Fields = new ObservableCollection<Field>();
            GenerateFields();

            NewGameCommand = new DelegateCommand(gameSize => { NewGame?.Invoke(this, new NewGameEventArgs(Convert.ToInt32(gameSize))); });
            LoadGameCommand = new DelegateCommand(_ => OnLoad());
            SaveGameCommand = new DelegateCommand(_ => OnSave());   
            SettingsCommand = new DelegateCommand(_ => OnSettings());
            PauseGameCommand = new DelegateCommand(_ => OnPause());

            _onGame = true;
        }
        #endregion

        #region Handler Methods
        private void OnSave()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnLoad()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnSettings()
        {
            Settings?.Invoke(this, EventArgs.Empty);
        }
        private void OnPause()
        {
            PauseGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods
        private Brush GetColor(int i, int j)
        {
            switch (_model.Table.Tábla[i, j])
            {
                case 0: return _Fieldcolor;
                case 1: return _Minecolor;
                case 2: return _Ecolor;
                case 3: return _Pcolor;
                case 4: return _Ecolor;
                case 5: return _Minecolor;
                case 6: return _Minecolor;
                case 7: return _Minecolor;
                default: return _Fieldcolor;
            }
        }
        public void RefreshTable()
        {
            foreach (Field field in Fields)
            {
                field.Color = GetColor(field.X, field.Y);
                OnPropertyChanged(nameof(field.Color));
            }
            OnPropertyChanged("Time");
            OnPropertyChanged("All");
        }
        private void StepGame(int v)
        {
            if (_onGame)
            {
                Field field = Fields[v];
                _model.MovePlayer(field.X, field.Y);
                RefreshTable();
            }
        }
        private void GenerateFields()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Field field = new Field() 
                    {
                        Color = GetColor(i, j),
                        X = i,
                        Y = j,
                        Number= i * Size + j,
                        StepCommand = new DelegateCommand(p => StepGame(Convert.ToInt32(p))),
                    };
                    Fields.Add(field);
                }
            }
        }


        #endregion
    }
}
