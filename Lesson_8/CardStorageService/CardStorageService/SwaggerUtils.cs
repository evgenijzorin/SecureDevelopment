using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace CardStorageService
{
    public static class SwaggerUtils
    {
        /// <summary>
        /// получить специальное имя контроллера соединенное с его действием
        /// </summary>
        /// <param name="apiDescription"></param>
        /// <returns></returns>
        public static string OperationIdProvider(ApiDescription apiDescription)
        {
            string controllerName = apiDescription.ActionDescriptor.RouteValues["controller"]; // получить имя контроллера из маршрута
            string actionName = apiDescription.ActionDescriptor.Id; 
           
            if (apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) // получить описание метода
            {
                actionName = methodInfo.Name;
                if(actionName.EndsWith("Async")) // если в имени метод помечен асинхронным
                {
                    actionName = actionName.Substring(0, actionName.Length - 5); // Убрать слово Async

                } 
            }
            return $"{controllerName}_{actionName}";
        }

    }
}
