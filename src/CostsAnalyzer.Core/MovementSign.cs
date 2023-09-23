namespace CostsAnalyzer.Core
{
    public enum MovementSign { Plus, Minus, Transfer }

    internal class MovementSignExt
    {
        static MovementSign GetMovementSign(decimal amount, string description)
        {
            MovementSign sign = MovementSign.Minus;

            if (amount < 0)
            {
                sign = MovementSign.Plus;
            }

            foreach (string s in new[] { "N26", "Hype" })
            {
                if (description.Contains(s, StringComparison.OrdinalIgnoreCase))
                {
                    sign = MovementSign.Transfer;
                    break;
                }
            }

            return sign;
        }
    }
}
