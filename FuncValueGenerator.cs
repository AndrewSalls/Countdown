using Countdown.ValueImplementations;
using Countdown.ValueImplementations.Values;

namespace Countdown
{
    public class FuncValueGenerator<T> : ValueGenerator<T> where T : IStringRepresentable<T>
    {
        private readonly GenFunc _smallGen;
        private readonly GenFunc _bigGen;
        private readonly int _smallGenAmt;
        private readonly int _bigGenAmt;

        public FuncValueGenerator(int smallGenAmt, int bigGenAmt, List<Operation<T>> operations, int minUse, int maxUse, VerifyEndState verifyEnd, GenFunc generateSmall, GenFunc generatebig)
            : base(GenValuesUsing(generateSmall, smallGenAmt), GenValuesUsing(generatebig, bigGenAmt), operations, minUse, maxUse, verifyEnd)
        {
            _smallGen = generateSmall;
            _bigGen = generatebig;
            _smallGenAmt = smallGenAmt;
            _bigGenAmt = bigGenAmt;
        }

        public new void Reset()
        {
            SmallValues = GenValuesUsing(_smallGen, _smallGenAmt);
            BigValues = GenValuesUsing(_bigGen, _bigGenAmt);
        }

        private static List<T> GenValuesUsing(GenFunc generate, int amt) => Enumerable.Range(0, amt).Select(i => generate(_rng)).ToList();

        public delegate T GenFunc(Random rng);
    }
}
