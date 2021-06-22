using System;

namespace PoC.AM.Abstract.Exceptions
{
    public class ProgrammersErrorException : Exception
    {
        public ProgrammersErrorException(string message) : base(message)
        {
        }
    }
}