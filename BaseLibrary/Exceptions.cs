using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Exceptions
{

    [Serializable]
    public class TPRException : Exception
    {
        public TPRException() { }
        public TPRException(string message) : base(message) { }
        public TPRException(string message, Exception inner) : base(message, inner) { }
        protected TPRException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class LinkOutImageException : TPRException
    {
        private const string localMessage = "Ссылка на выходное изображение совпадает с ссылкой на входное изображение. Это может привести к непредвиденным ошибкам";

        /// <summary>
        /// Метод, с которым связано исключение
        /// </summary>
        public MethodInfo MethodInfoError { get; }

        public LinkOutImageException() : base(localMessage) { }

        public LinkOutImageException(MethodInfo methodInfo) : base($"{localMessage}{Environment.NewLine}Метод - {methodInfo.Name}") {
            MethodInfoError = methodInfo;
        }

        public LinkOutImageException(string message) : base(message) { }
        public LinkOutImageException(string message, Exception inner) : base(message, inner) { }
        protected LinkOutImageException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
