using Unity.Entities;
using Unity.Transforms;
namespace NKPB
{
    [UpdateBefore(typeof(CountGroup))]
    public class ScanGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(ScanGroup))]
    public class CountGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(CountGroup))]
    public class UpdateGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(UpdateGroup))]
    public class RenderGroup : ComponentSystemGroup {}
}
