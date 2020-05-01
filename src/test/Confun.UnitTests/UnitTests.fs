namespace Confun.UnitTests

open Xunit

module internal UnitTests =
    let testFail message = Assert.True(false, message)