using Microsoft.AspNetCore.Razor.TagHelpers;
using TeamNest.Models.Entities;

namespace TeamNest.TagHelpers;

/// <summary>
/// Custom Tag Helper. In a view, write:  &lt;project-status value="@item.Status" /&gt;
/// It renders:  &lt;span class="status status-active"&gt;Active&lt;/span&gt;
/// </summary>
[HtmlTargetElement("project-status", TagStructure = TagStructure.WithoutEndTag)]
public class ProjectStatusTagHelper : TagHelper
{
    public ProjectStatus Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var (label, cssClass) = Value switch
        {
            ProjectStatus.Planning => ("Planning", "status-planning"),
            ProjectStatus.Active => ("Active", "status-active"),
            ProjectStatus.OnHold => ("On hold", "status-onhold"),
            ProjectStatus.Completed => ("Completed", "status-completed"),
            _ => (Value.ToString(), "status-planning")
        };

        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", $"status {cssClass}");
        output.Content.SetContent(label);
    }
}
