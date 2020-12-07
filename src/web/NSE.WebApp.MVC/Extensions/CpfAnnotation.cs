using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NSE.Core.DomainObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace NSE.WebApp.MVC.Extensions
{
    public class CpfAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return Cpf.Validar(value.ToString()) ? ValidationResult.Success : new ValidationResult("CPF em formato inválido");
        }
    }

    /// <summary>
    /// Esse adapter é utilizado para que possa validar também no front-end , campos do tipo requerido
    /// </summary>
    public class CpfAttributeAdapter : AttributeAdapterBase<CpfAttribute>
    {
        public CpfAttributeAdapter(CpfAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer) 
        {

        }

        // validação de front-end
        public override void AddValidation(ClientModelValidationContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException();
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-cpf", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return "CPF em formato inválido";
        }
    }

    // provider para CPFValidation
    public class CpfValidationAttibuteAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if(attribute is CpfAttribute CpfAtribute)
            {
                return new CpfAttributeAdapter(CpfAtribute, stringLocalizer);
            }

            return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }

}
