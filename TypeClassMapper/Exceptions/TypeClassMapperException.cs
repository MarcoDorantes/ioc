using System;
using System.Runtime.Serialization;

namespace nutility
{
  [Serializable]
  public class TypeClassMapperException : Exception
  {
    public TypeClassMapperException()
    {
    }

    public TypeClassMapperException(string message) : base(message)
    {
    }

    public TypeClassMapperException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TypeClassMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}