using Countdown.ValueImplementations;

//https://en.wikipedia.org/wiki/Countdown_(game_show)#Numbers_round

namespace Countdown
{
    public class ValueGenerator<T> where T : IStringRepresentable<T>
    {
        public static readonly int REPETITIONS = 1000;
        public static readonly VerifyEndState DEFAULT_VERIFICATION = (t) => true;
        public enum GenerationPhase { SELECTING, RANDOMIZING, EVALUATING, ERROR }

        public List<T> BigValues { get; private set; }
        public List<T> SmallValues { get; private set; }
        public List<Operation<T>> Operators { get; private set; }
        public int SelectionAmt { get; private set; }
        public int MinUse { get; private set; }
        public int MaxUse { get; private set; }

        private static readonly Random _rng;

        public List<T> Selected
        {
            get
            {
                if (State == GenerationPhase.SELECTING || State == GenerationPhase.ERROR)
                    throw new InvalidOperationException();

                return _selected;
            }
            private set => _selected = value;
        }
        private List<T> _selected;

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

        private readonly VerifyEndState _isValidEndState;

        public GenerationPhase State { get; private set; }

        public static ValueGenerator<IntValue> GetDefaultNumberGenerator()
        {
            return new ValueGenerator<IntValue>(
                new List<IntValue>() { 25, 50, 75, 100 },
                new List<IntValue>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                new List<Operation<IntValue>>()
                {
                    new Operation<IntValue>("+", 1, (a, b) => a + b, (a, b) => true),
                    new Operation<IntValue>("-", 1, (a, b) => a - b, (a, b) => a - b >= 0),
                    new Operation<IntValue>("*", 2, (a, b) => a * b, (a, b) => true),
                    new Operation<IntValue>("/", 2, (a, b) => a / b, (a, b) => b != 0 && a % b == 0)
                },
                6, 6, 6);
        }

        static ValueGenerator()
        {
            _rng = new Random();
        }
        private static void ShuffleValues(List<T> vals)
        {
            int n = vals.Count;

            while (n > 1)
            {
                int k = _rng.Next(n--);
                (vals[k], vals[n]) = (vals[n], vals[k]);
            }
        }

        public ValueGenerator(List<T> big, List<T> small, List<Operation<T>> operations, int selectionAmount, int minUse, int maxUse, VerifyEndState verifyEnd)
        {
            if (selectionAmount < 1 || selectionAmount > big.Count + small.Count)
                throw new ArgumentException("Selection amount is larger than number of possible selections, or is <= 0.");

            BigValues = new(big);
            SmallValues = new(small);
            Operators = operations;
            SelectionAmt = selectionAmount;
            MinUse = minUse;
            MaxUse = maxUse;
            _isValidEndState = verifyEnd;

            _selected = new();
            _steps = new();
            Reset();
        }
        public ValueGenerator(List<T> big, List<T> small, List<Operation<T>> operations, int selectionAmount, int minUse, int maxUse) : this(big, small, operations, selectionAmount, minUse, maxUse, DEFAULT_VERIFICATION)
        {

        }

        public void Reset()
        {
            ShuffleValues(BigValues);
            ShuffleValues(SmallValues);

            _selected = new List<T>();

            Goal = default;
            _steps = new(SelectionAmt - 1);
            State = GenerationPhase.SELECTING;
        }

        public void ChooseNumber(int position, bool isBig)
        {
            if (State == GenerationPhase.SELECTING)
            {
                //isBig makes position negative
                if (isBig)
                    _selected.Add(BigValues[position]);
                else
                    _selected.Add(SmallValues[position]);

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
                List<Operation<T>> outputSteps = new();
                LinkedList<T> options;

                T left, right;
                int operationPos, nextValue, errorCount;

                for (int trial = 0; trial < REPETITIONS; trial++)
                {
                    outputSteps.Clear();
                    options = new(_selected);

                    while (options.Count > 1)
                    {
                        errorCount = 0;
                        nextValue = _rng.Next(options.Count);
                        left = options.ElementAt(nextValue);
                        options.Remove(left);
                        nextValue = _rng.Next(options.Count);
                        right = options.ElementAt(nextValue);
                        options.Remove(right);

                        do
                        {
                            operationPos = _rng.Next(Operators.Count);
                            errorCount++;
                        }
                        while (errorCount < REPETITIONS && !Operators[operationPos].IsEvaluable(left, right));

                        if (errorCount >= REPETITIONS)
                            break;

                        var fixedEvaulation = Operators[operationPos].FixEvaluation(left, right);
                        outputSteps.Add(fixedEvaulation);

                        options.AddLast(fixedEvaulation.Result!);
                    }

                    if (options.Count == 1 && _isValidEndState(options.First()))
                    {
                        Goal = options.First();
                        _steps = outputSteps;
                        State = GenerationPhase.EVALUATING;
                        return;
                    }
                }

                State = GenerationPhase.ERROR;
            }
            else
                throw new InvalidOperationException();
        }

        public bool IsValidSolution(List<Operation<T>> forces)
        {
            if (State != GenerationPhase.EVALUATING)
                throw new InvalidOperationException();

            if (MinUse > forces.Count - 1 || forces.Count - 1 > MaxUse)
                return false;

            LinkedList<T> options = new(Selected);

            foreach(Operation<T> step in forces)
            {
                if (step.LeftValue is null || step.RightValue is null || step.Result is null)
                    return false;

                if (options.Contains(step.LeftValue) && options.Contains(step.RightValue) && step.IsEvaluable(step.LeftValue, step.RightValue) && step.Evaluate(step.LeftValue, step.RightValue)!.IsEquivalentTo(step.Result))
                {
                    options.Remove(step.LeftValue);
                    options.Remove(step.RightValue);
                    options.AddLast(step.Result);
                }
                else
                    return false;
            }

            return options.First()!.IsEquivalentTo(Goal);
        }

        public IReadOnlyList<Operation<T>> GetIntendedSolution()
        {
            if (State != GenerationPhase.EVALUATING)
                throw new InvalidOperationException();

            return _steps.AsReadOnly();
        }

        public bool FoundValidSolution()
        {
            if (State == GenerationPhase.EVALUATING)
                return true;
            if (State == GenerationPhase.ERROR)
                return false;

            throw new InvalidOperationException();
        }

        public delegate bool VerifyEndState(T val);
    }
}
