using System.Linq;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace Wilmar.Service.Core.OData
{
    /// <summary>
    /// 复合主键路由转换器
    /// </summary>
    internal class CompositeKeyRoutingConvention : EntityRoutingConvention
    {
        /// <summary>
        /// 重载选择Action
        /// </summary>
        /// <param name="odataPath"></param>
        /// <param name="controllerContext"></param>
        /// <param name="actionMap"></param>
        /// <returns></returns>
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            var action = base.SelectAction(odataPath, controllerContext, actionMap);
            if (action != null)
            {
                var routeValues = controllerContext.RouteData.Values;
                if (routeValues.ContainsKey(ODataRouteConstants.Key))
                {
                    var keyRaw = routeValues[ODataRouteConstants.Key] as string;
                    if (keyRaw.Contains(","))
                    {
                        string[] compoundKeyValues = keyRaw.Split(',');
                        for (int i = 0; i < compoundKeyValues.Length; i++)
                        {
                            routeValues.Add(ODataRouteConstants.Key + (i + 1).ToString(), compoundKeyValues[i].Trim());
                        }
                    }
                }
            }
            return action;
        }
    }
}
