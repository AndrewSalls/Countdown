namespace Countdown
{
    //Have to make it so that values can be stored eg. 1 + 2 = 3, 4 + 5 = 9, 3 * 9 = 27
    public class ValueGenerator<T>
    {
        public static readonly int REPETITIONS = 1000;
        //https://en.wikipedia.org/wiki/Countdown_(game_show)#Numbers_round
        public enum GenerationPhase { SELECTING, RANDOMIZING, EVALUATING, ERROR }

        public List<T> BigNumbers { get; private set; }
        public List<T> SmallNumbers { get; private set; }
        public List<Operation<T>> Operators { get; private set; }
        public int SelectionAmt { get; private set; }

        private readonly Random _rng;

        public List<int> Selected
        {
            get
            {
                if (State == GenerationPhase.SELECTING || State == GenerationPhase.ERROR)
                    throw new InvalidOperationException();

                return _selected;
            }
            private set => _selected = value;
        }
        private List<int> _selected;

        public T? Goal
        {
            get
            {
                if (State != GenerationPhase.EVALUATING)
                    throw new InvalidOperationException();

                return _goal;
            }
            private set => _goal = value;
        }
        private T? _goal;

        private List<Operation<T>> _steps;
        private List<T> _stepValues;

        public GenerationPhase State { get; private set; }

        public static ValueGenerator<int> GetDefaultNumberGenerator()
        {
            return new ValueGenerator<int>(
                new List<int>() { 25, 50, 75, 100 },
                new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                new List<Operation<int>>()
                {
                    new Operation<int>("+", (a, b) => a + b, (a, b) => true),
                    new Operation<int>("-", (a, b) => a - b, (a, b) => a - b >= 0),
                    new Operation<int>("*", (a, b) => a * b, (a, b) => true),
                    new Operation<int>("/", (a, b) => a / b, (a, b) => b != 0 && a % b == 0)
                },
                6);
        }
        public ValueGenerator(List<T> big, List<T> small, List<Operation<T>> operations, int selectionAmount)
        {
            if (selectionAmount < 1 || selectionAmount > big.Count + small.Count)
                throw new ArgumentException("Selection amount is larger than number of possible selections, or is <= 0.");

            _rng = new Random();

            BigNumbers = big;
            SmallNumbers = small;
            //TODO: Randomize order of elements from big & small

            Operators = operations;
            _selected = new List<int>();
            SelectionAmt = selectionAmount;

            Goal = default;
            _steps = new(SelectionAmt - 1);
            _stepValues = new(SelectionAmt);
            State = GenerationPhase.SELECTING;
        }

        public void ChooseNumber(int position, bool isBig)
        {
            if (State == GenerationPhase.SELECTING)
            {
                //isBig makes position negative
                if (isBig)
                {
                    if (!_selected.Contains(-position))
                        _selected.Add(-position);
                }
                else
                {
                    if (!_selected.Contains(position))
                        _selected.Add(position);
                }

                if (_selected.Count >= SelectionAmt)
                    State = GenerationPhase.RANDOMIZING;
            }
            else
                throw new InvalidOperationException();
        }

        public void RandomizeGoal()
        {
            if (State == GenerationPhase.RANDOMIZING || State == GenerationPhase.EVALUATING)
            {
                List<int> unused;
                List<T> outputStepValues;
                List<Operation<T>> outputSteps;
                T output;
                int nextValue, nextOperation, overError;

                for (int trial = 0; trial < REPETITIONS; trial++)
                {
                    unused = new(Selected);
                    outputSteps = new(unused.Count - 1);
                    outputStepValues = new(unused.Count);
                    nextValue = _rng.Next(unused.Count);
                    output = GetNumberAt(unused[nextValue]);
                    unused.RemoveAt(nextValue);

                    outputStepValues.Add(output);

                    while (unused.Count > 0)
                    {
                        overError = 0;
                        nextValue = _rng.Next(unused.Count);
                        do
                        {
                            nextOperation = _rng.Next(Operators.Count);
                            if (overError >= REPETITIONS)
                                break;
                            overError++;
                        }
                        while (!Operators[nextOperation].IsEvaluable(output, GetNumberAt(unused[nextValue])));

                        if (overError >= REPETITIONS)
                            break;

                        outputSteps.Add(Operators[nextOperation]);
                        outputStepValues.Add(GetNumberAt(unused[nextValue]));
                        output = Operators[nextOperation].Evaluate(output, GetNumberAt(unused[nextValue]));
                        unused.RemoveAt(nextValue);
                    }

                    if (unused.Count == 0)
                    {
                        Goal = output;
                        _steps = outputSteps;
                        _stepValues = outputStepValues;
                        State = GenerationPhase.EVALUATING;
                        return;
                    }
                }

                State = GenerationPhase.ERROR;
            }
            else
                throw new InvalidOperationException();
        }

        private T GetNumberAt(int pos) => pos < 0 ? BigNumbers[-pos] : SmallNumbers[pos];

        public bool IsValidSolution(List<T> values, List<Operation<T>> forces)
        {
            if (State != GenerationPhase.EVALUATING)
                throw new InvalidOperationException();

            T val = values[0];

            for (int i = 1; i < values.Count; i++)
            {
                if (!forces[i].IsEvaluable(val, values[i]))
                    return false;

                val = forces[i - 1].Evaluate(val, values[i]);
            }

            if (val == null)
                return Goal == null;

            return val.Equals(Goal);
        }

        public void GetIntendedSolution(out List<T> stepValues, out List<Operation<T>> steps)
        {
            if (State != GenerationPhase.EVALUATING)
                throw new InvalidOperationException();

            stepValues = new(_stepValues);
            steps = new(_steps);
        }

        public bool FoundValidSolution()
        {
            if (State == GenerationPhase.EVALUATING)
                return true;
            if (State == GenerationPhase.ERROR)
                return false;

            throw new InvalidOperationException();
        }
    }
}
