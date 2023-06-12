using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormFlow
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper, string expression)
            where TModel : class
            where TEnum : Enum
        {
            var values = new List<SelectListItem>();
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
            {
                values.Add(new SelectListItem
                {
                    Text = enumValue.ToString(),
                    Value = enumValue.ToString()
                });
            }

            return htmlHelper.DropDownList(expression, values);
        }
    }
}
