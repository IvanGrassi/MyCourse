using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace MyCourse.Customization.ModelBinders
{
    public class DecimalModelBinderProvider: IModelBinderProvider
    {
        //permette di registrare a livello globale un mio model binder
        //ATTENZIONE: é da registrare anche nella startup.cs dopo addMvc
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            //esamino la proprietà verso la quale si vuole fare il binding dei dati
            //se é di tipo decimal: usa il DecimalModelBinder, altrimenti restituisci null
            if (context.Metadata.ModelType == typeof(decimal)) {
                return new DecimalModelBinder();
            }
            return null;
        }
    }
}