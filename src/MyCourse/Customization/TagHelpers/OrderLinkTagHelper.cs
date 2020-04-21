

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCourse.Models.InputModels;

namespace MyCourse.Customization.TagHelpers
{
    public class OrderLinkTagHelper : AnchorTagHelper
    {
        //deriva dal tag helper anchor (a)

        public string OrderBy { get; set; }
        public CourseListInputModel Input { get; set; }



        public OrderLinkTagHelper(IHtmlGenerator generator) : base(generator)
        {
            //utilizzato per sfruttare la capacità del tag anchor di generare link
        }

        public override void Process(Microsoft.AspNetCore.Razor.TagHelpers.TagHelperContext context, Microsoft.AspNetCore.Razor.TagHelpers.TagHelperOutput output)
        {
            output.TagName = "a"; //a di anchor

            //Imposto i valori del link, indico all' anchor tag helper dove deve andare a navigare
            RouteValues["search"] = Input.Search;
            RouteValues["orderBy"] = OrderBy;
            RouteValues["ascending"] = (Input.OrderBy == OrderBy ? !Input.Ascending : Input.Ascending).ToString().ToLowerInvariant();

            //Faccio generare l'output html all'AnchorTagHelper
            base.Process(context, output);

            //Aggiungo l'indicatore di direzione
            if (Input.OrderBy == OrderBy)
            {
                var direction = Input.Ascending ? "up" : "down";
                output.PostContent.SetHtmlContent($" <i class=\"fas fa-caret-{direction}\"></i>");
                //e tramite PostContent aggiungo l'icona rappresentante il senso di ordinamento
            }
        }
    }
}