using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WorkFlow.Form.HtmlHelperExtensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var enumType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;

            var values = Enum.GetValues(enumType);
            var items = new List<SelectListItem>();

            foreach (var value in values)
            {
                var item = new SelectListItem
                {
                    Text = Enum.GetName(enumType, value),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                };
                items.Add(item);
            }

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        private static string GetEnumDisplayName<TEnum>(TEnum value)
        {
            var displayName = value.ToString();
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var displayAttribute = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .FirstOrDefault() as DisplayAttribute;
                if (displayAttribute != null)
                {
                    displayName = displayAttribute.Name;
                }
            }

            return displayName;
        }
    }
}