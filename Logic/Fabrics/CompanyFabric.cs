using System.Collections.Generic;
using Logic.Participants;
using Utilities.Common;

namespace Logic.Fabrics
{
    public class CompanyFabric : FabricBase<Company>, ICompanyFabric
    {
        protected override IEnumerable<string> EntityNames { get; set; } = new string[]
        {
            "Apple (USA)",
            "Microsoft (USA)",
            "Tesla (USA)",
            "Astana Banki (KZ)",
            "Kaspi Bank (KZ)",
            "HalykBank (KZ)",
            "Qazkom (KZ)",
            "Technodom (KZ)",
            "Sulpak (KZ)",
            "Google (USA)",
            "Bloomberg (USA)",
            "DELL (USA)",
            "Samsung (LOR)",
            "Xiaomi (CN)",
            "Meizu (CN)",
            "Toyota (JP)",
            "Subaru (JP)",
            "Twitter (USA)",
            "Youtube (USA)",
            "Delta Bank (KZ)",
            "BMW (DE)",
            "HTC (CN)",
            "Huawei (CN)",
            "LG Electronics (CN)",
            "Белый ветер (KZ)",
            "Sberbank (RU)"
        };
    }

    public interface ICompanyFabric : IFabricBase<Company> { }
}