using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC.Routing
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        private const string _namespacePrefix = "MVC.Controllers.";

        // if there are cases where namespaces and section names do not match neatly, use a dictionary to map sections to specific namespaces
        private Dictionary<string, string> _namespaceMap = new Dictionary<string, string>();

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            // get section name from route
            var sectionName = requestContext.RouteData.Values["section"].ToString();

            // section not provided, cannot search for controllers
            if (string.IsNullOrEmpty(sectionName)) return null;

            // get requested namespace by either finding it in the provided map, or appending the section to the prefix
            var requestedNamespace = _namespaceMap.ContainsKey(sectionName)
                ? _namespaceMap[sectionName]
                : $"{_namespacePrefix}{sectionName}";

            // get all valid matching controllers
            var controllersFound = GetControllers(controllerName, requestedNamespace);

            // no valid matches
            if (controllersFound.Count == 0) return null;

            // multiple controllers found
            if (controllersFound.Count > 1)
                throw CreateAmbiguousControllerException(
                    requestContext.RouteData.Route,
                    controllerName,
                    controllersFound
                );

            // return the single found controller
            return controllersFound.First();
        }

        // get all controller types within the executing assembly that match the given controller name and namespace
        private List<Type> GetControllers(string controllerName, string requestedNamespace) =>
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => IsControllerType(i)
                            && IsNamespaceMatch(requestedNamespace, i.Namespace)
                            && String.Equals(i.Name, $"{controllerName}Controller", StringComparison.OrdinalIgnoreCase))
                .ToList();

        // these functions are defined as "internal" in the MVC source code, so we need to extract them here.
        #region Extracted functions

        // determine if type is a controller type
        // original source: ControllerTypeCache.IsControllerType(Type t)
        private bool IsControllerType(Type t) => true
                && t != null && t.IsPublic && !t.IsAbstract
                && typeof(IController).IsAssignableFrom(t)
                && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);

        // check if the target namespace is equal to the requested namespace
        // if the requested namespace is wildcarded, check if the target namespace is equal or under the requested namespace
        // original source: IsNamespaceMatch.IsControllerType(string requestedNamespace, string targetNamespace)
        private bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            // looking for exact namespace match
            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
                return String.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);

            // looking for exact or sub-namespace match
            requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
            if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
                return false;

            // exact match
            if (requestedNamespace.Length == targetNamespace.Length)
                return true;

            // good prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar.Baz"
            if (targetNamespace[requestedNamespace.Length] == '.')
                return true;

            // bad prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar2"
            return false;
        }

        // create an exception for finding an ambiguous controller match
        // original source: DefaultControllerFactory.CreateAmbiguousControllerException(RouteBase route, string controllerName, ICollection<Type> matchingTypes)
        private InvalidOperationException CreateAmbiguousControllerException(RouteBase route, string controllerName, ICollection<Type> matchingTypes)
        {
            var errorFormats = new
            {
                WithoutRoute = "Multiple types were found that match the controller named '{0}'. This can happen if the route that services this request does not specify namespaces to search for a controller that matches the request. If this is the case, register this route by calling an overload of the 'MapRoute' method that takes a 'namespaces' parameter.{2}{2}The request for '{0}' has found the following matching controllers:{1}",
                WithRoute = "Multiple types were found that match the controller named '{0}'. This can happen if the route that services this request ('{1}') does not specify namespaces to search for a controller that matches the request. If this is the case, register this route by calling an overload of the 'MapRoute' method that takes a 'namespaces' parameter.{3}{3}The request for '{0}' has found the following matching controllers:{2}"
            };

            string typeList = String.Join(Environment.NewLine, matchingTypes.Select(i => i.FullName));

            string errorText;
            if (route is Route castRoute)
                errorText = String.Format(errorFormats.WithRoute, controllerName, castRoute.Url, typeList, Environment.NewLine);
            else
                errorText = String.Format(errorFormats.WithoutRoute, controllerName, typeList, Environment.NewLine);

            return new InvalidOperationException(errorText);
        }

        #endregion
    }
}