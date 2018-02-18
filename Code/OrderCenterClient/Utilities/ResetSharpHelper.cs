using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace OrderCenterClient.Utilities
{
    public static class ResetSharpHelper
    {
        private const string CONTENT_TYPE = "application/json";
        private static CultureInfo _currentCultureInfo = new CultureInfo("en-US");
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Objects,
            Culture = _currentCultureInfo
        };

        public static object Execute(MethodInfo methodInfo, object[] arguments)
        {
            var client = new RestClient("http://localhost:58660/");
            client.Timeout = 60 * 1000;

            var route = methodInfo.GetCustomAttributes<RouteAttribute>().First().Template;
            var attributes = methodInfo.GetCustomAttributes();
            ;
            var restRequest = new RestRequest(route, GetMethodType(attributes));

            var parameters = methodInfo.GetParameters();
            var segmentCount = route.Count(c => c == '{');
            for (var i = 0; i < segmentCount; i++)
            {
                restRequest.AddUrlSegment(parameters[i].Name, arguments[i].ToString());
            }

            // handles the parameters except segment parameters
            for (var i = segmentCount; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];
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

            var response = client.Execute(restRequest);

            int responseCode = (int)response.StatusCode;
            if (responseCode == 200)
            {
                return JsonConvert.DeserializeObject(response.Content, methodInfo.ReturnType, _jsonSerializerSettings);
                
            }
            else if (responseCode != 204)
            {
                if (string.IsNullOrEmpty(response.Content))
                {

                    throw new Exception(response.ErrorMessage);
                }
                else
                {

                    throw new WebException(response.Content);
                }
            }
            else
            {
          
            }

            return null;
        }

        private static Method GetMethodType(IEnumerable<Attribute> attributes)
        {
            foreach (var item in attributes)
            {
                if (item is IActionHttpMethodProvider)
                {
                    if (item is HttpGetAttribute)
                    {
                        return Method.GET;
                    }
                    else if (item is HttpPostAttribute)
                    {
                        return Method.POST;
                    }
                    else if (item is HttpPutAttribute)
                    {
                        return Method.PUT;
                    }
                    else
                    {
                        return Method.DELETE;
                    }
                }
            }

            throw new Exception("The method should set httpmethod");

        }

        private static void AddSimpleParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
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

        private static void AddArrayParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
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

        private static void AddObjectParameter(RestRequest restRequest, ParameterInfo parameter, Object argument, Type argumentType)
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
    }
}