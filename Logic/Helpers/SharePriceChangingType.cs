using System.ComponentModel;

namespace Logic.Helpers
{
    public enum SharePriceChangingType
    {
        [Description("Не изменяется")]
        Fixed = 0,

        [Description("Повышение")]
        Increasing = 1,

        [Description("Уменьшение")]
        Decreasing = 2
    }
}