using Countdown.ValueImplementations;

namespace Countdown.GameController
{
    public class ExpressionInput<T> : Panel
    {
        private struct Optional<U>
        {
            public readonly U? Value;
            public readonly bool IsValue;

            public Optional(U input) 
            {
                Value = input;
                IsValue = true;
            }
            public Optional()
            { 
                Value = default;
                IsValue = false;
            }
            public override bool Equals(object? obj) => obj is not null && Value is not null && Value.Equals(obj);
            public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        }

        //If Optional<T> is null, show blank add factor display, otherwise display value/subequation
        private readonly ExpressionFactor<Optional<T>> _value;

        public ExpressionInput()
        {
            _value = new(new Optional<T>());
        }
    }
}
