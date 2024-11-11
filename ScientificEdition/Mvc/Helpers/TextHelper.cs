using Microsoft.Extensions.Localization;

namespace ScientificEdition.Mvc.Helpers
{
    public class TextHelper
    {
        private readonly IStringLocalizer stringLocalizer;

        public TextHelper(IStringLocalizerFactory localizerFactory)
            => stringLocalizer = localizerFactory.Create("WebsiteTexts", typeof(TextHelper).Assembly.GetName().Name!);

        public string? Text(string textKey, string? defaultText = null)
            => stringLocalizer.GetString(textKey) ?? defaultText;
    }
}
