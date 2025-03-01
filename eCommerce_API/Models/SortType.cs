using System.ComponentModel;
using System.Runtime.Serialization;

namespace eCommerce_API.Models
{
    public enum SortType
    {
        //[EnumMember(Value = "Giá tăng dần")]
            [Description("Mặc định")]
            Default = 0,
            [Description("Giá tăng dần")]
            Ascending = 1,
            [Description("Giá giảm dần")]
            Decreasing = 2,
            [Description("% giảm giá")]
            Promotion = 3
    }
}
