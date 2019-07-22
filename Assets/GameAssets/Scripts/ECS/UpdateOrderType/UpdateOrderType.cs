using Unity.Entities;
using Unity.Transforms;
namespace NKPB
{
    public class ScanGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(ScanGroup))]
    public class CountGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(CountGroup))]
    public class InputGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(CountGroup))]
    public class MoveGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(MoveGroup))]
    public class JudgeGroup : ComponentSystemGroup {}

    [UpdateAfter(typeof(JudgeGroup))]
    public class PreRenderGroup : ComponentSystemGroup {}

    public class RenderGroup : ComponentSystemGroup {}

}
