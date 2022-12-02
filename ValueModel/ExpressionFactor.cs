namespace Countdown.ValueImplementations
{
    public class ExpressionFactor<T>
    {
        public bool IsValue { get { return _subEquation == null; } }
        public T Value {
            get
            {
                if(IsValue)
                    return _value!;

                return _subEquation!.Result;
            }
            set
            {
                _value = value;
                _subEquation = null;
            }
        }
        private T? _value;
        public Expression<T> SubEquation { get { return _subEquation!; } set { _subEquation = value; _value = default; } }
        private Expression<T>? _subEquation;

        public ExpressionFactor(T val)
        {
            _value = val;
            _subEquation = null;
        }
        public ExpressionFactor(Expression<T> sub)
        {
            _value = default;
            _subEquation = sub;
        }
    }
}
