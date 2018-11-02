using NetCommon.Codes;
using System;


namespace NetCommon
{
    [Serializable]
    public class ParameterObject
    {
        protected readonly NetParameterCode _operationCode;

        protected readonly Object _parameter;

        public NetParameterCode OperationCode { get { return _operationCode; } }

        public Object Parameter { get { return _parameter; } }

        public ParameterObject (NetParameterCode operationCode, Object parameter)
        {
            _operationCode = operationCode;
            _parameter = parameter;
        }
    }
}
