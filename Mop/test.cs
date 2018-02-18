using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;

using Unidos.Client.Interface;


using Castle.DynamicProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Unidos.Client.SAL
{
    public class RestProxyInterceptor : IInterceptor
    {
        private const string CONTENT_TYPE = "application/json";
        private const string TOKEN_HEADER = "Token";
        private const string ACCEPT_LANGUAGE_HEADER = "Accept-Language";
        private const string ENGLISH_LANGUAGE = "en-US";
        private const string SPANISH_LANGUAGE = "es-MX";
        private const string CLINET_REST_TIMEOUT = "ClientRestTimeout";
        private const string ASP_NET_SESSION_ID = "ASP.NET_SessionId";
        private const string FRONTEND_SESSION_ID = "Frontend_SessionId";
        private const string CLIENT_IP_ADDRESS = "ClientIpAddress";
        private const string CLIENT_PLATFORM = "ClientPlatform";
        private const string VALIDATION_CODE = "Validation-Code";
        private const string CLIENT_BROWSER_FAMILY = "ClientBrowserFamily";
        private const string EQUIPMENT_ID = "Equipment-Id";
        private const string GEO_POSITION = "Geo-Position";
        private const string GEO_ADDRESS = "Geo-Address";
        private const string UNIDOS_APP = "Unidos-App";

        private static string _baseUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private static CultureInfo _currentCultureInfo = new CultureInfo("en-US");
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Objects,
            Culture = _currentCultureInfo
        };

        private readonly IToken _token;
        private readonly ILoggerHelper _loggerHelper;

        public RestProxyInterceptor(IToken token, ILoggerHelper loggerHelper)
        {
            _token = token;
            _loggerHelper = loggerHelper;

            _baseUrl = _baseUrl.EndsWith("/") ? _baseUrl : _baseUrl + "/";
        }

        public void Intercept(IInvocation invocation)
        {

            var client = new RestClient(_baseUrl);
            client.Timeout = int.Parse(ConfigurationManager.AppSettings[CLINET_REST_TIMEOUT]);

            var methodInfo = invocation.Method;
            var route = methodInfo.GetCustomAttributes<RouteAttribute>().First().Template;
            var method = GetHttpMethod(methodInfo);
            var restRequest = new RestRequest(route, method);

            // adds "Token" to header
            var url = _baseUrl + route;
            if (!IsLogin(url) && !IsExchangeRates(url) && !IsRegister(url) && !IsRegisterPending(url) && !IsAnonymous(url))
            {
                restRequest.AddHeader(TOKEN_HEADER, _token.GetToken());
            }

            // adds "Accept-Language" to header
            string uiLanguage = Thread.CurrentThread.CurrentUICulture.Name;
            if (uiLanguage.Equals(SPANISH_LANGUAGE))
            {
                restRequest.AddHeader(ACCEPT_LANGUAGE_HEADER, uiLanguage);
            }
            else
            {
                restRequest.AddHeader(ACCEPT_LANGUAGE_HEADER, ENGLISH_LANGUAGE);
            }

            // adds client ip address to header.
            restRequest.AddHeader(CLIENT_IP_ADDRESS, HttpContext.Current.Request.UserHostAddress);
            
            // adds validation code to heander.
            SetValidationCode(restRequest);

            // adds j1kpme4dvag1dt0tmjbx0oeh to ASP.NET_SessionId cookie. POS backend's session id will be hardcoded.
            restRequest.AddCookie(ASP_NET_SESSION_ID, HttpContext.Current.Session.SessionID);

            var parameters = methodInfo.GetParameters();

            // handles the segment parameters in url, i.e. {controller}/{id}, the id is a segment parameter 
            var segmentCount = route.Count(c => c == '{');
            for (var i = 0; i < segmentCount; i++)
            {
                restRequest.AddUrlSegment(parameters[i].Name, invocation.Arguments[i].ToString());
            }

            // handles the parameters except segment parameters
            for (var i = segmentCount; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];
                var argument = invocation.Arguments[i];
                var argumentType = argument.GetType();
                if ((argumentType.IsPrimitive || argumentType.IsValueType || argumentType == typeof(string)))
                {
                    AddSimpleParameter(restRequest, parameter, argument, argumentType);
                }
                else if (argumentType.IsArray)
                {
                    AddArrayParameter(restRequest, parameter, argument, argumentType);
                }
                else
                {
                    AddObjectParameter(restRequest, parameter, argument, argumentType);
                }
            }

            var methodName = methodInfo.ReflectedType.FullName + "." + methodInfo.Name;
            _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Request, LogWebRequestType.Exec);

            // handles the response
            var response = client.Execute(restRequest);

            _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Completed, LogWebRequestType.Exec);

            int responseCode = (int)response.StatusCode;
            if (responseCode == 200)
            {
                var returnValue = JsonConvert.DeserializeObject(response.Content, methodInfo.ReturnType, _jsonSerializerSettings);
                invocation.ReturnValue = returnValue;
            }
            else if (responseCode != 204)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Error, LogWebRequestType.Exec, response.ErrorMessage);

                    throw new Exception(response.ErrorMessage);
                }
                else
                {
                    _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Error, LogWebRequestType.Exec, response.Content);

                    throw new WebException(response.Content);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Error, LogWebRequestType.Exec, response.ErrorMessage);
                }
                else
                {
                    _loggerHelper.LogWebRequest(methodName, LogWebRequestState.Error, LogWebRequestType.Exec, response.Content);
                }
            }
        }

        private void AddSimpleParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
        {
            if (parameter.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                var jsonObj = JsonConvert.SerializeObject(argument, _jsonSerializerSettings);
                restRequest.AddParameter(CONTENT_TYPE, jsonObj, ParameterType.RequestBody);
            }
            else
            {
                argument = argument.GetType() == typeof(DateTime) ?
                            ((DateTime)argument).ToString(_currentCultureInfo) :
                            argument.ToString();
                restRequest.AddParameter(parameter.Name, argument, ParameterType.QueryString);
            }
        }

        private void AddArrayParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
        {
            if (parameter.GetCustomAttribute<FromUriAttribute>() != null)
            {
                var elementType = argumentType.GetElementType();

                if (elementType.IsPrimitive || elementType.IsValueType || elementType == typeof(string))
                {
                    foreach (var item in (Array)argument)
                    {
                        restRequest.AddParameter(parameter.Name, item, ParameterType.QueryString);
                    }
                }
                else
                {
                    foreach (var item in (Array)argument)
                    {
                        var properties = item.GetType().GetProperties();

                        var value = string.Join(",", properties.Select(x => x.GetValue(item)));

                        restRequest.AddParameter(parameter.Name, value, ParameterType.QueryString);
                    }
                }
            }
            else
            {
                var jsonObj = JsonConvert.SerializeObject(argument, _jsonSerializerSettings);
                restRequest.AddParameter(CONTENT_TYPE, jsonObj, ParameterType.RequestBody);
            }
        }

        private void AddObjectParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
        {
            if (parameter.GetCustomAttribute<FromUriAttribute>() != null)
            {
                var properties = argumentType.GetProperties();
                foreach (var property in properties)
                {
                    var propType = property.PropertyType;
                    var value = property.GetValue(argument, null);

                    if (value != null)
                    {
                        value = value.GetType() == typeof(DateTime) ?
                            ((DateTime)value).ToString(_currentCultureInfo) :
                            value.ToString();
                        restRequest.AddParameter(property.Name, value, ParameterType.QueryString);
                    }
                }
            }
            else
            {
                var jsonObj = JsonConvert.SerializeObject(argument, _jsonSerializerSettings);
                restRequest.AddParameter(CONTENT_TYPE, jsonObj, ParameterType.RequestBody);
            }
        }

        private Method GetHttpMethod(MethodInfo methodInfo)
        {
            Method method;
            if (methodInfo.Name.ToLower().StartsWith("get"))
            {
                method = Method.GET;
            }
            else if (methodInfo.Name.ToLower().StartsWith("post"))
            {
                method = Method.POST;
            }
            else if (methodInfo.Name.ToLower().StartsWith("put"))
            {
                method = Method.PUT;
            }
            else
            {
                method = Method.DELETE;
            }
            return method;
        }

        private static bool IsLogin(string url)
        {
            if (url != null)
            {
                string[] urlStr = url.Split('/');
                if (urlStr.Length > 0 && (("Login").Equals(urlStr[urlStr.Length - 1]) || ("BusinessLogin").Equals(urlStr[urlStr.Length - 1])
                    || ("AdminUtilityLogin").Equals(urlStr[urlStr.Length - 1]) || ("AdminLogin").Equals(urlStr[urlStr.Length - 1])))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsExchangeRates(string url)
        {
            if (url != null)
            {
                string[] urlStr = url.Split('/');
                if (urlStr.Length > 0 && ("ExchangeRates").Equals(urlStr[urlStr.Length - 1]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAnonymous(string url)
        {
            if (url != null)
            {
                if (url.Contains("Anonymous"))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsRegister(string url)
        {
            if (url != null)
            {
                string[] urlStr = url.Split('/');
                if (urlStr.Length > 0 && ("BusinessEquipments").Equals(urlStr[urlStr.Length - 1]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsRegisterPending(string url)
        {
            if (url != null)
            {
                string[] urlStr = url.Split('/');
                if (urlStr.Length > 1 && ("Pending").Equals(urlStr[urlStr.Length - 1]) && ("BusinessEquipments").Equals(urlStr[urlStr.Length - 2]))
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetValidationCode(RestRequest restRequest)
        {
            var validationCodeKey = HttpContext.Current.Session[VALIDATION_CODE];

            var unidosApp = HttpContext.Current.Request.Headers["Unidos-App"];

            if (!string.IsNullOrEmpty(unidosApp))
            {
                restRequest.AddHeader(UNIDOS_APP, unidosApp);
            }

            string emailAddress = string.Empty;

            if (validationCodeKey != null)
            {
                emailAddress = validationCodeKey.ToString();

                if (!string.IsNullOrEmpty(emailAddress))
                {
                    var cookie = HttpContext.Current.Request.Cookies["ValidationCode_" + emailAddress];
                    if (cookie != null)
                    {
                        restRequest.AddHeader(VALIDATION_CODE, cookie.Value);
                    }
                }
            }

            System.Web.HttpCookie equipmentIdCookie = null;

            if (!string.IsNullOrEmpty(emailAddress))
            {
                equipmentIdCookie = HttpContext.Current.Request.Cookies[string.Format("EquipmentId_{0}", emailAddress)];
            }

            var equipmentNewIdCookie = HttpContext.Current.Request.Cookies["EquipmentId"];

            if (equipmentIdCookie != null || equipmentNewIdCookie != null)
            {
                restRequest.AddHeader(EQUIPMENT_ID, equipmentNewIdCookie != null ? equipmentNewIdCookie.Value : equipmentIdCookie.Value);
            }

            var geoPositionCookie = HttpContext.Current.Request.Cookies["Device_Geo_Position"];

            if (geoPositionCookie != null)
            {
                restRequest.AddHeader(GEO_POSITION, geoPositionCookie.Value);
            }

            var geoAddressCookie = HttpContext.Current.Request.Cookies["Device_Geo_Position_Address"];

            if (geoAddressCookie != null)
            {
                restRequest.AddHeader(GEO_ADDRESS, geoAddressCookie.Value);
            }
        }
    }
}
