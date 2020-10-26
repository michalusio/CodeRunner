using Backend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPBackend
{
    public abstract class BaseController : IMiddleware
    {
        private readonly List<RequestAttribute> RequestMethods;
        private RequestAttribute Request;

        protected HttpListenerResponse Response { get; private set; }
        protected ILogger Logger { get; private set; }
        public IMiddleware Next { private get; set; }

        protected BaseController(ILogger logger)
        {
            Logger = logger;
            var type = GetType();
            var controllerAttrib = type.GetCustomAttribute<ControllerAttribute>();
            RequestMethods = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod)
                .Select(method => (method.GetCustomAttribute<RequestAttribute>(), method))
                .Where(mm => mm.Item1 != null)
                .Select(mm => mm.Item1.SetMiddlewares(mm.method, this).UpdateRegex(mm.method, controllerAttrib, Logger))
                .ToList();
        }

        internal bool RunController(HttpListenerContext context)
        {
            var methodType = (RequestMethodType)Enum.Parse(typeof(RequestMethodType), context.Request.HttpMethod);
            Response = context.Response;
            try
            {
                foreach (var request in RequestMethods)
                {
                    if (request.MethodType != methodType) continue;
                    if (request.MatchMethod(context, Logger))
                    {
                        Request = request;
                        request.RunMiddlewares(context);
                        Response.Close();
                        return true;
                    }
                }
            }
            finally
            {
                Request = null;
                Response = null;
            }
            return false;
        }

        public void ResolveRequest(HttpListenerContext context)
        {
            Request.InvokeMethod(this, context);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RequestAttribute : Attribute
    {
        internal readonly RequestMethodType MethodType;
        internal readonly string RequestUrl;
        private List<IMiddleware> MiddlewareStack;
        private Dictionary<string, Type> paramTypes;
        private List<string> paramNames;
        private MethodInfo methodInfo;
        private Regex regex;
        private (string Name, Type Type) body;

        public RequestAttribute(RequestMethodType methodType, string requestUrl = "")
        {
            MethodType = methodType;
            RequestUrl = requestUrl;
        }

        internal RequestAttribute UpdateRegex(MethodInfo methodInfo, ControllerAttribute controller, ILogger logger)
        {
            var controllerPrefix = controller?.UrlPrefix ?? string.Empty;

            var methodParams = methodInfo.GetParameters();

            var bodyParameter = methodParams.Where(m => m.GetCustomAttribute<RequestBodyAttribute>() != null).SingleOrDefault();
            if (bodyParameter != null)
            {
                body = (bodyParameter.Name, bodyParameter.ParameterType);
            }

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
                logger.Log($"Registered endpoint: {MethodType} | {regex}" + ((body.Name != null ? $" with body: {body.Name}" : "") + $" [{MiddlewareStack.Count} Middlewares]"))
            );
            return this;
        }

        internal RequestAttribute SetMiddlewares(MethodInfo methodInfo, BaseController controller)
        {
            this.methodInfo = methodInfo;
            MiddlewareStack = HTTPService.GetMiddlewareStack().Concat(methodInfo.GetCustomAttributes<AttributeMiddleware>()).ToList();
            MiddlewareStack.Add(controller);

            var methodAttributes = methodInfo.GetCustomAttributes();

            for (int i = 0; i < MiddlewareStack.Count - 1; i++)
            {
                var middleware = MiddlewareStack[i];
                middleware.Next = MiddlewareStack[i + 1];
                if (middleware is BaseMiddleware bm)
                {
                    bm.ControllerAttributes = methodAttributes;
                }
                else if (middleware is AttributeMiddleware am)
                {
                    am.ControllerAttributes = methodAttributes;
                }
            }

            return this;
        }

        internal bool MatchMethod(HttpListenerContext context, ILogger logger)
        {
            var url = context.Request.RawUrl;
            var match = regex.Match(url);
            if (!match.Success) return false;
            logger.OuterLevelWrite("HTTP", () =>
                logger.Log($"Matching request on {regex}")
            );
            return true;
        }

        internal void RunMiddlewares(HttpListenerContext context)
        {
            MiddlewareStack.First().ResolveRequest(context);
        }

        internal bool InvokeMethod(object instance, HttpListenerContext context)
        {
            var url = context.Request.RawUrl;
            var match = regex.Match(url);
            if (!match.Success) return false;
            var paramValues = match
                .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => (g.Name, value: paramTypes[g.Name].GetTypeRegexValue(g.Value)));
            if (body.Name != null)
            {
                if (!context.Request.HasEntityBody) throw new Exception("No request body when required");
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    paramValues = paramValues.Append((body.Name, body.Type.GetTypeRegexValue(reader.ReadToEnd())));
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
    public sealed class ControllerAttribute : Attribute
    {
        internal readonly string UrlPrefix;
        public ControllerAttribute(string urlPrefix)
        {
            UrlPrefix = urlPrefix;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class RequestBodyAttribute : Attribute
    {

    }

    public enum RequestMethodType
    {
        GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS
    }

    internal static class RequestTypeExtensions
    {
        internal static string GetTypeRegex(this Type type, string name = "")
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
            if (type == typeof(float) || type == typeof(double))
            {
                return @"-?[0-9]+(?:\.[0-9]+)?";
            }
            if (type == typeof(string))
            {
                return ".*?";
            }
            return "{.*?}";
        }

        internal static object GetTypeRegexValue(this Type type, string value)
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(long)) return long.Parse(value);
            if (type == typeof(uint)) return uint.Parse(value);
            if (type == typeof(ulong)) return ulong.Parse(value);
            if (type == typeof(float)) return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            if (type == typeof(double)) return double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            if (type == typeof(string)) return value.Normalize();
            return JsonConvert.DeserializeObject(value, type);
        }
    }

}