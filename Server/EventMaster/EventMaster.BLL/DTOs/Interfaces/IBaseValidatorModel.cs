namespace EventMaster.BLL.DTOs.Interfaces;

public interface IBaseValidationModel
{
    public void Validate(object validator, IBaseValidationModel modelObj);
}