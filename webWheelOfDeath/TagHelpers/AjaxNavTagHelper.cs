using Microsoft.AspNetCore.Razor.TagHelpers;

namespace webWheelOfDeath.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "ajax-action")]
    [HtmlTargetElement("button", Attributes = "ajax-action")]
    public class AjaxNavTagHelper : TagHelper
    {
        public string AjaxAction { get; set; }
        public string AjaxController { get; set; }
        public long? AjaxId { get; set; }
        public string AjaxTarget { get; set; }
        public bool AjaxPost { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("data-ajax-nav", "");
            output.Attributes.SetAttribute("data-action", AjaxAction);

            if (!string.IsNullOrEmpty(AjaxController))
                output.Attributes.SetAttribute("data-controller", AjaxController);

            if (AjaxId.HasValue)
                output.Attributes.SetAttribute("data-id", AjaxId.Value);

            if (!string.IsNullOrEmpty(AjaxTarget))
                output.Attributes.SetAttribute("data-target", AjaxTarget);

            if (AjaxPost)
                output.Attributes.SetAttribute("data-method", "POST");

            // (so it's styled as a link if it's an anchor.)
            if (output.TagName == "a")
            {
                output.Attributes.SetAttribute("href", "#");
            }
        }
    }
}