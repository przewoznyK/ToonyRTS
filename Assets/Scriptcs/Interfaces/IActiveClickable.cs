using NUnit.Framework;
using System.Collections.Generic;

public interface IActiveClickable
{
    public ObjectTypeEnum CheckObjectType();
    public void ActiveObject();
    public List<UnitNameEnum> GetUnitsToBuyList();
}
