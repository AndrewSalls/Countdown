using Countdown.ValueImplementations;

//https://en.wikipedia.org/wiki/Countdown_(game_show)#Numbers_round

namespace Countdown
{
    public class ValueGenerator<T>
    {
        public static readonly int REPETITIONS = 1000;
        public static readonly VerifyEndState DEFAULT_VERIFICATION = (t) => true;
        public enum GenerationPhase { SELECTING, RANDOMIZING, EVALUATING, ERROR }

        public List<T> BigValues { get; protected set; }
        public List<T> SmallValues { get; protected set; }
        public List<Operation<T>> Operators { get; private set; }
        public int MinUse { get; private set; }
        public int MaxUse { get; private set; }

        protected static readonly Random _rng;

        public List<T> Selected
        {
            get
            {
                if (State == GenerationPhase.SELECTING || State == GenerationPhase.ERROR)
                    throw new InvalidOperationException();

                return _selected.Select(t => t.Value).ToList();
            }
            private set => _selected = value.Select(t => new ExpressionFactor<T>(t)).ToList();
        }
        private List<ExpressionFactor<T>> _selected;

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

        private List<Expression<T>> _steps;

        private readonly VerifyEndState _isValidEndState;

        public GenerationPhase State { get; private set; }

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

        public ValueGenerator(List<T> big, List<T> small, List<Operation<T>> operations, int minUse, int maxUse, VerifyEndState verifyEnd)
        {
            if (minUse < 2 || maxUse > big.Count + small.Count || maxUse < minUse)
                throw new ArgumentException("Selection amount is larger than number of possible selections, or is < 2.");

            BigValues = new(big);
            SmallValues = new(small);
            Operators = operations;
            MinUse = minUse;
            MaxUse = maxUse;
            _isValidEndState = verifyEnd;

            _selected = new();
            _steps = new();
            Reset();
        }
        public ValueGenerator(List<T> big, List<T> small, List<Operation<T>> operations, int minUse, int maxUse) : this(big, small, operations, minUse, maxUse, DEFAULT_VERIFICATION) { }

        public void Reset()
        {
            ShuffleValues(BigValues);
            ShuffleValues(SmallValues);

            _selected = new List<ExpressionFactor<T>>();

            Goal = default;
            _steps = new(MaxUse - 1);
            State = GenerationPhase.SELECTING;
        }

        public void ChooseNumber(int position, bool isBig)
        {
            if (State == GenerationPhase.SELECTING)
            {
                //isBig makes position negative
                if (isBig)
                    _selected.Add(new ExpressionFactor<T>(BigValues[position]));
                else
                    _selected.Add(new ExpressionFactor<T>(SmallValues[position]));

                if (_selected.Count >= MaxUse)
                    State = GenerationPhase.RANDOMIZING;
            }
            else
                throw new InvalidOperationException();
        }

        public void RandomizeGoal()
        {
            if (State == GenerationPhase.RANDOMIZING || State == GenerationPhase.EVALUATING)
            {
                List<Expression<T>> outputSteps = new();
                LinkedList<ExpressionFactor<T>> options;

                ExpressionFactor<T> left, right;
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
                        while (errorCount < REPETITIONS && !Operators[operationPos].IsEvaluable(left.Value, right.Value));

                        if (errorCount >= REPETITIONS)
                            break;

                        var expressionStep = new Expression<T>(Operators[operationPos], left, right);
                        outputSteps.Add(expressionStep);

                        options.AddLast(new ExpressionFactor<T>(expressionStep));
                    }

                    if (options.Count == 1 && _isValidEndState(options.First()!.Value))
                    {
                        Goal = options.First()!.Value;
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

        public bool IsValidSolution(List<Expression<T>> steps)
        {
            if (State != GenerationPhase.EVALUATING)
                throw new InvalidOperationException();

            if (MinUse > steps.Count - 1 || steps.Count - 1 > MaxUse)
                return false;

            LinkedList<T> options = new(Selected);

            foreach(Expression<T> step in steps)
            {
                if (step.Left is null || step.Right is null || step.Result is null)
                    return false;

                if (options.Contains(step.Left.Value) && options.Contains(step.Right.Value) && step.Operation.IsEvaluable(step.Left.Value, step.Right.Value) && step.Operation.Evaluate(step.Left.Value, step.Right.Value)!.Equals(step.Result))
                {
                    options.Remove(step.Left.Value);
                    options.Remove(step.Right.Value);
                    options.AddLast(step.Result);
                }
                else
                    return false;
            }

            return options.First()!.Equals(Goal);
        }

        public IReadOnlyList<Expression<T>> GetIntendedSolution()
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
