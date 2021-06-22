using System;
using PoC.AM.Abstract.Model;

namespace PoC.AM.Abstract.Exceptions
{
    public class SubRequestException : Exception
    {
        public SubRequestException(SubRequest subRequest) : base(subRequest.ToString())
        {
        }
    }
}