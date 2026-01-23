namespace GZone.Service.Extensions.Exceptions
{
    public class ConflictException : ApplicationException
    {
        public ConflictException(string message) : base(message)
        {
            //409: Cannot complete because existed
        }
    }
}
