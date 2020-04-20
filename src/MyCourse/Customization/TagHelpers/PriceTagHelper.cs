using Microsoft.AspNetCore.Razor.TagHelpers;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Customization.TagHelpers
{
    public class PriceTagHelper : TagHelper
    {
        //rappresenta i due tipi di prezzi
        public Money CurrentPrice { get; set; }
        public Money FullPrice { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Content.AppendHtml($"{CurrentPrice}");

            //genero anche il prezzo attuale se questo fosse differente
            if (!CurrentPrice.Equals(FullPrice))
            {
                output.Content.AppendHtml($"<br><s>{FullPrice}</s>");   //il "vecchio" prezzo si presenta barrato
            }
        }
    }
}