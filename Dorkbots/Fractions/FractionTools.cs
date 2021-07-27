namespace Dorkbots.Fractions
{
    public class FractionTools
    {
        public static Fraction CreateFraction(FractionValues fractionValues)
        {
            return new Fraction(fractionValues.numerator, fractionValues.denominator);
        }
    }

    [System.Serializable]
    public struct FractionValues
    {
        public uint numerator;
        public uint denominator;

        public FractionValues(uint numerator, uint denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }
    }
}