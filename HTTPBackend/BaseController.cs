using Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPBackend
{
    public abstract class BaseController
    {
        private readonly List<RequestAttribute> RequestMethods;

        protected HttpListenerResponse Response { get; private set; }
        protected ILogger Logger { get; private set; }

        protected BaseController(ILogger logger)
        {
            Logger = logger;
            var type = GetType();
            var controllerAttrib = type.GetCustomAttribute<ControllerAttribute>();
            RequestMethods = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod)
                .Select(method => (method.GetCustomAttribute<RequestAttribute>(), method))
                .Where(mm => mm.Item1 != null)
                .Select(mm => mm.Item1.UpdateRegex(mm.method, controllerAttrib, Logger))
                .ToList();
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            var methodType = (RequestMethodType)Enum.Parse(typeof(RequestMethodType), context.Request.HttpMethod);
            Response = context.Response;
            foreach (var request in RequestMethods)
            {
                if (request.MethodType != methodType) continue;
                if (request.InvokeMethod(this, context, Logger)) return true;
            }
            Response = null;
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class RequestAttribute : Attribute
    {
        public readonly RequestMethodType MethodType;
        public readonly string RequestUrl;
        private Dictionary<string, Type> paramTypes;
        private List<string> paramNames;
        private Regex regex;
        private MethodInfo methodInfo;
        private string bodyName;

        public RequestAttribute(RequestMethodType methodType, string requestUrl)
        {
            MethodType = methodType;
            RequestUrl = requestUrl;
        }

        internal RequestAttribute UpdateRegex(MethodInfo methodInfo, ControllerAttribute controller, ILogger logger)
        {
            var controllerPrefix = controller?.UrlPrefix ?? string.Empty;

            this.methodInfo = methodInfo;

            var methodParams = methodInfo.GetParameters();

            bodyName = methodParams.Where(m => m.GetCustomAttribute<RequestBodyAttribute>() != null).SingleOrDefault()?.Name;

            var parameters = methodParams.Select(p => (p.Name, p.ParameterType));

            paramNames = parameters.Select(p => p.Name).ToList();

            paramTypes = parameters.ToDictionary(p => p.Name, p => p.ParameterType);

            var regexString = new StringBuilder(controllerPrefix);
            regexString.Append(RequestUrl);
            foreach (var (BracketName, Name, ParameterType) in parameters.Select(p => (BracketName: $"{{{p.Name}}}", p.Name, p.ParameterType)))
            {
                regexString.Replace(BracketName, ParameterType.GetTypeRegex(Name));
            }
            regex = new Regex(regexString.ToString(), RegexOptions.Compiled);
            logger.OuterLevelWrite("HTTP", () =>
                logger.Log($"Registered endpoint: {MethodType};{regex}" + (string.IsNullOrEmpty(bodyName) ? "" : $" with body: {bodyName}"))
            );
            return this;
        }

        public bool InvokeMethod(object instance, HttpListenerContext context, ILogger logger)
        {
            var url = context.Request.RawUrl;
            var match = regex.Match(url);
            if (!match.Success) return false;
            logger.OuterLevelWrite("HTTP", () =>
                logger.Log($"Matching request on {regex}")
            );
            var paramValues = match
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => (g.Name, value: paramTypes[g.Name].GetTypeRegexValue(g.Value)));
            if (bodyName != null)
            {
                if (!context.Request.HasEntityBody) throw new Exception("No request body when required");
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    paramValues = paramValues.Append((bodyName, reader.ReadToEnd()));
                }
            }
            else if (context.Request.HasEntityBody) throw new Exception("Request body when not required");
            methodInfo.Invoke(instance,
                paramNames
                .Join(paramValues, p1 => p1, p2 => p2.Name, (p1, p2) => p2.value)
                .ToArray()
                );

            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class ControllerAttribute : Attribute
    {
        public readonly string UrlPrefix;
        public ControllerAttribute(string urlPrefix)
        {
            UrlPrefix = urlPrefix;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class RequestBodyAttribute : Attribute
    {

    }

    internal enum RequestMethodType
    {
        GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS
    }

    internal static class RequestTypeExtensions
    {
        public static string GetTypeRegex(this Type type, string name = "")
        {
            return $"(?<{name}>{GetTypeRegexGroup(type)})";
        }

        private static string GetTypeRegexGroup(Type type)
        {
            if (type == typeof(int) || type == typeof(long))
            {
                return "-?[0-9]+";
            }
            if (type == typeof(uint) || type == typeof(ulong))
            {
                return "[0-9]+";
            }
            if (type == typeof(string))
            {
                return ".*?";
            }
            return "";
        }

        public static object GetTypeRegexValue(this Type type, string value)
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(long)) return long.Parse(value);
            if (type == typeof(uint)) return uint.Parse(value);
            if (type == typeof(ulong)) return ulong.Parse(value);
            return value;
        }
    }
}