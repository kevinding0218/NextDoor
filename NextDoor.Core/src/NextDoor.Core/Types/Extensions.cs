using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NextDoor.Core.Types
{
    public static class Extensions
    {
        #region STRING Extensions
        public static string Underscore(this string value)
            => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        #endregion

        #region IConfiguration Extensions
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }
        #endregion
    }
}