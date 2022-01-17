using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Utilities
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            var versionAPI = controllerNamespace.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}
