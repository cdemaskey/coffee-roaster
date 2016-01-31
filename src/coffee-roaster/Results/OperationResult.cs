namespace CoffeeRoaster.Results
{
    public class OperationResult
    {
        public bool Successful { get; private set; }

        public OperationResult(bool successful)
        {
            this.Successful = successful;
        }
    }
}