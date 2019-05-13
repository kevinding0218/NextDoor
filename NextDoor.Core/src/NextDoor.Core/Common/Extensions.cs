using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NextDoor.Core.Common
{
    public static class Extensions
    {
        #region STRING Extensions
        public static string Underscore(this string value)
            => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        #endregion

        #region IConfiguration Extensions
        // bind properties of node in appsetting.json with class "TModel" property name
        // e.g: 
        // appsetting.json-> "serilog": {"consoleEnabled": true}
        // var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("serilog");
        // create a new instance of C# class "SerilogOptions" and bind the value of JSON "consoleEnabled" in appsetting.json to its C# property "ConsoleEnabled"
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }
        #endregion
    }
}