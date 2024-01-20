﻿using System.Runtime.Serialization;


namespace Indexer.Api.Exceptions;

[Serializable]
public abstract class BaseException : Exception, ISerializable
{
    protected BaseException() { }

    protected BaseException(string? message) : base(message) { }

    protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    [Obsolete]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }

    public abstract string Message { get; }
}