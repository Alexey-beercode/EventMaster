using System.Text;
using EventMaster.BLL.DTOs.Interfaces;
using EventMaster.BLL.Exceptions;
using FluentValidation;

namespace EventMaster.BLL.DTOs.Implementations;

public abstract class BaseValidationModel<T> : IBaseValidationModel
{
    public void Validate(object validator, IBaseValidationModel modelObj)
    {
        var instance = (IValidator<T>)validator;
        var result = instance.Validate((T)modelObj);
    
        if (!result.IsValid && result.Errors.Any())
        {
            var errors =new StringBuilder();
            result.Errors.ForEach(a => errors.Append(a.ErrorMessage).Append('\n'));
            throw new BadRequestException(errors.ToString());
        }
    }
}