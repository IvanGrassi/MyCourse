using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyCourse.Customization.TagHelpers
{
    public class RatingTagHelper : TagHelper
    {
        //si ricollega al tag rating contenuto nella index.cshtml di courses
        //ATTENZIONE: se il tag helper non produce risultati, andare in viewimports e registrare il tag helper

        public double Value { get; set; }

        // viene invocato dal viewengine razor nel momento in cui il tag helper é chiamato a produrre output html
        //TagHelperContext ci permette di avere informazioni sul tag cosi com'é stato usato nella view
        //TagHelperOutput utilizzato per scrivere l'output html
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //double value = (double)context.AllAttributes["Value"].Value;

            for (int i = 1; i <= 5; i++)
            {
                if (Value >= i)          //es: 4.5 > 1, poi incremento: 4.5 > 2, ecc.., 4.5 < 5, stampa la stella intera
                {
                    output.Content.AppendHtml("<i class=\"fas fa-star\"></i>");
                }
                else if (Value > i - 1) //es: 4.5 > 5-1(4), allora stampa la stella a metà
                {
                    output.Content.AppendHtml("<i class=\"fas fa-star-half-alt\"></i>");
                }
                else
                {                           //in tutti gli altri casi: stella vuota
                    output.Content.AppendHtml("<i class=\"far fa-star\"></i>");
                }
            }
        }
    }
}