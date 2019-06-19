using AlphaGu2.Views;
using BullsnCows;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AlphaGu2.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public enum GameTurn
        {
            Computer,
            Player,
        }

        private GameTurn _turn;
        public GameTurn Turn
        {
            get => _turn;
            set
            {
                SetProperty(ref _turn, value);

                switch (Turn)
                {
                    case GameTurn.Computer:
                        _question = _game.GetQuestion();
                        Message = string.Format("{0},\n이게 당신의 암호인가요?", _question);
                        FaceKind = PackIconModernKind.SmileyWhat;
                        break;
                }
            }
        }

        private PackIconModernKind _faceKind;
        public PackIconModernKind FaceKind
        {
            get => _faceKind;
            set => SetProperty(ref _faceKind, value);
        }

        public string Number { get; private set; }

        readonly MainWindow _window;
        readonly Random _random;

        Game _game;

        string _question;

        public MainWindowViewModel()
        {
            _game = new Game(Game.Digit.Four);
            _random = new Random();
            _window = App.Current.MainWindow as MainWindow;

            AnswerCommand = new DelegateCommand(AnswerClick);
            GuessCommand = new DelegateCommand(GuessClick);

            for (int i = 0; i <= _game.Length; i++)
            {
                Strikes.Add(i);
                Balls.Add(i);
            }

            Initialize();
        }

        private void Initialize()
        {
            ComputerLog.Clear();
            PlayerLog.Clear();

            SelectedStrike = 0;
            SelectedBall = 0;

            Message = "당신이 질문할 차례입니다.";
            FaceKind = PackIconModernKind.SmileyGrin + _random.Next(2) * 2;
            Number = _game.AllowedNumbers.RandomValue();
            Guess = "";
            Turn = GameTurn.Computer + _random.Next(2);
        }

        private ObservableCollection<History> _computerLog;
        public ObservableCollection<History> ComputerLog
        {
            get => _computerLog ?? (_computerLog = new ObservableCollection<History>());
        }

        private ObservableCollection<History> _playerLog;
        public ObservableCollection<History> PlayerLog
        {
            get => _playerLog ?? (_playerLog = new ObservableCollection<History>());
        }

        private int _selectedStrike;
        public int SelectedStrike
        {
            get => _selectedStrike;
            set => SetProperty(ref _selectedStrike, value);
        }

        private ObservableCollection<int> strikes;
        public ObservableCollection<int> Strikes
        {
            get => strikes ?? (strikes = new ObservableCollection<int>());
        }

        private int _selectedBall;
        public int SelectedBall
        {
            get => _selectedBall;
            set => SetProperty(ref _selectedBall, value);
        }

        private ObservableCollection<int> _balls;
        public ObservableCollection<int> Balls
        {
            get { return _balls ?? (_balls = new ObservableCollection<int>()); }
        }

        public DelegateCommand AnswerCommand { get; }

        private async void AnswerClick()
        {
            if (Turn == GameTurn.Computer && SelectedStrike + SelectedBall <= _game?.Length)
            {
                int count = _game.AllowedNumbers.Count;

                ComputerLog.Add(new History(_question, new Answer(SelectedStrike, SelectedBall)));
                _game.PutAnswer(SelectedStrike, SelectedBall);

                if (SelectedStrike == _game.Length)
                {
                    switch (_random.Next(4))
                    {
                        case 0:
                            FaceKind = PackIconModernKind.SmileyGrin;
                            break;
                        case 1:
                            FaceKind = PackIconModernKind.SmileyHappy;
                            break;
                        case 2:
                            FaceKind = PackIconModernKind.SmileyKiki;
                            break;
                        case 3:
                            FaceKind = PackIconModernKind.SmileyTounge;
                            break;
                    }

                    Message = string.Format("제가 이겼습니다!\n저의 암호는 {0}입니다.", Number);
                    Repeat();
                    return;
                }

                if (_game.AllowedNumbers.Count == 0)
                {
                    FaceKind = PackIconModernKind.SmileyWhat;
                    Message = "조건에 맞는 암호가 존재하지 않습니다.\n당신이 생각한 암호는 무엇인가요?";

                    string number = await _window.ShowInputAsync("조건에 맞는 암호가 존재하지 않습니다.", "당신이 생각한 암호는 무엇인가요?");
                    CheckConsistent(number);
                    return;
                }

                double reduction = 1 - _game.AllowedNumbers.Count / (double)count;

                if (reduction >= 0.9)
                    FaceKind = PackIconModernKind.SmileyKiki;
                else if (reduction >= 0.8)
                    FaceKind = PackIconModernKind.SmileyGrin;
                else if (reduction >= 0.7)
                    FaceKind = PackIconModernKind.SmileyHappy;
                else if (reduction >= 0.5)
                    FaceKind = PackIconModernKind.SmileyGlasses;
                else if (reduction >= 0.4)
                    FaceKind = PackIconModernKind.SmileyWhat;
                else if (reduction >= 0.3)
                    FaceKind = PackIconModernKind.SmileyGrumpy;
                else if (reduction >= 0.2)
                    FaceKind = PackIconModernKind.SmileyCry;
                else
                    FaceKind = PackIconModernKind.SmileyFrown;

                Message = "그렇군요, 알겠습니다.";
                Turn = GameTurn.Player;
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _guess;
        public string Guess
        {
            get => _guess;
            set => SetProperty(ref _guess, value);
        }

        public DelegateCommand GuessCommand { get; }

        private async void GuessClick()
        {
            if (await _window.GetCurrentDialogAsync<BaseMetroDialog>() != null)
                return;

            if (Turn == GameTurn.Player && Guess?.Length == _game.Length && Guess.All(e => "1234567890".Contains(e)) && Guess.Distinct().Count() == 4)
            {
                PlayerLog.Add(new History(Guess, _game.GetAnswer(Number, Guess)));

                if (Number == Guess)
                {
                    switch (_random.Next(3))
                    {
                        case 0:
                            FaceKind = PackIconModernKind.SmileyCry;
                            break;
                        case 1:
                            FaceKind = PackIconModernKind.SmileyFrown;
                            break;
                        case 2:
                            FaceKind = PackIconModernKind.SmileyGrumpy;
                            break;
                    }

                    Message = "앗, 제 암호를 맞추셨군요...\n당신의 암호는 무엇인가요?";

                    string number = await _window.ShowInputAsync("앗, 제 암호를 맞추셨군요...", "당신의 암호는 무엇인가요?");
                    CheckConsistent(number);
                }
                else
                {
                    Turn = GameTurn.Computer;
                }

                Guess = "";
            }
        }

        private async void Repeat()
        {
            var result = await _window.ShowMessageAsync(Message, "한 번 더 하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });
            switch (result)
            {
                case MessageDialogResult.Affirmative:
                    _game = new Game((Game.Digit)_game.Length);
                    Initialize();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }

        private async void CheckConsistent(string number)
        {
            var result = ComputerLog.Where(e => !_game.IsConsistent(number, e)).ToList();

            if (result.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (History history in result)
                {
                    Answer answer = _game.GetAnswer(number, history.Question);
                    builder.AppendFormat("{0}: {1} -> {2}", history.Question, history.Answer, answer);
                    builder.AppendLine();
                }

                builder.AppendLine();
                builder.Append("안타깝지만 이번 게임은 제 승리가 되겠군요.");

                Message = "이런, 당신이 잘못 대답한 질문이 있네요.";

                await _window.ShowMessageAsync(Message, builder.ToString());

                Message = "안타깝지만 이번 게임은 제 승리가 되겠군요.";
            }
            else
            {
                Message = "와우, 저를 이기셨군요. 정말 대단하네요!";
            }

            Repeat();
        }
    }
}
