using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnitTests.TypeValueDefaults;
using LibWheelOfDeath;
using LibEntity.NetCore.Infrastructure;
using static LibWheelOfDeath.EnumResultType;
using LibEntity.NetCore.Exceptions;

namespace UnitTests;

[TestClass]
public class CResultTests
{

    #region CONSTRUCTOR TESTS
    [TestMethod]
    public void CResultTest()
    {
        CResult result = new();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CResultTestWithId()
    {
        long id = 1L;
        CResult result = new(id);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ParametersInitialiseWithDefaultValuesTest()
    {
        CResult result = new();
        Assert.IsTrue(CDataDefaults.IsDefaultValue(result, nameof(result.Id)));
        Assert.IsTrue(CDataDefaults.IsDefaultValue(result, nameof(result.IsWin)));
        Assert.IsTrue(CDataDefaults.IsDefaultValue(result, nameof(result.ResultType)));
    }
    #endregion


    [DataTestMethod]
    [DataRow(0, null, Exceeded_Throws)]
    [DataRow(1, null, Default)]
    [DataRow(0, null, Timed_Out)]
    [DataRow(0, false, Default)]
    [DataRow(0, true, Default)]
    [DataRow(0, null, Killed)]
    public void CreateFailsOnDefaultParameter(long id, bool? isWin, EnumResultType resultType)
    {
        var result = new CResult
        {
            Id = id,
            IsWin = isWin,
            ResultType = resultType
        };

        Assert.ThrowsException<CEntityValidationException>(result.Create, $"Attempting to create {nameof(CResult)}");
    }

    [TestMethod()]
    public void UpdateTest()
    {
        Assert.Fail();
    }

    [DataTestMethod]
    [DataRow(0, true, Default)]
    [DataRow(0, false, Default)]
    [DataRow(0, false, Exceeded_Throws)]
    [DataRow(0, null, Timed_Out)]
    [DataRow(0, null, Killed)]
    [DataRow(1, null, Timed_Out)]
    [DataRow(2, null, Killed)]
    public void SearchTest(long id, bool? isWin, EnumResultType resultType)
    {
        CResult result = new()
        {
            Id = id,
            IsWin = isWin,
            ResultType = resultType
        };
        result.Search();
    }

    [TestMethod()]
    public void ResetTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void PopulateTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void ValidateTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void ToStringTest()
    {
        Assert.Fail();
    }
}
