using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyCourse.Customization.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //vado ad ottenere il valore inviato dall'utente (quando ha inviato il form) grazie a un value provider
            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            
            //parsing del valore decimal
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue)) 
            {
                //il valore decimal andrà a finire nella proprietà deciamal
                bindingContext.Result = ModelBindingResult.Success(decimalValue);
            }
            return Task.CompletedTask;
        }
    }
}