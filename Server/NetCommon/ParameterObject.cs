using NetCommon.Codes;
using System;
using System.Collections.Generic;


namespace NetCommon
{
    [Serializable]
    public class ParameterObject
    {
        protected readonly Dictionary<NetParameterCode, object> _parameters;

        protected readonly NetErrorCode _errorCode;

        public Dictionary<NetParameterCode, object> Parameters { get { return _parameters; } }

        public NetErrorCode ErrorCode { get { return _errorCode; } }

        public ParameterObject (Dictionary<NetParameterCode, object> parameters)
            : this(parameters, NetErrorCode.Success) { }

        public ParameterObject (Dictionary<NetParameterCode, object> parameters, NetErrorCode errorCode)
        {
            _parameters = parameters;
            _errorCode = errorCode;
        }
    }
}
