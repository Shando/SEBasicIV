using System;

namespace SEBasicIV
{
    [Serializable]
    public class SyntaxErr : Exception
    {
        public SyntaxErr() : base() { }
        public SyntaxErr(string message) : base(message) { }
        public SyntaxErr(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected SyntaxErr(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

