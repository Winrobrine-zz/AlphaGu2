using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BullsnCows
{
    public class Game
    {
        public enum Digit
        {
            Three = 3,
            Four = 4,
        }

        public int Length { get; }
        public List<string> AllowedNumbers { get; }

        List<History> _history = new List<History>();

        int _step;
        string _lastQuestion;

        readonly Answer[] POTENTIAL_ANSWERS;

        public Game() : this(Digit.Four) { }
        public Game(Digit digit)
        {
            Length = (int)digit;
            AllowedNumbers = Permutation.Create("1234567890", Length).ToList();

            switch (digit)
            {
                case Digit.Three:
                    POTENTIAL_ANSWERS = new Answer[]
                    {
                        new Answer(0, 0),
                        new Answer(0, 1),
                        new Answer(0, 2),
                        new Answer(0, 3),
                        new Answer(1, 0),
                        new Answer(1, 1),
                        new Answer(1, 2),
                        new Answer(2, 0),
                        new Answer(3, 0),
                    };
                    break;
                case Digit.Four:
                    POTENTIAL_ANSWERS = new Answer[]
                    {
                        new Answer(0, 0),
                        new Answer(0, 1),
                        new Answer(0, 2),
                        new Answer(0, 3),
                        new Answer(0, 4),
                        new Answer(1, 0),
                        new Answer(1, 1),
                        new Answer(1, 2),
                        new Answer(1, 3),
                        new Answer(2, 0),
                        new Answer(2, 1),
                        new Answer(2, 2),
                        new Answer(3, 0),
                        new Answer(4, 0),
                    };
                    break;
            }
        }

        public Answer GetAnswer(string number, string question)
        {
            int bulls = 0, cows = 0;

            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] == question[i])
                    bulls++;
                else if (number.Contains(question[i]))
                    cows++;
            }

            return new Answer(bulls, cows);
        }

        public bool IsConsistent(string number, History history)
        {
            Answer answer = GetAnswer(number, history.Question);
            return answer.Bulls == history.Answer.Bulls && answer.Cows == history.Answer.Cows;
        }

        public bool IsFinished()
        {
            return AllowedNumbers.Count <= 1;
        }

        public string GetQuestion()
        {
            if (_step++ == 0)
                return _lastQuestion = AllowedNumbers.RandomValue();
            return _lastQuestion = AllowedNumbers.Shuffle().AsParallel().MinBy(question => POTENTIAL_ANSWERS.AsParallel().Sum(answer => Math.Pow(AllowedNumbers.AsParallel().Count(e => IsConsistent(e, new History(question, answer))), 2)));
        }

        public void PutAnswer(int bulls, int cows)
        {
            PutAnswer(new Answer(bulls, cows));
        }

        public void PutAnswer(Answer answer)
        {
            History history = new History(_lastQuestion, answer);
            _history.Add(history);
            AllowedNumbers.RemoveAll(e => !IsConsistent(e, history));
        }

        public int GetStep()
        {
            if (IsFinished())
            {
                if (_history.Last().Answer.Bulls == Length)
                    return _step;
                return _step + 1;
            }
            return _step;
        }

        public bool IsCorrect(out string result)
        {
            result = AllowedNumbers.FirstOrDefault(e => IsConsistent(e, _history.Last()));
            return result != null;
        }
    }
}