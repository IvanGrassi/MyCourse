using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyCourse.Customization.TagHelpers
{

    //il Tag Helper va in esecuzione per elementi input, quando é presente l'attributo asp-for
    [HtmlTargetElement("input", Attributes = "asp-for")]

    public class InputNumberTagHelper : TagHelper
    {
        //tag helper utilizzato per definire i numeri presenti nelle caselle dei prezzi con la virgola

        //override di Order che consente di impostare valore numerico (elevato) in modo che il Tag Helper 
        //vada in esecuzione dopo quello fornito da ASP.NET Core cosi da avere la possibilità di risolvere l'errore
        public override int Order => int.MaxValue; 

        [HtmlAttributeName("asp-for")]  //permette di ottenere il valore impostato su asp-for
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //l'attributo type é impostato su number, escludo tutti gli altri tipi di casella (text, email, ...)
           bool isNumberInputType = output.Attributes.Any(attribute => "type".Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase) && "number".Equals(attribute.Value as string, StringComparison.InvariantCultureIgnoreCase));
            if (!isNumberInputType)
            {
                return;                 //non é di tipo number: esco
            }
            if (For.ModelExplorer.ModelType != typeof(decimal))
            {
                return;                //se il numero é diverso da decimal, esco
            }
            //se invece é decimal: formatto secondo la InvariantCulture
            //il formato F2 produce due cifre decimali
            decimal value = (decimal) For.Model;
            string formattedValue = value.ToString("F2", CultureInfo.InvariantCulture);
            //SetAttribute reimposta il valore sull'attributo value
            output.Attributes.SetAttribute("value", formattedValue);
		}
    }
}