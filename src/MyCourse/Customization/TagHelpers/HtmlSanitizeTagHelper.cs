using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Ganss.XSS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyCourse.Customization.TagHelpers
{

    [HtmlTargetElement(Attributes = "html-sanitize")]   //viene richiamato ogni volta che si usa l'attributo html-sanitize
    public class HtmlSanitizeTagHelper : TagHelper
    {
        //la pagina server per evitare il cross site scripting (iniettare del codice javascript dall'ispeziona pagina)

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //Otteniamo il contenuto del tag con il metodo a cui forniamo NullHtmlEncoder.Default che fornisce il risultato tale e quale (come si trova nel db)
            TagHelperContent tagHelperContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            string content = tagHelperContent.GetContent(NullHtmlEncoder.Default);  //restituisce la stringa

            var sanitizer = CreateSanitizer();      
            content = sanitizer.Sanitize(content);

            //Reimpostiamo il risultato all'interno del tag (il suo contenuto)
            output.Content.SetHtmlContent(content);
        }

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer(); //creo un istanza del sanitizer

            //Tutti i tag consentiti e che verranno inclusi nel risultato
            sanitizer.AllowedTags.Clear();      //rimuovo tutti i tag forniti di default e setto una mia lista di ciò che consento
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("ol");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("iframe");

            //Attributi consentiti (faccio la stessa cosa che ho fatto con i tag)
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowDataAttributes = false;

            //Stili consentiti: nessuna proprietà css accettata
            sanitizer.AllowedCssProperties.Clear();

            sanitizer.FilterUrl += FilterUrl;   //eseguo la funzione ogni volta che la libreria si trova ad esaminare un url
            sanitizer.PostProcessNode += ProcessIFrames;

            return sanitizer;
        }

        private static void FilterUrl(object sender, FilterUrlEventArgs filterUrlEventArgs)
        {
            //vado a verificare che l'url inizi con la stringa www.youtube.com oppure https
            if (!filterUrlEventArgs.OriginalUrl.StartsWith("//www.youtube.com/") && !filterUrlEventArgs.OriginalUrl.StartsWith("https://www.youtube.com/"))
            {
                filterUrlEventArgs.SanitizedUrl = null; //altrimenti imposti a null l'url
            }
        }
        
        private static void ProcessIFrames(object sender, PostProcessNodeEventArgs postProcessNodeEventArgs)
        {
            //ogni qualvolta la libreria incontra un iframe
            var iframe = postProcessNodeEventArgs.Node as IHtmlInlineFrameElement;
            if (iframe == null) {
                return;
            }
            //voglio che venga avvolto da una span su cui metto una speciale classe video-container
            var container = postProcessNodeEventArgs.Document.CreateElement("span");
            container.ClassName = "video-container";
            container.AppendChild(iframe.Clone(true));
            postProcessNodeEventArgs.ReplacementNodes.Add(container);
        }
    }
}