
namespace HeroServer
{
    public class LeapValueResult<T> : LeapResult
    {
        public T Value { get; set; }

        public LeapValueResult()
        {
        }

        public LeapValueResult(T value)
        {
            Value = value;
        }
    }
}