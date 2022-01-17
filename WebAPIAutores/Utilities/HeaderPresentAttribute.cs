using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Utilities
{
    public class HeaderPresentAttribute: Attribute, IActionConstraint
    {
        private readonly string header;
        private readonly string value;

        public HeaderPresentAttribute(string header, string value)
        {
            this.header = header;
            this.value = value;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(header))
            {
                return false;
            }

            return string.Equals(headers[header], value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
